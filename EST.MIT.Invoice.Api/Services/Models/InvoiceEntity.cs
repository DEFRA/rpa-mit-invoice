using Azure;
using Azure.Data.Tables;

namespace Invoices.Api.Services.Models;

public class InvoiceEntity : ITableEntity
{
    public string PartitionKey { get; set; } = default!;
    public string RowKey { get; set; } = default!;
    public string Data { get; set; } = default!;

    public string Status { get; set; } = default!;

    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}