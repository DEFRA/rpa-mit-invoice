using System.Diagnostics.CodeAnalysis;

namespace EST.MIT.Invoice.Api.Services.API.Models;

[ExcludeFromCodeCoverageAttribute]
public class PaymentScheme
{
    public string code { get; set; }
    public string description { get; set; }
}