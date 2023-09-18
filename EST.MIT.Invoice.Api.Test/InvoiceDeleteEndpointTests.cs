using FluentAssertions;
using Invoices.Api.Endpoints;
using Invoices.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;

namespace Invoices.Api.Test;

public class InvoiceDeleteEndpointTests
{
    private readonly IInvoiceService _invoiceService =
            Substitute.For<IInvoiceService>();

    private readonly IEventQueueService _eventQueueService =
        Substitute.For<IEventQueueService>();

    [Fact]
    public async Task DeleteInvoice_ValidArguments_DeletesFromCosmosAndSendsMessageToQueue()
    {
        // Arrange
        const string id = "invoice-123";
        const string scheme = "invoices";

        _invoiceService.DeleteBySchemeAndIdAsync(scheme, id).Returns(id);
        _eventQueueService.CreateMessage(id, "deleted", "invoice-deleted", "Invoice deleted").Returns(Task.CompletedTask);
        var result = await InvoiceDeleteEndpoints.DeleteInvoice(id, scheme, _invoiceService, _eventQueueService);

        result.GetCreatedStatusCode().Should().Be(200);
    }
}