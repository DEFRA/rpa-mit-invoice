using EST.MIT.Invoice.Api.Repositories.Interfaces;

namespace EST.MIT.Invoice.Api.Repositories;

public class ReferenceDataRepository : IReferenceDataRepository
{
    private readonly IHttpClientFactory _clientFactory;

    public ReferenceDataRepository(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<HttpResponseMessage> GetSchemesListAsync(string? invoiceType, string? organisation)
    {
        var client = _clientFactory.CreateClient("ReferenceDataApi");

        var response = (string.IsNullOrEmpty(invoiceType) && string.IsNullOrEmpty(organisation))
            ? await client.GetAsync($"/schemeTypes")
            : await client.GetAsync($"/schemeTypes?invoiceType={invoiceType}&organisation={organisation}");

        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }

        return response;
    }

    public async Task<HttpResponseMessage> GetOrganisationsListAsync()
    {
        var client = _clientFactory.CreateClient("ReferenceDataApi");

        var response = await client.GetAsync($"/organisations");

        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }

        return response;
    }
}