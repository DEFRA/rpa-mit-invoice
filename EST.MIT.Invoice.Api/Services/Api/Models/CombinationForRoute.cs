using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace EST.MIT.Invoice.Api.Services.Api.Models;

[ExcludeFromCodeCoverageAttribute]
public class CombinationForRoute
{
    /// <summary>
    /// Account code component of combination
    /// </summary>
    public string AccountCode { get; init; } = default!;

    /// <summary>
    /// Scheme code component of combination
    /// </summary>
    public string SchemeCode { get; init; } = default!;

    /// <summary>
    /// Delivery body code component of combination
    /// </summary>
    public string DeliveryBodyCode { get; init; } = default!;

    /// <summary>
    /// Creates instance of CombinationForRoute
    /// </summary>
    /// <param name="accountCode"></param>
    /// <param name="schemeCode"></param>
    /// <param name="deliveryBodyCode"></param>
    public CombinationForRoute(string accountCode, string schemeCode, string deliveryBodyCode)
    {
        AccountCode = accountCode;
        SchemeCode = schemeCode;
        DeliveryBodyCode = deliveryBodyCode;
    }

    /// <summary>
    /// Creates instance of CombinationForRoute
    /// </summary>
    public CombinationForRoute()
    {
    }
}