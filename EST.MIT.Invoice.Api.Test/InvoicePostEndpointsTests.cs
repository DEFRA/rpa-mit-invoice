using Invoices.Api.Services;
using Invoices.Api.Endpoints;
using NSubstitute;
using FluentAssertions;
using Invoices.Api.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Invoices.Api.Test;

public class InvoicePostEndpointTests
{
    private readonly ITableService _tableService =
        Substitute.For<ITableService>();

    private readonly IValidator<Invoice> _validator = new InvoiceValidator();

    [Fact]
    public async Task PostInvoicebySchemeAndInvoiceId_WhenInvoiceExists()
    {
        const string scheme = "bps";
        const string invoiceId = "123456789";

        var invoice = new Invoice
        {
            Id = invoiceId,
            Scheme = scheme,
            Status = "Awaiting",
            CreatedBy = "test",
            Header = new InvoiceHeader { Id = "123456789" }
        };

        _tableService.CreateInvoice(invoice).Returns(false);

        var result = await InvoiceEndpoints.CreateInvoice(invoice, _tableService, _validator);

        result.GetBadRequestStatusCode().Should().Be(400);
    }

    [Fact]
    public async Task PostInvoicebySchemeAndInvoiceId_WhenInvoiceDoesNotExist()
    {
        const string scheme = "bps";
        const string invoiceId = "123456789";

        var invoice = new Invoice
        {
            Id = invoiceId,
            Scheme = scheme,
            Status = "awaiting",
            CreatedBy = "test",
            Header = new InvoiceHeader { Id = "123456789" }
        };

        _tableService.CreateInvoice(invoice).Returns(true);

        var result = await InvoiceEndpoints.CreateInvoice(invoice, _tableService, _validator);

        result.GetCreatedStatusCode().Should().Be(201);
        result.GetCreatedResultValue<Invoice>().Should().BeEquivalentTo(invoice);
    }

    [Theory]
    [ClassData(typeof(InvoiceValidationTestData))]
    public async Task PostInvoicebySchemeAndInvoiceId_WhenInvoiceMissingInvoiceProperties(string id, string scheme, string status, string createdBy, string errorKey)
    {
        var invoice = new Invoice
        {
            Id = id,
            Scheme = scheme,
            Status = status,
            CreatedBy = createdBy,
            Header = new InvoiceHeader { Id = "123456789" }
        };

        var result = await InvoiceEndpoints.CreateInvoice(invoice, _tableService, _validator);

        result.GetBadRequestResultValue<HttpValidationProblemDetails>().Should().NotBeNull();
        result?.GetBadRequestResultValue<HttpValidationProblemDetails>()?.Errors.Should().ContainKey(errorKey);
    }
}