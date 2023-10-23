using System.Diagnostics.CodeAnalysis;
using Azure.Identity;
using Azure.Storage.Queues;
using EST.MIT.Invoice.Api.Services;

namespace EST.MIT.Invoice.Api.Endpoints;

[ExcludeFromCodeCoverage]
public static class QueueDefinition
{
    public static void AddQueueServices(this IServiceCollection services, IConfiguration configuration)
    {
        var storageAccountCredential = configuration.GetSection("QueueConnectionString:Credential").Value;
        services.AddSingleton<IEventQueueService>(_ =>
        {
            var queueName = configuration.GetSection("EventQueueName").Value;
            if (IsManagedIdentity(storageAccountCredential))
            {
                var queueServiceUri = configuration.GetSection("QueueConnectionString:QueueServiceUri").Value;
                var queueUrl = new Uri($"{queueServiceUri}{queueName}");
                Console.WriteLine($"EventQueueService using Managed Identity with url {queueUrl}");
                return new EventQueueService(new QueueClient(queueUrl, new DefaultAzureCredential()));
            }
            else
            {
                return new EventQueueService(new QueueClient(configuration.GetSection("QueueConnectionString").Value, queueName));
            }
        });

        services.AddSingleton<IPaymentQueueService>(_ =>
        {
            var queueName = configuration.GetSection("PaymentQueueName").Value;
            if (IsManagedIdentity(storageAccountCredential))
            {
                var queueServiceUri = configuration.GetSection("QueueConnectionString:QueueServiceUri").Value;
                var queueUrl = new Uri($"{queueServiceUri}{queueName}");
                Console.WriteLine($"PaymentQueueService using Managed Identity with url {queueUrl}");
                return new PaymentQueueService(new QueueClient(queueUrl, new DefaultAzureCredential()));
            }
            else
            {
                return new PaymentQueueService(new QueueClient(configuration.GetSection("QueueConnectionString").Value, queueName));
            }
        });
    }

    private static bool IsManagedIdentity(string credentialName)
    {
        return credentialName != null && credentialName.ToLower() == "managedidentity";
    }
}
