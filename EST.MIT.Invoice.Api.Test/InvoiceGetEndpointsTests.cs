using EST.MIT.Invoice.Api.Services;
using EST.MIT.Invoice.Api.Endpoints;
using FluentAssertions;
using EST.MIT.Invoice.Api.Models;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using EST.MIT.Invoice.Api.Services.PaymentsBatch;

namespace EST.MIT.Invoice.Api.Test;

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