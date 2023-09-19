using EST.MIT.Invoice.Api.Models;

namespace EST.MIT.Invoice.Api.Services;

public interface IEventQueueService
{
    Task CreateMessage(string id, string status, string action, string message, PaymentRequestsBatch? invoice = null);
}