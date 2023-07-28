using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.API.Models;

namespace EST.MIT.Invoice.Api.Services.API.Interfaces;


public interface IReferenceDataApi
{
    Task<ApiResponse<IEnumerable<PaymentScheme>>> GetSchemesAsync();
    Task<ApiResponse<IEnumerable<Organisation>>> GetOrganisationsAsync();
}
