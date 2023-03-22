using Invoices.Api.Services;
using Invoices.Api.Endpoints;
using NSubstitute;
using FluentAssertions;
using Invoices.Api.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Invoices.Api.Test;

public class InvoicePutEndpointTests
{
    private readonly ITableService _tableService =
        Substitute.For<ITableService>();

    private readonly IQueueService _queueService =
        Substitute.For<IQueueService>();

    private readonly IValidator<Invoice> _validator = new InvoiceValidator();

    [Fact]
    public async Task PutInvoicebySchemeAndInvoiceId_WhenInvoiceExists()
    {
        const string scheme = "bps";
        const string invoiceId = "123456789";

        var invoice = new Invoice
        {
            Id = invoiceId,
            SchemeType = scheme,
            Status = "awaiting",
            InvoiceType = "ap",
            Headers = new List<InvoiceHeader>
            {
                new()
                {
                    Value = 123456789
                }
            }
        };

        _tableService.UpdateInvoice(invoice).Returns(true);

        var result = await InvoiceEndpoints.UpdateInvoice(invoiceId, invoice, _tableService, _queueService, _validator);

        result.GetOkObjectResultStatusCode().Should().Be(200);
        result.GetOkObjectResultValue<Invoice>().Should().BeEquivalentTo(invoice);

        await _queueService.DidNotReceive().CreateMessage("");
    }

    [Fact]
    public async Task PostInvoicebySchemeAndInvoiceId_WhenInvoiceDoesNotExist()
    {
        const string scheme = "bps";
        const string invoiceId = "123456789";

        var invoice = new Invoice
        {
            Id = invoiceId,
            SchemeType = scheme,
            Status = "awaiting",
            InvoiceType = "ap",
            Headers = new List<InvoiceHeader>
            {
                new()
                {
                    Value = 123456789
                }
            }
        };

        _tableService.UpdateInvoice(invoice).Returns(false);

        var result = await InvoiceEndpoints.UpdateInvoice(invoiceId, invoice, _tableService, _queueService, _validator);

        result.GetBadRequestStatusCode().Should().Be(400);

        await _queueService.DidNotReceive().CreateMessage("");
    }

    [Fact]
    public async Task PutInvoicebySchemeAndInvoiceId_WhenApproved()
    {
        const string scheme = "bps";
        const string invoiceId = "123456789";

        var invoice = new Invoice
        {
            Id = invoiceId,
            SchemeType = scheme,
            Status = "approved",
            InvoiceType = "ap",
            Headers = new List<InvoiceHeader>
            {
                new()
                {
                    Value = 123456789
                }
            }
        };

        _tableService.UpdateInvoice(invoice).Returns(true);

        var result = await InvoiceEndpoints.UpdateInvoice(invoiceId, invoice, _tableService, _queueService, _validator);

        result.GetOkObjectResultStatusCode().Should().Be(200);
        result.GetOkObjectResultValue<Invoice>().Should().BeEquivalentTo(invoice);

        var expectedMessage = $"{{\"id\":\"{invoiceId}\",\"scheme\":\"{scheme}\"}}";
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

        var result = await InvoiceEndpoints.UpdateInvoice(id, invoice, _tableService, _queueService, _validator);

        result.GetBadRequestResultValue<HttpValidationProblemDetails>().Should().NotBeNull();
        result?.GetBadRequestResultValue<HttpValidationProblemDetails>()?.Errors.Should().ContainKey(errorKey);
    }
}