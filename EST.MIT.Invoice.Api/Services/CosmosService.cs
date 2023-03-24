using Invoices.Api.Models;
using Microsoft.Azure.Cosmos;

namespace Invoices.Api.Services;

public class CosmosService : ICosmosService
{
    private readonly Container _container;
    public CosmosService(CosmosClient cosmosClient, string databaseName, string containerName)
    {
        _container = cosmosClient.GetContainer(databaseName, containerName);
    }

    public async Task<List<Invoice>> Get(string sqlCosmosQuery)
    {
        var query = _container.GetItemQueryIterator<Invoice>(new QueryDefinition(sqlCosmosQuery));

        var result = new List<Invoice>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            result.AddRange(response);
        }

        return result;
    }

    public async Task<Invoice> Create(Invoice invoice)
    {
        return await _container.CreateItemAsync<Invoice>(invoice, new PartitionKey(invoice.SchemeType));
    }

    public async Task<Invoice> Update(Invoice invoice)
    {
        return await _container.UpsertItemAsync<Invoice>(invoice, new PartitionKey(invoice.SchemeType));
    }
}