using Newtonsoft.Json;

namespace EST.MIT.Invoice.Api.Models;
public class AppendixReferences
{
    [JsonProperty("claimReferenceNumber")]
    public string ClaimReferenceNumber { get; init; } = default!;
}