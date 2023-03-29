using Invoices.Api.Models;

namespace Invoices.Api.Services;

public interface IEventQueueService
{
    Task CreateMessage(string id, string status, string action, string message, Invoice? invoice = null);
}