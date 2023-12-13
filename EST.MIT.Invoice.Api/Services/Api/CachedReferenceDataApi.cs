using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using System.Net;
using EST.MIT.Invoice.Api.Constants;
using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Util;


namespace EST.MIT.Invoice.Api.Services.Api;

public class CachedReferenceDataApi : ICachedReferenceDataApi
{
    private readonly IReferenceDataRepository _referenceDataRepository;
    private readonly ILogger<CachedReferenceDataApi> _logger;
    private readonly IHttpContentDeserializer _httpContentDeserializer;
    private readonly ICacheService _cacheService;

    private static readonly SemaphoreSlim combinationsSemaphore = new SemaphoreSlim(1, 1);
    private static readonly SemaphoreSlim deliveryBodyCodesSemaphore = new SemaphoreSlim(1, 1);
    private static readonly SemaphoreSlim fundCodesSemaphore = new SemaphoreSlim(1, 1);
    private static readonly SemaphoreSlim mainAccountCodesSemaphore = new SemaphoreSlim(1, 1);
    private static readonly SemaphoreSlim schemeCodesSemaphore = new SemaphoreSlim(1, 1);
    private static readonly SemaphoreSlim marketingYearsSemaphore = new SemaphoreSlim(1, 1);

    public CachedReferenceDataApi(IReferenceDataRepository referenceDataRepository, ILogger<CachedReferenceDataApi> logger, IHttpContentDeserializer httpContentDeserializer,
        ICacheService cacheService)
    {
        _referenceDataRepository = referenceDataRepository;
        _logger = logger;
        _httpContentDeserializer = httpContentDeserializer;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<IEnumerable<CombinationForRoute>>> GetCombinationsListForRouteAsync(string accountType, string organisation, string paymentType, string schemeType)
    {
        var cacheKey = new { RefDataCacheKeyPrefixes.Combinations, accountType, organisation, paymentType, schemeType };
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
                await combinationsSemaphore.WaitAsync();

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
                    var combinationsForRouteResponse = await this.GetCombinationsListForRouteFromApiAsync(accountType, organisation, paymentType, schemeType);

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
                combinationsSemaphore.Release();
            }
        }

