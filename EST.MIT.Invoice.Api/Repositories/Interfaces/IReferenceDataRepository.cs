namespace EST.MIT.Invoice.Api.Repositories.Interfaces;

public interface IReferenceDataRepository
{
    Task<HttpResponseMessage> GetSchemeTypesListAsync(string? accountType, string? organisation);
    Task<HttpResponseMessage> GetPaymentTypesListAsync(string? accountType, string? organisation, string? schemeType);
    Task<HttpResponseMessage> GetOrganisationsListAsync(string? accountType);
    Task<HttpResponseMessage> GetSchemeCodesListAsync(string? accountType, string? organisation, string? paymentType, string? schemeType);
    Task<HttpResponseMessage> GetFundCodesListAsync(string? accountType, string? organisation, string? paymentType, string? schemeType);
    Task<HttpResponseMessage> GetCombinationsListForRouteAsync(string accountType, string organisation, string paymentType, string schemeType);
}
