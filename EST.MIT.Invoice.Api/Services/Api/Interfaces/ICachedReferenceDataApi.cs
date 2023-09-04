using EST.MIT.Invoice.Api.Services.Api.Models;

namespace EST.MIT.Invoice.Api.Services.Api.Interfaces;
public interface ICachedReferenceDataApi
{
    Task<ApiResponse<IEnumerable<ValidCombinationForRoute>>> GetCombinationsListForRouteAsync(string invoiceType, string organisation, string paymentType, string schemeType);
}