        return apiResponse;
    }

    private async Task<ApiResponse<IEnumerable<CombinationForRoute>>> GetCombinationsListForRouteFromApiAsync(string accountType, string organisation, string paymentType, string schemeType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetCombinationsListForRouteAsync(accountType, organisation, paymentType, schemeType);

        _logger.LogInformation($"Calling Reference Data API for Route Combinations");

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.OK)
            {
                Data = new List<CombinationForRoute>()
            };
        } 
        else if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.OK)
                {
                    Data = new List<CombinationForRoute>()
                };
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
                else
                {
                    _logger.LogWarning("No content returned from API");
                    return new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.OK)
                    {
                        Data = new List<CombinationForRoute>()
                    };
                }
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

    public async Task<ApiResponse<IEnumerable<DeliveryBodyCode>>> GetDeliveryBodyCodesForRouteAsync(string accountType, string organisation, string paymentType, string schemeType)
    {
        var cacheKey = new { RefDataCacheKeyPrefixes.DeliveryBodyCodes, accountType, organisation, paymentType, schemeType };
        var apiResponse = new ApiResponse<IEnumerable<DeliveryBodyCode>>(false);

        var cachedDataForRoute = _cacheService.GetData<IEnumerable<DeliveryBodyCode>>(cacheKey);

        if (cachedDataForRoute != null)
        {
            var dataForRoute = cachedDataForRoute.ToList();
            apiResponse = new ApiResponse<IEnumerable<DeliveryBodyCode>>(HttpStatusCode.OK)
            {
                Data = dataForRoute
            };
        }
        else
        {
            try
            {
                await deliveryBodyCodesSemaphore.WaitAsync();

                cachedDataForRoute = _cacheService.GetData<IEnumerable<DeliveryBodyCode>>(cacheKey);
                if (cachedDataForRoute != null)
                {
                    var dataForRoute = cachedDataForRoute.ToList();
                    apiResponse = new ApiResponse<IEnumerable<DeliveryBodyCode>>(HttpStatusCode.OK)
                    {
                        Data = dataForRoute
                    };
                }
                else
                {
                    var dataForRouteResponse = await this.GetDeliveryBodyCodesForRouteFromApiAsync(accountType, organisation, paymentType, schemeType);

                    if (dataForRouteResponse.IsSuccess)
                    {
                        _cacheService.SetData(cacheKey, dataForRouteResponse.Data);

                        apiResponse = dataForRouteResponse;
                    }
                    else
                    {
                        apiResponse = dataForRouteResponse;
                    }
                }
            }
            finally
            {
                deliveryBodyCodesSemaphore.Release();
            }
        }

        return apiResponse;
    }

    private async Task<ApiResponse<IEnumerable<DeliveryBodyCode>>> GetDeliveryBodyCodesForRouteFromApiAsync(string accountType, string organisation, string paymentType, string schemeType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetDeliveryBodyCodesListAsync(accountType, organisation, paymentType, schemeType);

        _logger.LogInformation($"Calling Reference Data API for Delivery Body Codes");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<DeliveryBodyCode>>(HttpStatusCode.NoContent);
            }

            try
            {
                var responseDataTask = _httpContentDeserializer.DeserializeListAsync<DeliveryBodyCode>(response.Content);

                var message = responseDataTask.Exception?.Message;

                if (responseDataTask.IsFaulted)
                {
                    _logger.LogError("Error message is ", message);
                    throw responseDataTask.Exception?.InnerException ?? new Exception("An error occurred while processing the response.");
                }

                await responseDataTask;
                var dataForRoute = responseDataTask.Result.ToList();

                if (dataForRoute.Any())
                {
                    return new ApiResponse<IEnumerable<DeliveryBodyCode>>(HttpStatusCode.OK)
                    {
                        Data = dataForRoute
                    };
                }

                _logger.LogInformation("No content returned from API");
                return new ApiResponse<IEnumerable<DeliveryBodyCode>>(HttpStatusCode.NotFound);

            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<DeliveryBodyCode>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<DeliveryBodyCode>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<DeliveryBodyCode>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<DeliveryBodyCode>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<DeliveryBodyCode>>(HttpStatusCode.InternalServerError, error);
    }

    public async Task<ApiResponse<IEnumerable<FundCode>>> GetFundCodesForRouteAsync(string accountType, string organisation, string paymentType, string schemeType)
    {
        var cacheKey = new { RefDataCacheKeyPrefixes.FundCodes, accountType, organisation, paymentType, schemeType };
        var apiResponse = new ApiResponse<IEnumerable<FundCode>>(false);

        var cachedDataForRoute = _cacheService.GetData<IEnumerable<FundCode>>(cacheKey);

        if (cachedDataForRoute != null)
        {
            var dataForRoute = cachedDataForRoute.ToList();
            apiResponse = new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.OK)
            {
                Data = dataForRoute
            };
        }
        else
        {
            try
            {
                await fundCodesSemaphore.WaitAsync();

                cachedDataForRoute = _cacheService.GetData<IEnumerable<FundCode>>(cacheKey);
                if (cachedDataForRoute != null)
                {
                    var dataForRoute = cachedDataForRoute.ToList();
                    apiResponse = new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.OK)
                    {
                        Data = dataForRoute
                    };
                }
                else
                {
                    var dataForRouteResponse = await this.GetFundCodesForRouteFromApiAsync(accountType, organisation, paymentType, schemeType);

                    if (dataForRouteResponse.IsSuccess)
                    {
                        _cacheService.SetData(cacheKey, dataForRouteResponse.Data);

                        apiResponse = dataForRouteResponse;
                    }
                    else
                    {
                        apiResponse = dataForRouteResponse;
                    }
                }
            }
            finally
            {
                fundCodesSemaphore.Release();
            }
        }

        return apiResponse;
    }

    private async Task<ApiResponse<IEnumerable<FundCode>>> GetFundCodesForRouteFromApiAsync(string accountType, string organisation, string paymentType, string schemeType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetFundCodesListAsync(accountType, organisation, paymentType, schemeType);

        _logger.LogInformation($"Calling Reference Data API for FundCodes");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.NoContent);
            }

            try
            {
                var responseDataTask = _httpContentDeserializer.DeserializeListAsync<FundCode>(response.Content);

                var message = responseDataTask.Exception?.Message;

                if (responseDataTask.IsFaulted)
                {
                    _logger.LogError("Error message is ", message);
                    throw responseDataTask.Exception?.InnerException ?? new Exception("An error occurred while processing the response.");
                }

                await responseDataTask;
                var dataForRoute = responseDataTask.Result.ToList();

                if (dataForRoute.Any())
                {
                    return new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.OK)
                    {
                        Data = dataForRoute
                    };
                }

                _logger.LogInformation("No content returned from API");
                return new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.NotFound);

            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<FundCode>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.InternalServerError, error);
    }
    
    public async Task<ApiResponse<IEnumerable<MainAccountCode>>> GetMainAccountCodesForRouteAsync(string accountType, string organisation, string paymentType, string schemeType)
    {
        var cacheKey = new { RefDataCacheKeyPrefixes.MainAccountCodes, accountType, organisation, paymentType, schemeType };
        var apiResponse = new ApiResponse<IEnumerable<MainAccountCode>>(false);

        var cachedDataForRoute = _cacheService.GetData<IEnumerable<MainAccountCode>>(cacheKey);

        if (cachedDataForRoute != null)
        {
            var dataForRoute = cachedDataForRoute.ToList();
            apiResponse = new ApiResponse<IEnumerable<MainAccountCode>>(HttpStatusCode.OK)
            {
                Data = dataForRoute
            };
        }
        else
        {
            try
            {
                await mainAccountCodesSemaphore.WaitAsync();

                cachedDataForRoute = _cacheService.GetData<IEnumerable<MainAccountCode>>(cacheKey);
                if (cachedDataForRoute != null)
                {
                    var dataForRoute = cachedDataForRoute.ToList();
                    apiResponse = new ApiResponse<IEnumerable<MainAccountCode>>(HttpStatusCode.OK)
                    {
                        Data = dataForRoute
                    };
                }
                else
                {
                    var dataForRouteResponse = await this.GetMainAccountCodesForRouteFromApiAsync(accountType, organisation, paymentType, schemeType);

                    if (dataForRouteResponse.IsSuccess)
                    {
                        _cacheService.SetData(cacheKey, dataForRouteResponse.Data);

                        apiResponse = dataForRouteResponse;
                    }
                    else
                    {
                        apiResponse = dataForRouteResponse;
                    }
                }
            }
            finally
            {
                mainAccountCodesSemaphore.Release();
            }
        }

        return apiResponse;
    }

    private async Task<ApiResponse<IEnumerable<MainAccountCode>>> GetMainAccountCodesForRouteFromApiAsync(string accountType, string organisation, string paymentType, string schemeType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetMainAccountCodesListAsync(accountType, organisation, paymentType, schemeType);

        _logger.LogInformation($"Calling Reference Data API for MainAccountCodes");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<MainAccountCode>>(HttpStatusCode.NoContent);
            }

            try
            {
                var responseDataTask = _httpContentDeserializer.DeserializeListAsync<MainAccountCode>(response.Content);

                var message = responseDataTask.Exception?.Message;

                if (responseDataTask.IsFaulted)
                {
                    _logger.LogError("Error message is ", message);
                    throw responseDataTask.Exception?.InnerException ?? new Exception("An error occurred while processing the response.");
                }

                await responseDataTask;
                var dataForRoute = responseDataTask.Result.ToList();

                if (dataForRoute.Any())
                {
                    return new ApiResponse<IEnumerable<MainAccountCode>>(HttpStatusCode.OK)
                    {
                        Data = dataForRoute
                    };
                }

                _logger.LogInformation("No content returned from API");
                return new ApiResponse<IEnumerable<MainAccountCode>>(HttpStatusCode.NotFound);

            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<MainAccountCode>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<MainAccountCode>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<MainAccountCode>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<MainAccountCode>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<MainAccountCode>>(HttpStatusCode.InternalServerError, error);
    }
    
    public async Task<ApiResponse<IEnumerable<SchemeCode>>> GetSchemeCodesForRouteAsync(string accountType, string organisation, string paymentType, string schemeType)
    {
        var cacheKey = new { RefDataCacheKeyPrefixes.SchemeCodes, accountType, organisation, paymentType, schemeType };
        var apiResponse = new ApiResponse<IEnumerable<SchemeCode>>(false);

        var cachedDataForRoute = _cacheService.GetData<IEnumerable<SchemeCode>>(cacheKey);

        if (cachedDataForRoute != null)
        {
            var dataForRoute = cachedDataForRoute.ToList();
            apiResponse = new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.OK)
            {
                Data = dataForRoute
            };
        }
        else
        {
            try
            {
                await schemeCodesSemaphore.WaitAsync();

                cachedDataForRoute = _cacheService.GetData<IEnumerable<SchemeCode>>(cacheKey);
                if (cachedDataForRoute != null)
                {
                    var dataForRoute = cachedDataForRoute.ToList();
                    apiResponse = new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.OK)
                    {
                        Data = dataForRoute
                    };
                }
                else
                {
                    var dataForRouteResponse = await this.GetSchemeCodesForRouteFromApiAsync(accountType, organisation, paymentType, schemeType);

                    if (dataForRouteResponse.IsSuccess)
                    {
                        _cacheService.SetData(cacheKey, dataForRouteResponse.Data);

                        apiResponse = dataForRouteResponse;
                    }
                    else
                    {
                        apiResponse = dataForRouteResponse;
                    }
                }
            }
            finally
            {
                schemeCodesSemaphore.Release();
            }
        }

        return apiResponse;
    }

    private async Task<ApiResponse<IEnumerable<SchemeCode>>> GetSchemeCodesForRouteFromApiAsync(string accountType, string organisation, string paymentType, string schemeType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetSchemeCodesListAsync(accountType, organisation, paymentType, schemeType);

        _logger.LogInformation($"Calling Reference Data API for SchemeCodes");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.NoContent);
            }

            try
            {
                var responseDataTask = _httpContentDeserializer.DeserializeListAsync<SchemeCode>(response.Content);

                var message = responseDataTask.Exception?.Message;

                if (responseDataTask.IsFaulted)
                {
                    _logger.LogError("Error message is ", message);
                    throw responseDataTask.Exception?.InnerException ?? new Exception("An error occurred while processing the response.");
                }

                await responseDataTask;
                var dataForRoute = responseDataTask.Result.ToList();

                if (dataForRoute.Any())
                {
                    return new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.OK)
                    {
                        Data = dataForRoute
                    };
                }

                _logger.LogInformation("No content returned from API");
                return new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.NotFound);

            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<SchemeCode>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.InternalServerError, error);
    }

    public async Task<ApiResponse<IEnumerable<MarketingYear>>> GetMarketingYearsForRouteFromApiAsyncAsync(string? accountType, string? organisation, string? paymentType, string? schemeType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetMarketingYearsListAsync(accountType, organisation, paymentType, schemeType);

        _logger.LogInformation($"Calling Reference Data API for Marketing Years");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.NoContent);
            }

            try
            {
                var responseDataTask = _httpContentDeserializer.DeserializeListAsync<MarketingYear>(response.Content);

                var message = responseDataTask.Exception?.Message;

                if (responseDataTask.IsFaulted)
                {
                    _logger.LogError("Error message is ", message);
                    throw responseDataTask.Exception?.InnerException ?? new Exception("An error occurred while processing the response.");
                }

                await responseDataTask;
                var marketingYear = responseDataTask.Result.ToList();

                if (marketingYear.Any())
                {
                    return new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.OK)
                    {
                        Data = marketingYear
                    };
                }

                _logger.LogInformation("No content returned from API");
                return new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.NotFound);

            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<MarketingYear>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.InternalServerError, error);
    }

    public async Task<ApiResponse<IEnumerable<MarketingYear>>> GetMarketingYearsForRouteAsync(string accountType, string organisation, string paymentType, string schemeType)
    {
        var cacheKey = new { RefDataCacheKeyPrefixes.MarketingYears, accountType, organisation, paymentType, schemeType };
        var apiResponse = new ApiResponse<IEnumerable<MarketingYear>>(false);

        var cachedDataForRoute = _cacheService.GetData<IEnumerable<MarketingYear>>(cacheKey);

        if (cachedDataForRoute != null)
        {
            var dataForRoute = cachedDataForRoute.ToList();
            apiResponse = new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.OK)
            {
                Data = dataForRoute
            };
        }
        else
        {
            try
            {
                await marketingYearsSemaphore.WaitAsync();

                cachedDataForRoute = _cacheService.GetData<IEnumerable<MarketingYear>>(cacheKey);
                if (cachedDataForRoute != null)
                {
                    var dataForRoute = cachedDataForRoute.ToList();
                    apiResponse = new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.OK)
                    {
                        Data = dataForRoute
                    };
                }
                else
                {
                    var dataForRouteResponse = await this.GetMarketingYearsForRouteFromApiAsyncAsync(accountType, organisation, paymentType, schemeType);

                    if (dataForRouteResponse.IsSuccess)
                    {
                        _cacheService.SetData(cacheKey, dataForRouteResponse.Data);

                        apiResponse = dataForRouteResponse;
                    }
                    else
                    {
                        apiResponse = dataForRouteResponse;
                    }
                }
            }
            finally
            {
               marketingYearsSemaphore.Release();
            }
        }

        return apiResponse;
    }
}
