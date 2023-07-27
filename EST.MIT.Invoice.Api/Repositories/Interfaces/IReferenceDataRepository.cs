namespace EST.MIT.Invoice.Api.Repositories.Interfaces;

public interface IReferenceDataRepository
{
    Task<HttpResponseMessage> GetSchemesListAsync();
    Task<HttpResponseMessage> GetOrganisationsListAsync();
}
