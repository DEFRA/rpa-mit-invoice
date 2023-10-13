using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EST.MIT.Invoice.Api.Models;

public class InvoiceLine
{
    public Guid Id { get; set; }

    [JsonProperty("value")]
    [Range(0, 999999999999.99, ErrorMessage = "Value must be between 0 and 999999999999.99")]
    public decimal Value { get; set; }

    [JsonProperty("fundCode")]
    public string FundCode { get; init; } = default!;

    [JsonProperty("mainAccount")]
    public string MainAccount { get; init; } = default!;

    [JsonProperty("schemeCode")]
    [Required(ErrorMessage = "SchemeCode must be specified")]
    public string SchemeCode { get; init; } = default!;

    [JsonProperty("marketingYear")]
    [Range(2021, 2099, ErrorMessage = "Marketing Year must be between 2021 and 2099")]
    public int MarketingYear { get; init; }

    [JsonProperty("deliveryBody")]
    public string DeliveryBody { get; init; } = default!;

    [JsonProperty("description")]
    [Required(ErrorMessage = "Description must be stated")]
    public string Description { get; init; } = default!;

    [JsonProperty("currency")]
    public string Currency { get; init; } = default!;
}