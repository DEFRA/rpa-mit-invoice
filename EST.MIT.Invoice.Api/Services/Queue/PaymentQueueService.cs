using System.Diagnostics.CodeAnalysis;

namespace EST.MIT.Invoice.Api.Services;

[ExcludeFromCodeCoverage]
public class PaymentQueueService : IPaymentQueueService
{
    private readonly IConfiguration _configuration;
    private readonly ServiceBusProvider _serviceBusProvider;

    public PaymentQueueService(ServiceBusProvider serviceBusProvider, IConfiguration configuration)
    {
        _serviceBusProvider = serviceBusProvider;
        _configuration = configuration;
    }

    public async Task CreateMessage(string message)
    {
        await _serviceBusProvider.SendMessageAsync(_configuration["PaymentQueueName"], message);
    }
}