using System.Diagnostics.CodeAnalysis;
using Azure.Storage.Queues;
using Invoices.Api.Services;

namespace Invoices.Api.Endpoints;

[ExcludeFromCodeCoverage]
public static class QueueDefinition
{
    public static void AddQueueServices(this IServiceCollection services, string storageConnection, string eventQueueName, string paymentQueueName)
    {
        services.AddSingleton<IEventQueueService>(_ =>
        {
            var eventQueueClient = new QueueClient(storageConnection, eventQueueName);
            return new EventQueueService(eventQueueClient);
        });

        services.AddSingleton<IQueueService>(_ =>
        {
            var paymentQueueClient = new QueueClient(storageConnection, paymentQueueName);
            return new PaymentQueueService(paymentQueueClient);
        });
    }
}
