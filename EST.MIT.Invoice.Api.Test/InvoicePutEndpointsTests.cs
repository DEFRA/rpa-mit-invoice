using Invoices.Api.Services;
using Invoices.Api.Endpoints;
using NSubstitute;
using FluentAssertions;
using Invoices.Api.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Invoices.Api.Test;

public class InvoicePutEndpointTests
{
    private readonly ICosmosService _cosmosService =
        Substitute.For<ICosmosService>();

    private readonly IQueueService _queueService =
        Substitute.For<IQueueService>();

    private readonly IValidator<Invoice> _validator = new InvoiceValidator();

    private readonly Invoice invoiceTestData = InvoiceTestData.CreateInvoice();

    [Fact]
    public async Task PutInvoicebySchemeAndInvoiceId_WhenInvoiceExists()
    {
        var invoice = invoiceTestData;

        _cosmosService.Update(invoice).Returns(invoice);

        var result = await InvoiceEndpoints.UpdateInvoice(invoice.Id, invoice, _cosmosService, _queueService, _validator);

        result.GetOkObjectResultStatusCode().Should().Be(200);
        result.GetOkObjectResultValue<Invoice>().Should().BeEquivalentTo(invoice);

        await _queueService.DidNotReceive().CreateMessage("");
    }

    [Fact]
    public async Task PutInvoicebySchemeAndInvoiceId_WhenApproved()
    {
        var invoice = InvoiceTestData.CreateInvoice("approved");

        _cosmosService.Update(invoice).Returns(invoice);

        var result = await InvoiceEndpoints.UpdateInvoice(invoice.Id, invoice, _cosmosService, _queueService, _validator);

        result.GetOkObjectResultStatusCode().Should().Be(200);
        result.GetOkObjectResultValue<Invoice>().Should().BeEquivalentTo(invoice);

        var expectedMessage = JsonSerializer.Serialize(new InvoiceGenerator { Id = invoice.Id, Scheme = invoice.SchemeType });
        await _queueService.Received().CreateMessage(expectedMessage);
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
        };

        var result = await InvoiceEndpoints.UpdateInvoice(id, invoice, _cosmosService, _queueService, _validator);

        result.GetBadRequestResultValue<HttpValidationProblemDetails>().Should().NotBeNull();
        result?.GetBadRequestResultValue<HttpValidationProblemDetails>()?.Errors.Should().ContainKey(errorKey);
    }
}