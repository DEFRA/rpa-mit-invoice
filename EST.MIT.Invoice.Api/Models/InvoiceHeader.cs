using System.Text.Json.Serialization;

namespace Invoices.Api.Models;

public class InvoiceHeader
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;
    [JsonPropertyName("claimReference")]
    public string ClaimReference { get; init; } = default!;
    [JsonPropertyName("claimReferenceNumber")]
    public string ClaimReferenceNumber { get; init; } = default!;
    [JsonPropertyName("frn")]
    public string FRN { get; init; } = default!;
    [JsonPropertyName("agreementNumber")]
    public string AgreementNumber { get; init; } = default!;
    [JsonPropertyName("currency")]
    public string Currency { get; init; } = default!;
    [JsonPropertyName("description")]
    public string Description { get; init; } = default!;
}