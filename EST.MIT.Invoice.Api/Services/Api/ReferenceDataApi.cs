using EST.MIT.Invoice.Api.Services.API.Interfaces;
using EST.MIT.Invoice.Api.Services.API.Models;
using System.Net;
using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Util;


namespace EST.MIT.Invoice.Api.Services.Api;

public class ReferenceDataApi : IReferenceDataApi
{
    private readonly IReferenceDataRepository _referenceDataRepository;
    private readonly ILogger<ReferenceDataApi> _logger;
    private readonly IHttpContentDeserializer _httpContentDeserializer;

    public ReferenceDataApi(IReferenceDataRepository referenceDataRepository, ILogger<ReferenceDataApi> logger, IHttpContentDeserializer httpContentDeserializer)
    {
        _referenceDataRepository = referenceDataRepository;
        _logger = logger;
        _httpContentDeserializer = httpContentDeserializer;
    }

    public async Task<ApiResponse<IEnumerable<PaymentScheme>>> GetSchemeTypesAsync(string? invoiceType, string? organisation)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetSchemeTypesListAsync(invoiceType, organisation);

        _logger.LogInformation($"Calling Reference Data API for Scheme Types");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.NoContent);
            }

            try
            {
                var responseDataTask = _httpContentDeserializer.DeserializeList<PaymentScheme>(response.Content);

                var message = responseDataTask.Exception?.Message;

                if (responseDataTask.IsFaulted)
                {
                    _logger.LogError("Error message is ", message);
                    throw responseDataTask.Exception?.InnerException ?? new Exception("An error occurred while processing the response.");
                }

                await responseDataTask;
                var paymentSchemes = responseDataTask.Result.ToList();

                if (paymentSchemes.Any())
                {
                    return new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK)
                    {
                        Data = paymentSchemes
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

    public async Task<ApiResponse<IEnumerable<PaymentType>>> GetPaymentTypesAsync(string? invoiceType, string? organisation, string? schemeType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetPaymentTypesListAsync(invoiceType, organisation, schemeType);

        _logger.LogInformation($"Calling Reference Data API for Payment Types");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.NoContent);
            }

            try
            {
                var responseDataTask = _httpContentDeserializer.DeserializeList<PaymentType>(response.Content);

                var message = responseDataTask.Exception?.Message;

                if (responseDataTask.IsFaulted)
                {
                    _logger.LogError("Error message is ", message);
                    throw responseDataTask.Exception?.InnerException ?? new Exception("An error occurred while processing the response.");
                }

                await responseDataTask;
                var paymentSchemes = responseDataTask.Result.ToList();

                if (paymentSchemes.Any())
                {
                    return new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.OK)
                    {
                        Data = paymentSchemes
                    };
                }

                _logger.LogInformation("No content returned from API");
                return new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.NotFound);

            }
            catch (Exception ex)
            {
                error.Add("deserializing", new List<string>() { ex.Message });
                return new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.InternalServerError, error)
                {
                    Data = new List<PaymentType>()
                };
            }
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No content returned from API");
            return new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Invalid request was sent to API");
            error.Add($"{HttpStatusCode.BadRequest}", new List<string>() { "Invalid request was sent to API" });

            return new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.BadRequest, error);
        }

        _logger.LogError("Unknown response from API");
        error.Add($"{HttpStatusCode.InternalServerError}", new List<string>() { "Unknown response from API" });
        return new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.InternalServerError, error);
    }

    public async Task<ApiResponse<IEnumerable<Organisation>>> GetOrganisationsAsync(string? invoiceType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetOrganisationsListAsync(invoiceType);

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
                var responseDataTask = _httpContentDeserializer.DeserializeList<Organisation>(response.Content);

                var message = responseDataTask.Exception?.Message;

                if (responseDataTask.IsFaulted)
                {
                    _logger.LogError("Error message is ", message);
                    throw responseDataTask.Exception?.InnerException ?? new Exception("An error occurred while processing the response.");
                }

                await responseDataTask;
                var organisations = responseDataTask.Result.ToList();

                if (organisations.Any())
                {
                    return new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK)
                    {
                        Data = organisations
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

    public async Task<ApiResponse<IEnumerable<SchemeCode>>> GetSchemeCodesAsync(string? invoiceType, string? organisation, string? paymentType, string? schemeType)
    {
        var error = new Dictionary<string, List<string>>();
        var response = await _referenceDataRepository.GetSchemeCodesListAsync(invoiceType, organisation,paymentType, schemeType);

        _logger.LogInformation($"Calling Reference Data API for Scheme Codes");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                _logger.LogWarning("No content returned from API");
                return new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.NoContent);
            }

            try
            {
                var responseDataTask = _httpContentDeserializer.DeserializeList<SchemeCode>(response.Content);

                var message = responseDataTask.Exception?.Message;

                if (responseDataTask.IsFaulted)
                {
                    _logger.LogError("Error message is ", message);
                    throw responseDataTask.Exception?.InnerException ?? new Exception("An error occurred while processing the response.");
                }

                await responseDataTask;
                var schemeCodes = responseDataTask.Result.ToList();

                if (schemeCodes.Any())
                {
                    return new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.OK)
                    {
                        Data = schemeCodes
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
}
