using Newtonsoft.Json;

namespace EST.MIT.Invoice.Api.Services.Api.Models
{
    public class SchemeCode
    {
        [JsonProperty("code")]
        public string Code { get; set; } = default!;
        [JsonProperty("description")]
        public string Description { get; set; } = default!;
    }
}
