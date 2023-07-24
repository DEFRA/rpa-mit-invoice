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
    private readonly ICosmosService _cosmosService =
        Substitute.For<ICosmosService>();

    private readonly Invoice invoiceTestData = InvoiceTestData.CreateInvoice();

    [Fact]
    public async Task GetInvoicebySchemeAndInvoiceId_WhenInvoiceExists()
    {
        const string scheme = "bps";
        const string invoiceId = "123456789";

        var invoice = invoiceTestData;

        var sqlCosmosQuery = $"SELECT * FROM c WHERE c.schemeType = '{scheme}' AND c.id = '{invoiceId}'";
        _cosmosService.Get(sqlCosmosQuery)
            .Returns(new List<Invoice> { invoice });

        var result = await InvoiceGetEndpoints.GetInvoice(scheme, invoiceId, _cosmosService);

        result.GetOkObjectResultValue<Invoice>().Should().BeEquivalentTo(invoice);
        result.GetOkObjectResultStatusCode().Should().Be(200);
    }

    [Fact]
    public async Task GetInvoicebySchemeAndInvoiceId_WhenInvoiceDoesNotExists()
    {
        const string scheme = "bps";
        const string invoiceId = "123456789";

        var sqlCosmosQuery = $"SELECT * FROM c WHERE c.schemeType = '{scheme}' AND c.id = '{invoiceId}'";
        _cosmosService.Get(sqlCosmosQuery).ReturnsNull();

        var result = await InvoiceGetEndpoints.GetInvoice(scheme, invoiceId, _cosmosService);

        result.GetNotFoundResultStatusCode().Should().Be(404);
    }
}