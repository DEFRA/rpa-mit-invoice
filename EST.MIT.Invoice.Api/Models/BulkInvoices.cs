using Newtonsoft.Json;

namespace Invoices.Api.Models;

public class BulkInvoices
{
    [JsonProperty("invoices")]
    public IEnumerable<Invoice> Invoices { get; set; } = default!;
    [JsonProperty("reference")]
    public string Reference { get; set; } = default!;
    [JsonProperty("schemeType")]
    public string SchemeType { get; set; } = default!;
}