using System.Diagnostics.CodeAnalysis;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using System.Net;
using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Util;
using Microsoft.Extensions.Caching.Memory;


namespace EST.MIT.Invoice.Api.Services.Api;

public class CachedReferenceDataApi : ICachedReferenceDataApi
{
    private readonly IReferenceDataRepository _referenceDataRepository;
    private readonly ILogger<CachedReferenceDataApi> _logger;
    private readonly IHttpContentDeserializer _httpContentDeserializer;
    private readonly ICacheService _cacheService;

    private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    public CachedReferenceDataApi(IReferenceDataRepository referenceDataRepository, ILogger<CachedReferenceDataApi> logger, IHttpContentDeserializer httpContentDeserializer,
        ICacheService cacheService)
    {
        _referenceDataRepository = referenceDataRepository;
        _logger = logger;
        _httpContentDeserializer = httpContentDeserializer;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<IEnumerable<CombinationForRoute>>> GetCombinationsListForRouteAsync(string invoiceType, string organisation, string paymentType, string schemeType)
    {
        var cacheKey = new { invoiceType, organisation, paymentType, schemeType };
        var apiResponse = new ApiResponse<IEnumerable<CombinationForRoute>>(false);

        var cachedCombinationsForRoute = _cacheService.GetData<IEnumerable<CombinationForRoute>>(cacheKey);

        if (cachedCombinationsForRoute != null)
        {
            var combinationsForRoute = cachedCombinationsForRoute.ToList();
            apiResponse = new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.OK)
            {
                Data = combinationsForRoute
            };
        }
        else
        {
            try
            {
                await semaphore.WaitAsync();

                cachedCombinationsForRoute = _cacheService.GetData<IEnumerable<CombinationForRoute>>(cacheKey);
                if (cachedCombinationsForRoute != null)
                {
                    var combinationsForRoute = cachedCombinationsForRoute.ToList();
                    apiResponse = new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.OK)
                    {
                        Data = combinationsForRoute
                    };
                }
                else
                {
                    var combinationsForRouteResponse = await this.GetCombinationsListForRouteFromApiAsync(invoiceType, organisation, paymentType, schemeType);

                    if (combinationsForRouteResponse.IsSuccess)
                    {
                        _cacheService.SetData(cacheKey, combinationsForRouteResponse.Data);

                        apiResponse = combinationsForRouteResponse;
                    }
                    else
                    {
                        apiResponse = combinationsForRouteResponse;
                    }
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        return apiResponse;
    }

    private async Task<ApiResponse<IEnumerable<CombinationForRoute>>> GetCombinationsListForRouteFromApiAsync(string invoiceType, string organisation, string paymentType, string schemeType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetCombinationsListForRouteAsync(invoiceType, organisation, paymentType, schemeType);

        _logger.LogInformation($"Calling Reference Data API for Route Combinations");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.NoContent);
            }

            try
            {
                var responseDataTask = _httpContentDeserializer.DeserializeListAsync<CombinationForRoute>(response.Content);

                var message = responseDataTask.Exception?.Message;

                if (responseDataTask.IsFaulted)
                {
                    _logger.LogError("Error message is ", message);
                    throw responseDataTask.Exception?.InnerException ?? new Exception("An error occurred while processing the response.");
                }

                await responseDataTask;
                var combinationsForRoute = responseDataTask.Result.ToList();

                if (combinationsForRoute.Any())
                {
                    return new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.OK)
                    {
                        Data = combinationsForRoute
                    };
                }

                _logger.LogInformation("No content returned from API");
                return new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.NotFound);

            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<CombinationForRoute>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.InternalServerError, error);
    }
}
