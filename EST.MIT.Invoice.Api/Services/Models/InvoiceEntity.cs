using Invoices.Api.Models;
using Newtonsoft.Json;

namespace Invoices.Api.Services.Models;

public class InvoiceEntity
{
    [JsonProperty("schemeType")]
    public string SchemeType { get; set; } = default!;
    [JsonProperty("id")]
    public string Id { get; set; } = default!;
    [JsonProperty("data")]
    public string Data { get; set; } = default!;
    [JsonProperty("reference")]
    public string Reference { get; set; } = default!;
    [JsonProperty("value")]
    public decimal Value { get; set; } = default!;
    [JsonProperty("status")]
    public string Status { get; set; } = default!;
    [JsonProperty("createdBy")]
    public string CreatedBy { get; set; } = default!;
    [JsonProperty("updatedBy")]
    public string UpdatedBy { get; set; } = default!;
    [JsonProperty("created")]
    public string Created { get; set; } = default!;
    [JsonProperty("updated")]
    public string Updated { get; set; } = default!;
}