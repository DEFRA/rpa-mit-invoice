using Newtonsoft.Json;

namespace Invoices.Api.Models;

public class InvoiceLine
{
    [JsonProperty("value")]
    public decimal Value { get; set; }
    [JsonProperty("currency")]
    public string Currency { get; set; } = null!;
    [JsonProperty("schemeCode")]
    public string SchemeCode { get; set; } = null!;
    [JsonProperty("description")]
    public string Description { get; set; } = null!;
    [JsonProperty("fundCode")]
    public string FundCode { get; set; } = null!;
}