using EST.MIT.Invoice.Api.Services.API.Models;

namespace EST.MIT.Invoice.Api.Services.API.Interfaces;


public interface IReferenceDataApi
{
    Task<ApiResponse<IEnumerable<PaymentScheme>>> GetSchemesAsync();
    Task<ApiResponse<IEnumerable<Invoices.Api.Models.Invoice>>> GetOrganisationsAsync();
}
