using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Invoices.Api.Models;
using Invoices.Api.Services;
using Moq;

namespace Invoices.Api.Test;

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
