using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace EST.MIT.Invoice.Api.Services.Api.Models;

[ExcludeFromCodeCoverageAttribute]
public abstract class ReferenceDataBaseApiModel
{
    [JsonProperty("code")]
    public string Code { get; set; } = default!;
    [JsonProperty("description")]
    public string Description { get; set; } = default!;
}