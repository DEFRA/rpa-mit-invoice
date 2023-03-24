using Invoices.Api.Services;
using Invoices.Api.Endpoints;
using NSubstitute;
using FluentAssertions;
using Invoices.Api.Services.Models;
using NSubstitute.ReturnsExtensions;
using Invoices.Api.Models;

namespace Invoices.Api.Test;

public class InvoiceGetEndpointTests
{
    private readonly ITableService _tableService =
        Substitute.For<ITableService>();

    private readonly Invoice invoiceTestData = InvoiceTestData.CreateInvoice();

    [Fact]
    public async Task GetInvoicebySchemeAndInvoiceId_WhenInvoiceExists()
    {
        const string scheme = "bps";
        const string invoiceId = "123456789";

        var invoice = invoiceTestData;

        _tableService.GetInvoice(scheme, invoiceId)
            .Returns(new InvoiceEntity
            {
                PartitionKey = scheme,
                RowKey = invoiceId,
                Status = invoice.Status,
                Data = System.Text.Json.JsonSerializer.Serialize(invoice),
                ETag = Azure.ETag.All,
                Timestamp = DateTimeOffset.UtcNow
            });

        var result = await InvoiceEndpoints.GetInvoice(scheme, invoiceId, _tableService);

        result.GetOkObjectResultValue<Invoice>().Should().BeEquivalentTo(invoice);
        result.GetOkObjectResultStatusCode().Should().Be(200);
    }

    [Fact]
    public async Task GetInvoicebySchemeAndInvoiceId_WhenInvoiceDoesNotExists()
    {
        const string scheme = "bps";
        const string invoiceId = "123456789";

        _tableService.GetInvoice(scheme, invoiceId).ReturnsNull();

        var result = await InvoiceEndpoints.GetInvoice(scheme, invoiceId, _tableService);

        result.GetNotFoundResultStatusCode().Should().Be(404);
    }
}