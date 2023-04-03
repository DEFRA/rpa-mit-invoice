using System.Diagnostics.CodeAnalysis;
using Invoices.Api.Models;
using Invoices.Api.Services.Models;
using Invoices.Api.Util;
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
        var query = _container.GetItemQueryIterator<InvoiceEntity>(new QueryDefinition(sqlCosmosQuery));

        var result = new List<InvoiceEntity>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            result.AddRange(response);
        }

        return InvoiceMapper.MapToInvoice(result);
    }

    public async Task<Invoice> Create(Invoice invoice)
    {
        var invoiceEntity = InvoiceMapper.MapToInvoiceEntity(invoice);
        await _container.CreateItemAsync<InvoiceEntity>(invoiceEntity, new PartitionKey(invoiceEntity.SchemeType));
        return invoice;
    }

    [ExcludeFromCodeCoverageAttribute]
    public async Task<BulkInvoices?> CreateBulk(BulkInvoices invoices)
    {
        var schemeType = invoices.SchemeType;
        var reference = invoices.Reference;
        var batch = _container.CreateTransactionalBatch(new PartitionKey(schemeType));

        foreach (var invoice in invoices.Invoices)
        {
            invoice.Reference = reference;
            var invoiceEntity = InvoiceMapper.MapToInvoiceEntity(invoice);
            batch.CreateItem<InvoiceEntity>(invoiceEntity);
        }

        TransactionalBatchResponse? batchResponse = await batch.ExecuteAsync();
        if (batchResponse.IsSuccessStatusCode)
        {
            return invoices;
        }

        return null;
    }

    public async Task<Invoice> Update(Invoice invoice)
    {
        var invoiceEntity = InvoiceMapper.MapToInvoiceEntity(invoice);
        await _container.UpsertItemAsync<InvoiceEntity>(invoiceEntity, new PartitionKey(invoice.SchemeType));
        return invoice;
    }

    public async Task<string> Delete(string id, string scheme)
    {
        await _container.DeleteItemAsync<InvoiceEntity>(id, new PartitionKey(scheme));
        return id;
    }
}