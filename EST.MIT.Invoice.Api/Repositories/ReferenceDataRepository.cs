using EST.MIT.Invoice.Api.Repositories.Interfaces;

namespace EST.MIT.Invoice.Api.Repositories;

public class ReferenceDataRepository : IReferenceDataRepository
{
    private readonly IHttpClientFactory _clientFactory;

    public ReferenceDataRepository(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<HttpResponseMessage> GetSchemesListAsync()
    {
        var client = _clientFactory.CreateClient("ReferenceDataApi");

        var response = await client.GetAsync($"/schemeTypes");

        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }

        return response;
    }
}