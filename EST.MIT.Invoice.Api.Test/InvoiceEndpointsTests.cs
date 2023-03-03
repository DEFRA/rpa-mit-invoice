using Invoices.Api.Services;
using Invoices.Api.Endpoints;
using NSubstitute;
using FluentAssertions;
using Invoices.Api.Services.Models;
using NSubstitute.ReturnsExtensions;

namespace EST.MIT.Invoice.Api.Test;

public class InvoiceEndpointTests
{
    private readonly ITableService _tableService =
        Substitute.For<ITableService>();

    [Fact]
    public async Task GetInvoicebySchemeAndInvoiceId_WhenCustomerExists()
    {
        const string scheme = "bps";
        const string invoiceId = "123456789";

        var invoice = new Invoices.Api.Models.Invoice
        {
            Id = invoiceId,
            Scheme = scheme,
            Status = "Awaiting"
        };

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

        result.GetOkObjectResultValue<Invoices.Api.Models.Invoice>().Should().BeEquivalentTo(invoice);
        result.GetOkObjectResultStatusCode().Should().Be(200);
    }

    [Fact]
    public async Task GetInvoicebySchemeAndInvoiceId_WhenCustomerDoesNotExists()
    {
        const string scheme = "bps";
        const string invoiceId = "123456789";

        _tableService.GetInvoice(scheme, invoiceId).ReturnsNull();

        var result = await InvoiceEndpoints.GetInvoice(scheme, invoiceId, _tableService);

        result.GetNotFoundResultStatusCode().Should().Be(404);
    }
}