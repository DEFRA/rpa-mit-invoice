using Newtonsoft.Json;

namespace EST.MIT.Invoice.Api.Models;

public class BulkInvoices
{
    [JsonProperty("invoices")]
    public IEnumerable<PaymentRequestsBatch> Invoices { get; set; } = default!;
    [JsonProperty("reference")]
    public string Reference { get; set; } = default!;
    [JsonProperty("schemeType")]
    public string SchemeType { get; set; } = default!;
}