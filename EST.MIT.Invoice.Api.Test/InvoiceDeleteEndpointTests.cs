using FluentAssertions;
using EST.MIT.Invoice.Api.Endpoints;
using EST.MIT.Invoice.Api.Services;
using EST.MIT.Invoice.Api.Services.PaymentsBatch;
using NSubstitute;

namespace EST.MIT.Invoice.Api.Test;

public class InvoiceDeleteEndpointTests
{
    private readonly IPaymentRequestsBatchService _paymentRequestsBatchService =
            Substitute.For<IPaymentRequestsBatchService>();

    private readonly IEventQueueService _eventQueueService =
        Substitute.For<IEventQueueService>();

    [Fact]
    public async Task DeleteInvoice_ValidArguments_DeletesFromCosmosAndSendsMessageToQueue()
    {
        // Arrange
        const string id = "invoice-123";
        const string scheme = "invoices";

        _paymentRequestsBatchService.DeleteBySchemeAndIdAsync(scheme, id).Returns(id);
        _eventQueueService.CreateMessage(id, "deleted", "invoice-deleted", "Invoice deleted").Returns(Task.CompletedTask);
        var result = await InvoiceDeleteEndpoints.DeleteInvoice(id, scheme, _paymentRequestsBatchService, _eventQueueService);

        result.GetCreatedStatusCode().Should().Be(200);
    }
}