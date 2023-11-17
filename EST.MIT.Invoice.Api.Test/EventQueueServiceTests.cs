using Azure.Storage.Queues;
using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Services;
using Moq;

namespace EST.MIT.Invoice.Api.Test;

public class EventQueueServiceTests
{
    [Fact]
    public async Task CreateMessage_ValidArguments_CallsSendMessageAsync()
    {
        var queueClientMock = new Mock<QueueClient>();
        var eventQueueService = new EventQueueService(queueClientMock.Object);
        var id = Guid.NewGuid().ToString();
        const string status = "new";
        const string action = "create";
        const string message = "Invoice created successfully";
        var invoice = new PaymentRequestsBatch { Id = "123456789", Status = "new" };

        await eventQueueService.CreateMessage(id, status, action, message, invoice);

        queueClientMock.Verify(qc => qc.SendMessageAsync(It.IsAny<string>()), Times.Once);
    }
}
