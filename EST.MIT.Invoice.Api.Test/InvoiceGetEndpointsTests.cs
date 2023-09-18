using Invoices.Api.Services;
using Invoices.Api.Endpoints;
using FluentAssertions;
using Invoices.Api.Models;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Invoices.Api.Test;

public class InvoiceGetEndpointTests
{
    private readonly IInvoiceService _invoiceService =
        Substitute.For<IInvoiceService>();

    private readonly Invoice invoiceTestData = InvoiceTestData.CreateInvoice();

    [Fact]
    public async Task GetInvoicebySchemeAndInvoiceId_WhenInvoiceExists()
    {
        const string scheme = "bps";
        const string invoiceId = "123456789";

        var invoice = invoiceTestData;

        _invoiceService.GetBySchemeAndIdAsync(scheme, invoiceId).Returns(new List<Invoice> { invoice });

        var result = await InvoiceGetEndpoints.GetInvoice(scheme, invoiceId, _invoiceService);

        result.GetOkObjectResultValue<Invoice>().Should().BeEquivalentTo(invoice);
        result.GetOkObjectResultStatusCode().Should().Be(200);
    }

    [Fact]
    public async Task GetInvoicebySchemeAndInvoiceId_WhenInvoiceDoesNotExists()
    {
        const string scheme = "bps";
        const string invoiceId = "123456789";

        _invoiceService.GetBySchemeAndIdAsync(scheme, invoiceId)
            .ReturnsNull();

        var result = await InvoiceGetEndpoints.GetInvoice(scheme, invoiceId, _invoiceService);

        result.GetNotFoundResultStatusCode().Should().Be(404);
    }
}