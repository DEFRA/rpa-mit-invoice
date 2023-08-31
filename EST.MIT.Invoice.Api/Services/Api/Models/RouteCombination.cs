using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace EST.MIT.Invoice.Api.Services.Api.Models;

[ExcludeFromCodeCoverageAttribute]
public class RouteCombination
{
    /// <summary>
    /// Account code component of valid combination
    /// </summary>
    public string AccountCode { get; init; } = default!;

    /// <summary>
    /// Scheme code component of valid combination
    /// </summary>
    public string SchemeCode { get; init; } = default!;

    /// <summary>
    /// Delivery body code component of valid combination
    /// </summary>
    public string DeliveryBodyCode { get; init; } = default!;

    /// <summary>
    /// Creates instance of RouteCombination
    /// </summary>
    /// <param name="accountCode"></param>
    /// <param name="schemeCode"></param>
    /// <param name="deliveryBodyCode"></param>
    public RouteCombination(string accountCode, string schemeCode, string deliveryBodyCode)
    {
        AccountCode = accountCode;
        SchemeCode = schemeCode;
        DeliveryBodyCode = deliveryBodyCode;
    }

    /// <summary>
    /// Creates instance of RouteCombination
    /// </summary>
    public RouteCombination()
    {
    }
}