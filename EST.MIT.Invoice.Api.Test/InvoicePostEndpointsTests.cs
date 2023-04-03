using Invoices.Api.Services;
using Invoices.Api.Endpoints;
using NSubstitute;
using FluentAssertions;
using Invoices.Api.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using NSubstitute.ReturnsExtensions;

namespace Invoices.Api.Test;

public class InvoicePostEndpointTests
{
    private readonly ICosmosService _cosmosService =
        Substitute.For<ICosmosService>();

    private readonly IEventQueueService _eventQueueService =
        Substitute.For<IEventQueueService>();

    private readonly Invoice invoiceTestData = InvoiceTestData.CreateInvoice();

    private readonly IValidator<Invoice> _validator = new InvoiceValidator();

    [Fact]
    public async Task PostInvoicebySchemeAndInvoiceId_WhenInvoiceDoesNotExist()
    {
        var invoice = invoiceTestData;

        _cosmosService.Create(invoice).Returns(invoice);
        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-created", "Invoice created").Returns(Task.CompletedTask);

        var result = await InvoicePostEndpoints.CreateInvoice(invoice, _validator, _cosmosService, _eventQueueService);

        result.GetCreatedStatusCode().Should().Be(201);
        result.GetCreatedResultValue<Invoice>().Should().BeEquivalentTo(invoice);
    }

    [Fact]
    public async Task PostInvoicebySchemeAndInvoiceId_WhenCreateReturnsNull()
    {
        var invoice = invoiceTestData;

        _cosmosService.Create(invoice).ReturnsNull();
        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-created", "Invoice created").Returns(Task.CompletedTask);

        var result = await InvoicePostEndpoints.CreateInvoice(invoice, _validator, _cosmosService, _eventQueueService);

        result.GetCreatedStatusCode().Should().Be(400);
    }

    [Theory]
    [ClassData(typeof(InvoiceValidationTestData))]
    public async Task PostInvoicebySchemeAndInvoiceId_WhenInvoiceMissingInvoiceProperties(string id, string scheme, string status, string errorKey)
    {
        var invoice = new Invoice
        {
            Id = id,
            SchemeType = scheme,
            Status = status,
            InvoiceType = "ap",
            PaymentRequests = new List<InvoiceHeader>
            {
                new()
                {
                    Value = 123456789
                }
            }
        };

        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-create-failed", "Invoice creation failed").Returns(Task.CompletedTask);
        var result = await InvoicePostEndpoints.CreateInvoice(invoice, _validator, _cosmosService, _eventQueueService);

        result.GetBadRequestResultValue<HttpValidationProblemDetails>().Should().NotBeNull();
        result?.GetBadRequestResultValue<HttpValidationProblemDetails>()?.Errors.Should().ContainKey(errorKey);
    }
}