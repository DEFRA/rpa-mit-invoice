using EST.MIT.Invoice.Api.Services.Api.Models;

namespace EST.MIT.Invoice.Api.Services.Api.Interfaces;
public interface ICachedReferenceDataApi
{
    Task<ApiResponse<IEnumerable<RouteCombination>>> GetRouteCombinationsAsync(string invoiceType, string organisation, string paymentType, string schemeType);
}
