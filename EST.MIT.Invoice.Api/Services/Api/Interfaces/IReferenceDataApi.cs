
using EST.MIT.Invoice.Api.Services.API.Models;

namespace EST.MIT.Invoice.Api.Services.API.Interfaces;


public interface IReferenceDataApi
{
    Task<ApiResponse<IEnumerable<PaymentScheme>>> GetSchemeTypesAsync(string? invoiceType, string? organisation);

    Task<ApiResponse<IEnumerable<PaymentType>>> GetPaymentTypesAsync(string? invoiceType, string? organisation, string? schemeType);
    
    Task<ApiResponse<IEnumerable<Organisation>>> GetOrganisationsAsync(string? invoiceType);
}
