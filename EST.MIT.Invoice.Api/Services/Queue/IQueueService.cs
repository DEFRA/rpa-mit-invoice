namespace EST.MIT.Invoice.Api.Services;

public interface IQueueService
{
    Task CreateMessage(string message);
}