using System.Diagnostics.CodeAnalysis;
using Azure.Storage.Queues;

namespace EST.MIT.Invoice.Api.Services;

[ExcludeFromCodeCoverage]
public class PaymentQueueService : IPaymentQueueService
{
    private readonly QueueClient _queueClient;

    public PaymentQueueService(QueueClient queueClient)
    {
        _queueClient = queueClient;
    }

    public async Task CreateMessage(string message)
    {
        await _queueClient.SendMessageAsync(message);
    }
}