using EST.MIT.Invoice.Api.Repositories.Interfaces;

namespace EST.MIT.Invoice.Api.Repositories;

public class ReferenceDataRepository : IReferenceDataRepository
{
    private readonly IHttpClientFactory _clientFactory;

    public ReferenceDataRepository(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<HttpResponseMessage> GetSchemeTypesListAsync(string? invoiceType, string? organisation)
    {
        var client = _clientFactory.CreateClient("ReferenceDataApi.SchemeTypes");

        var response = (string.IsNullOrEmpty(invoiceType) && string.IsNullOrEmpty(organisation))
            ? await client.GetAsync($"/schemeTypes")
            : await client.GetAsync($"/schemeTypes?invoiceType={invoiceType}&organisation={organisation}");

        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }

        return response;
    }

    public async Task<HttpResponseMessage> GetPaymentTypesListAsync(string? invoiceType, string? organisation, string? schemeType)
    {
        var client = _clientFactory.CreateClient("ReferenceDataApi.PaymentTypes");

        var response = (string.IsNullOrEmpty(invoiceType) && string.IsNullOrEmpty(organisation))
            ? await client.GetAsync($"/paymentTypes")
            : await client.GetAsync($"/paymentTypes?invoiceType={invoiceType}&organisation={organisation}&schemeType={schemeType}");

        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }

        return response;
    }
}