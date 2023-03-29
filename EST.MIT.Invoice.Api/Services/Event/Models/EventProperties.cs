using System.Text.Json.Serialization;

namespace Invoices.Api.Services.Models;

public class EventProperties
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;
    [JsonPropertyName("checkpoint")]
    public string Checkpoint { get; init; } = default!;
    [JsonPropertyName("status")]
    public string Status { get; init; } = default!;
    [JsonPropertyName("action")]
    public EventAction Action { get; init; } = default!;
}