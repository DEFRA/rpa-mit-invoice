using EST.MIT.Invoice.Api.Services.Api.Models;


namespace EST.MIT.Invoice.Api.Services.Api.Interfaces;
public interface IReferenceDataApi
{
    Task<ApiResponse<IEnumerable<PaymentScheme>>> GetSchemeTypesAsync(string? accountType, string? organisation);
    Task<ApiResponse<IEnumerable<PaymentType>>> GetPaymentTypesAsync(string? accountType, string? organisation, string? schemeType);
    Task<ApiResponse<IEnumerable<Organisation>>> GetOrganisationsAsync(string? accountType);
}
