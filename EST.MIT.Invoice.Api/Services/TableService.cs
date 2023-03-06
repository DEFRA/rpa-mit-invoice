using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using Invoices.Api.Models;
using Invoices.Api.Services.Models;

namespace Invoices.Api.Services;

[ExcludeFromCodeCoverage]
public class TableService : ITableService
{
    private readonly TableClient _tableClient;

    public TableService(TableServiceClient tableServiceClient, IConfiguration configuration)
    {
        var tableName = configuration.GetValue<string>("Storage:TableName");
        _tableClient = tableServiceClient.GetTableClient(tableName);
    }

    public async Task<InvoiceEntity?> GetInvoice(string scheme, string invoiceId)
    {
        try
        {
            return await _tableClient.GetEntityAsync<InvoiceEntity>(scheme, invoiceId);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<bool> CreateInvoice(Invoice invoice)
    {
        var invoiceEntity = await GetInvoice(scheme: invoice.Scheme, invoiceId: invoice.Id);

        if (invoiceEntity is not null)
        {
            return false;
        }

        invoiceEntity = new InvoiceEntity
        {
            PartitionKey = invoice.Scheme,
            RowKey = invoice.Id,
            Status = invoice.Status,
            Data = JsonSerializer.Serialize(invoice)
        };

        await _tableClient.AddEntityAsync(invoiceEntity);
        return true;
    }

    public async Task<bool> UpdateInvoice(Invoice invoice)
    {
        var invoiceEntity = await GetInvoice(invoice.Scheme, invoice.Id);

        if (invoiceEntity is null)
        {
            return false;
        }

        invoiceEntity.Status = invoice.Status;
        invoiceEntity.Data = JsonSerializer.Serialize(invoice);

        await _tableClient.UpdateEntityAsync(invoiceEntity, Azure.ETag.All, TableUpdateMode.Replace);
        return true;
    }
}
