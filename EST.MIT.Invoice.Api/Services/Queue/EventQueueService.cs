using System.Text;
using System.Text.Json;
using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Services.Models;

namespace EST.MIT.Invoice.Api.Services;

public class EventQueueService : IEventQueueService
{
    private readonly IConfiguration _configuration;
    private readonly ServiceBusProvider _serviceBusProvider;

    public EventQueueService(ServiceBusProvider serviceBusProvider, IConfiguration configuration)
    {
        _serviceBusProvider = serviceBusProvider;
        _configuration = configuration;
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

        await _serviceBusProvider.SendMessageAsync(_configuration["EventQueueName"] ,JsonSerializer.Serialize(eventRequest));
    }
}