using System.Text.Json.Serialization;

namespace Invoices.Api.Models;

public class InvoiceGenerator
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;
    [JsonPropertyName("scheme")]
    public string Scheme { get; set; } = default!;
}