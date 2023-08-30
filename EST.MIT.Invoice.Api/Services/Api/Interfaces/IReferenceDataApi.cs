using EST.MIT.Invoice.Api.Services.Api.Models;


namespace EST.MIT.Invoice.Api.Services.Api.Interfaces;
public interface IReferenceDataApi
{
    Task<ApiResponse<IEnumerable<PaymentScheme>>> GetSchemeTypesAsync(string? invoiceType, string? organisation);
    Task<ApiResponse<IEnumerable<PaymentType>>> GetPaymentTypesAsync(string? invoiceType, string? organisation, string? schemeType);
    Task<ApiResponse<IEnumerable<Organisation>>> GetOrganisationsAsync(string? invoiceType);
    Task<ApiResponse<IEnumerable<SchemeCode>>> GetSchemeCodesAsync(string? invoiceType, string? organisation, string? paymentType, string? schemeType);
    Task<ApiResponse<IEnumerable<DeliveryBodyCode>>> GetDeliveryBodyCodesAsync(string? invoiceType, string? organisation, string? paymentType, string? schemeType);
    Task<ApiResponse<IEnumerable<FundCode>>> GetFundCodesAsync(string? invoiceType, string? organisation, string? paymentType, string? schemeType);
    Task<ApiResponse<IEnumerable<MainAccount>>> GetMainAccountsAsync(string? invoiceType, string? organisation, string? paymentType, string? schemeType);
}
