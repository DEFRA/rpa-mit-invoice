using System.Text;
using System.Text.Json;
using Azure.Storage.Queues;
using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Services.Models;

namespace EST.MIT.Invoice.Api.Services;

public class EventQueueService : IEventQueueService
{
    private readonly QueueClient _queueClient;

    public EventQueueService(QueueClient queueClient)
    {
        _queueClient = queueClient;
    }

    public async Task CreateMessage(string id, string status, string action, string message, PaymentRequestsBatch? invoice = null)
    {
        var eventRequest = new Event()
        {
            Name = "Invoice",
            Properties = new EventProperties()
            {
                Id = id,
                Status = status,
                Checkpoint = "Invoice Api",
                Action = new EventAction()
                {
                    Type = action,
                    Message = message,
                    Timestamp = DateTime.UtcNow,
                    Data = JsonSerializer.Serialize(invoice)
                }
            }
        };

        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventRequest));
        await _queueClient.SendMessageAsync(Convert.ToBase64String(bytes));
    }
}