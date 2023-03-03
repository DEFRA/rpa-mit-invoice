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
}