using EST.MIT.Invoice.Api.Services.Api.Models;

namespace EST.MIT.Invoice.Api.Services.Api.Interfaces;
public interface ICachedReferenceDataApi
{
    Task<ApiResponse<IEnumerable<CombinationForRoute>>> GetCombinationsListForRouteAsync(string accountType, string organisation, string paymentType, string schemeType);
    Task<ApiResponse<IEnumerable<DeliveryBodyCode>>> GetDeliveryBodyCodesForRouteAsync(string accountType, string organisation, string paymentType, string schemeType);
    Task<ApiResponse<IEnumerable<FundCode>>> GetFundCodesForRouteAsync(string accountType, string organisation, string paymentType, string schemeType);
    Task<ApiResponse<IEnumerable<MainAccountCode>>> GetMainAccountCodesForRouteAsync(string accountType, string organisation, string paymentType, string schemeType);
    Task<ApiResponse<IEnumerable<SchemeCode>>> GetSchemeCodesForRouteAsync(string accountType, string organisation, string paymentType, string schemeType);
    Task<ApiResponse<IEnumerable<MarketingYear>>> GetMarketingYearsForRouteAsync(string? accountType, string? organisation, string? paymentType, string? schemeType);
}

