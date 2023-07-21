using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace EST.MIT.Invoice.Api.Services.API.Models;

[ExcludeFromCodeCoverageAttribute]
public class PaymentScheme
{
    [JsonProperty("code")]
    public string Code { get; set; } = default!;
    
    [JsonProperty("description")]
    public string Description { get; set; } = default!;
}