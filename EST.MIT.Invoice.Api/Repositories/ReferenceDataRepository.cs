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

    public async Task<HttpResponseMessage> GetOrganisationsListAsync(string? invoiceType)
    {
        var client = _clientFactory.CreateClient("ReferenceDataApi.Organisations");

        var response = (string.IsNullOrEmpty(invoiceType))
           ? await client.GetAsync($"/organisations")
            : await client.GetAsync($"/organisations?invoiceType={invoiceType}");

        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }

        return response;
    }

    public async Task<HttpResponseMessage> GetSchemeCodesListAsync(string? invoiceType, string? organisation, string? paymentType, string? schemeType)
    {
        var client = _clientFactory.CreateClient("ReferenceApi.SchemeCodes");

        var response = (string.IsNullOrEmpty(invoiceType) && string.IsNullOrEmpty(organisation) && string.IsNullOrEmpty(paymentType) && string.IsNullOrEmpty(schemeType))
            ? await client.GetAsync($"/schemeCodes")
            : await client.GetAsync($"/schemeCodes?invoiceType={invoiceType}&organisation={organisation}&paymentType={paymentType}&schemeType={schemeType}");

        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }

        return response;
    }

    public async Task<HttpResponseMessage> GetFundCodesListAsync(string? invoiceType, string? organisation, string? paymentType, string? schemeType)
    {
        var client = _clientFactory.CreateClient("ReferenceApi.FundCodes");

        var response = (string.IsNullOrEmpty(invoiceType) && string.IsNullOrEmpty(organisation) && string.IsNullOrEmpty(paymentType) && string.IsNullOrEmpty(schemeType))
            ? await client.GetAsync($"/fundCodes")
            : await client.GetAsync($"/fundCodes?invoiceType={invoiceType}&organisation={organisation}&paymentType={paymentType}&schemeType={schemeType}");

        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }

        return response;
    }

    public async Task<HttpResponseMessage> GetCombinationsListForRouteAsync(string invoiceType, string organisation, string paymentType, string schemeType)
    {
        var client = _clientFactory.CreateClient("ReferenceApi.Combinations");

        var response = await client.GetAsync($"/combinations?invoiceType={invoiceType}&organisation={organisation}&paymentType={paymentType}&schemeType={schemeType}");

        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }

        return response;
    }
}