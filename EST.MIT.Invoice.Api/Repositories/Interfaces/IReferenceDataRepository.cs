namespace EST.MIT.Invoice.Api.Repositories.Interfaces;

public interface IReferenceDataRepository
{
    Task<HttpResponseMessage> GetSchemesListAsync(string? invoiceType, string? organisation);
 
    Task<HttpResponseMessage> GetOrganisationsListAsync(string? invoiceType);
}
