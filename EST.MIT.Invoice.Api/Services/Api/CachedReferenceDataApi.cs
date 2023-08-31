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
    private readonly IMemoryCache _memoryCache;

    private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    public CachedReferenceDataApi(IReferenceDataRepository referenceDataRepository, ILogger<CachedReferenceDataApi> logger, IHttpContentDeserializer httpContentDeserializer,
        IMemoryCache memoryCache)
    {
        _referenceDataRepository = referenceDataRepository;
        _logger = logger;
        _httpContentDeserializer = httpContentDeserializer;
        _memoryCache = memoryCache;
    }


    public async Task<ApiResponse<IEnumerable<RouteCombination>>> GetRouteCombinationsAsync(string invoiceType, string organisation, string paymentType, string schemeType)
    {
        var cacheKey = new { invoiceType, organisation, paymentType, schemeType };
        var apiResponse = new ApiResponse<IEnumerable<RouteCombination>>(false);

        if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<RouteCombination> routeCombinations))
        {
            apiResponse = new ApiResponse<IEnumerable<RouteCombination>>(HttpStatusCode.OK)
            {
                Data = routeCombinations
            };
        }
        else
        {
            try
            {
                await semaphore.WaitAsync();

                if (_memoryCache.TryGetValue(cacheKey, out routeCombinations))
                {
                    apiResponse = new ApiResponse<IEnumerable<RouteCombination>>(HttpStatusCode.OK)
                    {
                        Data = routeCombinations
                    };
                }
                else
                {
                    var routeCombinationsResponse = await this.GetRouteCombinationsFromApiAsync(invoiceType, organisation, paymentType, schemeType);

                    if (routeCombinationsResponse.IsSuccess)
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                            .SetAbsoluteExpiration(TimeSpan.FromMinutes(60))
                            .SetPriority(CacheItemPriority.Normal);

                        _memoryCache.Set(cacheKey, routeCombinationsResponse.Data, cacheEntryOptions);

                        apiResponse = routeCombinationsResponse;
                    }
                    else
                    {
                        apiResponse = routeCombinationsResponse;
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

    private async Task<ApiResponse<IEnumerable<RouteCombination>>> GetRouteCombinationsFromApiAsync(string invoiceType, string organisation, string paymentType, string schemeType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetRouteCombinationsListAsync(invoiceType, organisation, paymentType, schemeType);

        _logger.LogInformation($"Calling Reference Data API for Route Combinations");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<RouteCombination>>(HttpStatusCode.NoContent);
            }

            try
            {
                var responseDataTask = _httpContentDeserializer.DeserializeListAsync<RouteCombination>(response.Content);

                var message = responseDataTask.Exception?.Message;

                if (responseDataTask.IsFaulted)
                {
                    _logger.LogError("Error message is ", message);
                    throw responseDataTask.Exception?.InnerException ?? new Exception("An error occurred while processing the response.");
                }

                await responseDataTask;
                var routeCombinations = responseDataTask.Result.ToList();

                if (routeCombinations.Any())
                {
                    return new ApiResponse<IEnumerable<RouteCombination>>(HttpStatusCode.OK)
                    {
                        Data = routeCombinations
                    };
                }

                _logger.LogInformation("No content returned from API");
                return new ApiResponse<IEnumerable<RouteCombination>>(HttpStatusCode.NotFound);

            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<RouteCombination>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<RouteCombination>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<RouteCombination>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<RouteCombination>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<RouteCombination>>(HttpStatusCode.InternalServerError, error);
    }
}
