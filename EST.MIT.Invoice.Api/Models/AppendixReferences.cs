using Newtonsoft.Json;

namespace Invoices.Api.Models;
public class AppendixReferences
{
    [JsonProperty("claimReferenceNumber")]
    public string ClaimReferenceNumber { get; init; } = default!;
}