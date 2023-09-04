using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Invoices.Api.Models;

public class InvoiceLine
{
    [JsonProperty("value")]
    [Range(0, 999999999999.99, ErrorMessage = "Value must be between 0 and 999999999999.99")]
    public decimal Value { get; set; }
    [JsonProperty("currency")]
    public string Currency { get; set; } = null!;
    [JsonProperty("schemeCode")]
    [Required(ErrorMessage = "SchemeCode must be specified")]
    public string SchemeCode { get; set; } = null!;
    [JsonProperty("description")]
    [Required(ErrorMessage = "Description must be stated")]
    public string Description { get; set; } = null!;
    [JsonProperty("fundCode")]
    public string FundCode { get; init; } = default!;
    [JsonProperty("mainAccount")]
    public string MainAccount { get; init; } = default!;
}