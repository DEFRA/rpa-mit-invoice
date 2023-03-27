using Newtonsoft.Json;

namespace Invoices.Api.Models;

public class Invoice
{
    [JsonProperty("id")]
    public string Id { get; init; } = default!;
    [JsonProperty("invoiceType")]
    public string InvoiceType { get; init; } = default!;
    [JsonProperty("accountType")]
    public string AccountType { get; init; } = default!;
    [JsonProperty("organisation")]
    public string Organisation { get; init; } = default!;
    [JsonProperty("schemeType")]
    public string SchemeType { get; init; } = default!;
    [JsonProperty("headers")]
    public List<InvoiceHeader> Headers { get; init; } = default!;
    [JsonProperty("status")]
    public string Status { get; init; } = default!;
    [JsonProperty("reference")]
    public string Reference { get; init; } = default!;
    [JsonProperty("created")]
    public DateTime Created { get; init; } = DateTime.UtcNow;
    [JsonProperty("updated")]
    public DateTime? Updated { get; init; }
    [JsonProperty("createdBy")]
    public string CreatedBy { get; init; } = default!;
    [JsonProperty("updatedBy")]
    public string UpdatedBy { get; init; } = default!;
}