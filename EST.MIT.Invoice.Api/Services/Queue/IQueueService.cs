namespace Invoices.Api.Services;

public interface IQueueService
{
    Task CreateMessage(string message);
}