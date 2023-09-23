using EST.MIT.Invoice.Api.Repositories.Interfaces;

namespace EST.MIT.Invoice.Api.Repositories;

public class ReferenceDataRepository : IReferenceDataRepository
{
    private readonly IHttpClientFactory _clientFactory;

    public ReferenceDataRepository(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<HttpResponseMessage> GetSchemeTypesListAsync(string? accountType, string? organisation)
    {
        var client = _clientFactory.CreateClient("ReferenceDataApi.SchemeTypes");

        var response = (string.IsNullOrEmpty(accountType) && string.IsNullOrEmpty(organisation))
            ? await client.GetAsync($"/schemeTypes")
            : await client.GetAsync($"/schemeTypes?invoiceType={accountType}&organisation={organisation}");

        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }

        return response;
    }

    public async Task<HttpResponseMessage> GetPaymentTypesListAsync(string? accountType, string? organisation, string? schemeType)
    {
        var client = _clientFactory.CreateClient("ReferenceDataApi.PaymentTypes");

        var response = (string.IsNullOrEmpty(accountType) && string.IsNullOrEmpty(organisation))
            ? await client.GetAsync($"/paymentTypes")
            : await client.GetAsync($"/paymentTypes?invoiceType={accountType}&organisation={organisation}&schemeType={schemeType}");

        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }

        return response;
    }

    public async Task<HttpResponseMessage> GetOrganisationsListAsync(string? accountType)
    {
        var client = _clientFactory.CreateClient("ReferenceDataApi.Organisations");

        var response = (string.IsNullOrEmpty(accountType))
           ? await client.GetAsync($"/organisations")
            : await client.GetAsync($"/organisations?invoiceType={accountType}");

        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }

        return response;
    }

    public async Task<HttpResponseMessage> GetSchemeCodesListAsync(string? accountType, string? organisation, string? paymentType, string? schemeType)
    {
        var client = _clientFactory.CreateClient("ReferenceDataApi");

        var response = (string.IsNullOrEmpty(accountType) && string.IsNullOrEmpty(organisation) && string.IsNullOrEmpty(paymentType) && string.IsNullOrEmpty(schemeType))
            ? await client.GetAsync($"/schemes")
            : await client.GetAsync($"/schemes/{accountType}/{organisation}/{schemeType}/{paymentType}");

        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }

        return response;
    }

    public async Task<HttpResponseMessage> GetFundCodesListAsync(string? accountType, string? organisation, string? paymentType, string? schemeType)
    {
        var client = _clientFactory.CreateClient("ReferenceApi.FundCodes");

        var response = (string.IsNullOrEmpty(accountType) && string.IsNullOrEmpty(organisation) && string.IsNullOrEmpty(paymentType) && string.IsNullOrEmpty(schemeType))
            ? await client.GetAsync($"/funds")
            : await client.GetAsync($"/funds?invoiceType={accountType}&organisation={organisation}&paymentType={paymentType}&schemeType={schemeType}");

        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }

        return response;
    }

    public async Task<HttpResponseMessage> GetCombinationsListForRouteAsync(string accountType, string organisation, string paymentType, string schemeType)
    {
        var client = _clientFactory.CreateClient("ReferenceApi.Combinations");

        var response = await client.GetAsync($"/combinations?invoiceType={accountType}&organisation={organisation}&paymentType={paymentType}&schemeType={schemeType}");

        if (!response.IsSuccessStatusCode)
        {
            response.Content = new StringContent(await response.Content.ReadAsStringAsync());
        }

        return response;
    }
}