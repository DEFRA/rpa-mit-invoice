using System.Diagnostics.CodeAnalysis;
using Azure.Storage.Queues;

namespace Invoices.Api.Services;

[ExcludeFromCodeCoverage]
public class QueueService : IQueueService
{
    private readonly QueueClient _queueClient;

    public QueueService(QueueClient queueClient)
    {
        _queueClient = queueClient;
    }

    public async Task CreateMessage(string message)
    {
        await _queueClient.SendMessageAsync(message);
    }
}