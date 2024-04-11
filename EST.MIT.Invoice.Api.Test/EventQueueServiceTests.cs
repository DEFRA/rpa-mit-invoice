using Azure.Messaging.ServiceBus;
using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace EST.MIT.Invoice.Api.Test;

public class EventQueueServiceTests
{
    private readonly Mock<ServiceBusProvider> _mockServiceBusProvider;
    private readonly Mock<IConfiguration> _mockConfiguration;

    public EventQueueServiceTests()
    {
        _mockServiceBusProvider = new Mock<ServiceBusProvider>();
        _mockConfiguration = new Mock<IConfiguration>();
    }

    [Fact]
    public async Task CreateMessage_ValidArguments_CallsSendMessageAsync()
    {
        _mockServiceBusProvider.Setup(s => s.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        var service = new EventQueueService(_mockServiceBusProvider.Object, _mockConfiguration.Object);

        var id = Guid.NewGuid().ToString();
        const string status = "new";
        const string action = "create";
        const string message = "Invoice created successfully";
        var invoice = new PaymentRequestsBatch { Id = "123456789", Status = "new" };

        await service.CreateMessage(id, status, action, message, invoice);

        _mockServiceBusProvider.Verify(qc => qc.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}
