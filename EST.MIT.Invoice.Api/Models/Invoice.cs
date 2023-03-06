using System.Text.Json.Serialization;

namespace Invoices.Api.Models;

public class Invoice
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;

    [JsonPropertyName("scheme")]
    public string Scheme { get; init; } = default!;

    [JsonPropertyName("status")]
    public string Status { get; init; } = default!;

    [JsonPropertyName("createdBy")]
    public string CreatedBy { get; init; } = default!;

    [JsonPropertyName("updatedBy")]
    public string UpdatedBy { get; init; } = default!;

    [JsonPropertyName("header")]
    public InvoiceHeader Header { get; init; } = default!;
}