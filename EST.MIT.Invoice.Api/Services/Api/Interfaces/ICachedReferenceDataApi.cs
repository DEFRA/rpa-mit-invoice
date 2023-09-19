using EST.MIT.Invoice.Api.Services.Api.Models;

namespace EST.MIT.Invoice.Api.Services.Api.Interfaces;
public interface ICachedReferenceDataApi
{
    Task<ApiResponse<IEnumerable<CombinationForRoute>>> GetCombinationsListForRouteAsync(string accountType, string organisation, string paymentType, string schemeType);
}
