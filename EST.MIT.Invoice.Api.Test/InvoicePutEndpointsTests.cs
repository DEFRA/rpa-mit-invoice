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
            Scheme = scheme,
            Status = "awaiting",
            CreatedBy = "test",
            UpdatedBy = "test",
            Header = new InvoiceHeader
            {
                Id = "123456789",
                ClaimReference = "123456789",
                ClaimReferenceNumber = "123456789",
                FRN = "123456789",
                AgreementNumber = "123456789",
                Currency = "GBP",
                Description = "Test"
            }
        };

        _tableService.UpdateInvoice(invoice).Returns(true);

        var result = await InvoiceEndpoints.UpdateInvoice(invoice, _tableService, _queueService, _validator);

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
            Scheme = scheme,
            Status = "awaiting",
            CreatedBy = "test",
            UpdatedBy = "test",
            Header = new InvoiceHeader
            {
                Id = "123456789",
                ClaimReference = "123456789",
                ClaimReferenceNumber = "123456789",
                FRN = "123456789",
                AgreementNumber = "123456789",
                Currency = "GBP",
                Description = "Test"
            }
        };

        _tableService.UpdateInvoice(invoice).Returns(false);

        var result = await InvoiceEndpoints.UpdateInvoice(invoice, _tableService, _queueService, _validator);

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
            Scheme = scheme,
            Status = "approved",
            CreatedBy = "test",
            UpdatedBy = "test",
            Header = new InvoiceHeader
            {
                Id = "123456789",
                ClaimReference = "123456789",
                ClaimReferenceNumber = "123456789",
                FRN = "123456789",
                AgreementNumber = "123456789",
                Currency = "GBP",
                Description = "Test"
            }
        };

        _tableService.UpdateInvoice(invoice).Returns(true);

        var result = await InvoiceEndpoints.UpdateInvoice(invoice, _tableService, _queueService, _validator);

        result.GetOkObjectResultStatusCode().Should().Be(200);
        result.GetOkObjectResultValue<Invoice>().Should().BeEquivalentTo(invoice);

        var expectedMessage = $"{{\"id\":\"{invoiceId}\",\"scheme\":\"{scheme}\"}}";
        await _queueService.Received().CreateMessage(expectedMessage);
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
            CreatedBy = createdBy
        };

        var result = await InvoiceEndpoints.UpdateInvoice(invoice, _tableService, _queueService, _validator);

        result.GetBadRequestResultValue<HttpValidationProblemDetails>().Should().NotBeNull();
        result?.GetBadRequestResultValue<HttpValidationProblemDetails>()?.Errors.Should().ContainKey(errorKey);
    }
}