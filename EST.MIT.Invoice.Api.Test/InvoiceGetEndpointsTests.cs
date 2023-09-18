using Invoices.Api.Services;
using Invoices.Api.Endpoints;
using FluentAssertions;
using Invoices.Api.Models;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Invoices.Api.Services.PaymentsBatch;

namespace Invoices.Api.Test;

public class InvoiceGetEndpointTests
{
    private readonly IPaymentRequestsBatchService _paymentRequestsBatchService =
        Substitute.For<IPaymentRequestsBatchService>();

    private readonly PaymentRequestsBatch _paymentRequestsBatchTestData = PaymentRequestsBatchTestData.CreateInvoice();

    [Fact]
    public async Task GetInvoicebySchemeAndInvoiceId_WhenInvoiceExists()
    {
        const string scheme = "bps";
        const string invoiceId = "123456789";

        var invoice = _paymentRequestsBatchTestData;

        _paymentRequestsBatchService.GetBySchemeAndIdAsync(scheme, invoiceId).Returns(new List<PaymentRequestsBatch> { invoice });

        var result = await InvoiceGetEndpoints.GetInvoice(scheme, invoiceId, _paymentRequestsBatchService);

        result.GetOkObjectResultValue<PaymentRequestsBatch>().Should().BeEquivalentTo(invoice);
        result.GetOkObjectResultStatusCode().Should().Be(200);
    }

    [Fact]
    public async Task GetInvoicebySchemeAndInvoiceId_WhenInvoiceDoesNotExists()
    {
        const string scheme = "bps";
        const string invoiceId = "123456789";

        _paymentRequestsBatchService.GetBySchemeAndIdAsync(scheme, invoiceId)
            .ReturnsNull();

        var result = await InvoiceGetEndpoints.GetInvoice(scheme, invoiceId, _paymentRequestsBatchService);

        result.GetNotFoundResultStatusCode().Should().Be(404);
    }
}