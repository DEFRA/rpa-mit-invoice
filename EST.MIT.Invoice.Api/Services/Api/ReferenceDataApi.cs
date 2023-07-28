using EST.MIT.Invoice.Api.Services.API.Interfaces;
using EST.MIT.Invoice.Api.Services.API.Models;
using System.Net;
using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Services.Api.Models;

namespace EST.MIT.Invoice.Api.Services.Api;

public class ReferenceDataApi : IReferenceDataApi
{
    private readonly IReferenceDataRepository _referenceDataRepository;
    private readonly ILogger<ReferenceDataApi> _logger;

    public ReferenceDataApi(IReferenceDataRepository referenceDataRepository, ILogger<ReferenceDataApi> logger)
    {
        _referenceDataRepository = referenceDataRepository;
        _logger = logger;
    }


    public async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetSchemesAsync()
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetSchemesListAsync();

        _logger.LogInformation($"Calling Reference Data API for Schemes");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NoContent);
            }

            try
            {
                var responseDataTask = response.Content.ReadFromJsonAsync<IEnumerable<PaymentScheme>>();
                await responseDataTask;

                if (responseDataTask.IsFaulted)
                {
                    _logger.LogError(responseDataTask.Exception?.Message);
                    throw responseDataTask.Exception?.InnerException ?? new Exception("An error occurred while processing the response.");
                }

                var responseData = responseDataTask.Result;
                if (responseData != null)
                {
                    return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK)
                    {
                        Data = responseData
                    };
                }

                _logger.LogInformation("No content returned from API");
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NotFound);

            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<PaymentScheme>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.InternalServerError, error);
    }

    public async Task<ApiResponse<IEnumerable<Organisation>>> GetOrganisationsAsync()
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetOrganisationsListAsync();

        _logger.LogInformation($"Calling Reference Data API for Organisations");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.NoContent);
            }
            try
            {
                var responseDataTask = response.Content.ReadFromJsonAsync<IEnumerable<Organisation>>();
                await responseDataTask;

                if (responseDataTask.IsFaulted)
                {
                    _logger.LogError(responseDataTask.Exception?.Message);
                    throw responseDataTask.Exception?.InnerException ?? new Exception("An error occurred while processing the response.");
                }

                var responseData = responseDataTask.Result;
                if (responseData != null)
                {
                    return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK)
                    {
                        Data = responseData
                    };
                }

                _logger.LogInformation("No content returned from API");
                return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.NotFound);

            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<Organisation>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.InternalServerError, error);
    }
}
