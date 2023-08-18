namespace EST.MIT.Invoice.Api.Repositories.Interfaces;

public interface IReferenceDataRepository
{
    Task<HttpResponseMessage> GetSchemeTypesListAsync(string? invoiceType, string? organisation);
    Task<HttpResponseMessage> GetPaymentTypesListAsync(string? invoiceType, string? organisation, string? schemeType);
    Task<HttpResponseMessage> GetOrganisationsListAsync(string? invoiceType);
    Task<HttpResponseMessage> GetSchemeCodesListAsync(string? invoiceType, string? organisation, string? paymentType, string? schemeType);
}
