namespace EST.MIT.Invoice.Api.Services;

public interface IPaymentQueueService
{
    Task CreateMessage(string message);
}