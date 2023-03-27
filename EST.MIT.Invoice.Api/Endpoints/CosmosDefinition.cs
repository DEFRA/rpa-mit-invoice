using System.Diagnostics.CodeAnalysis;
using Invoices.Api.Services;
using Microsoft.Azure.Cosmos;

namespace Invoices.Api.Endpoints;

[ExcludeFromCodeCoverage]
public static class CosmosDefinition
{
    public static void AddCosmosServices(this IServiceCollection services, string url, string primaryKey, string databaseName, string containerName)
    {
        services.AddSingleton<ICosmosService>(_ =>
        {
            var cosmosClient = new CosmosClient(url, primaryKey);
            return new CosmosService(cosmosClient, databaseName, containerName);
        });
    }
}
