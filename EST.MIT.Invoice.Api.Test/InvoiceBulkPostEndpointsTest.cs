using System.Net;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using EST.MIT.Invoice.Api.Services.Api.Models;
using FluentAssertions;
using FluentValidation;
using EST.MIT.Invoice.Api.Endpoints;
using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Services;
using EST.MIT.Invoice.Api.Services.Api;
using EST.MIT.Invoice.Api.Services.PaymentsBatch;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace EST.MIT.Invoice.Api.Test;

public class InvoiceBulkPostEndpointsTest
{
    private readonly IPaymentRequestsBatchService _paymentRequestsBatchService =
        Substitute.For<IPaymentRequestsBatchService>();
    private readonly IEventQueueService _eventQueueService =
        Substitute.For<IEventQueueService>();
    private readonly IReferenceDataApi _referenceDataApiMock =
         Substitute.For<IReferenceDataApi>();
    private readonly ICachedReferenceDataApi _cachedReferenceDataApiMock =
        Substitute.For<ICachedReferenceDataApi>();

    private readonly IMockedDataService _mockedDataService = new MockedDataService();
    private readonly IValidator<BulkInvoices> _validator;
    private readonly PaymentRequestsBatch _paymentRequestsBatchTestData = PaymentRequestsBatchTestData.CreateInvoice(InvoiceStatuses.Approved);

    public InvoiceBulkPostEndpointsTest()
    {
        var paymentSchemeErrors = new Dictionary<string, List<string>>();
        var orgnisationErrors = new Dictionary<string, List<string>>();
        var payTypesErrors = new Dictionary<string, List<string>>();
        var schemeCodeErrors = new Dictionary<string, List<string>>();
        var fundCodeErrors = new Dictionary<string, List<string>>();
        var combinationsForRouteErrors = new Dictionary<string, List<string>>();
        var marketingYearErrors = new Dictionary<string, List<string>>();

        var response = new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK, paymentSchemeErrors);
        var organisationRespnse = new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK, orgnisationErrors);
        var paymentTypeResponse = new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.OK, payTypesErrors);
        var schemeCodeResponse = new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.OK, schemeCodeErrors);
        var fundCodeResponse = new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.OK, fundCodeErrors);
        var marketingYearResponse = new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.OK, marketingYearErrors);
        var combinationsForRouteResponse = new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.OK, combinationsForRouteErrors);

        var paymentSchemes = new List<PaymentScheme>()
        {
            new PaymentScheme()
            {
                Code = "bps"
            }
        };
        response.Data = paymentSchemes;

        var organisation = new List<Organisation>()
        {
            new Organisation()
            {
                 Code = "Test Org"
            }
        };
        organisationRespnse.Data = organisation;

        var paymentTypes = new List<PaymentType>()
        {
            new PaymentType()
            {
                Code = "DOM"
            }
        };
        paymentTypeResponse.Data = paymentTypes;

        var schemeCodes = new List<SchemeCode>()
        {
            new SchemeCode()
            {
                Code = "SchemeCodeValue"
            }
        };
        schemeCodeResponse.Data = schemeCodes;

        var fundCodes = new List<FundCode>()
        {
            new FundCode()
            {
                Code = "123456789"
            }
        };
        fundCodeResponse.Data = fundCodes;

        var marketingYears = new List<MarketingYear>()
        {
            new MarketingYear() {
                Code ="2023"
            }
        };
        marketingYearResponse.Data = marketingYears;

        var combinationsForRoute = new List<CombinationForRoute>()
        {
            new CombinationForRoute()
            {
                AccountCode = "AccountCodeValue",
                DeliveryBodyCode = "RP00",
                SchemeCode = "SchemeCodeValue",
            },
            new CombinationForRoute()
            {
                AccountCode = "AccountCodeValue",
                DeliveryBodyCode = "RP01",
                SchemeCode = "SchemeCodeValue",
            }
        };
        combinationsForRouteResponse.Data = combinationsForRoute;

        _referenceDataApiMock
            .GetSchemeTypesAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(response));

        _referenceDataApiMock
             .GetOrganisationsAsync(Arg.Any<string>())
             .Returns(Task.FromResult(organisationRespnse));

        _referenceDataApiMock
            .GetPaymentTypesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<String>())
            .Returns(Task.FromResult(paymentTypeResponse));

        _cachedReferenceDataApiMock
            .GetSchemeCodesForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(schemeCodeResponse));

        _cachedReferenceDataApiMock.GetFundCodesForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(fundCodeResponse));

        _cachedReferenceDataApiMock
             .GetMarketingYearsForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
             .Returns(Task.FromResult(marketingYearResponse));

        _cachedReferenceDataApiMock
            .GetCombinationsListForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(combinationsForRouteResponse));


        var mainAccountCodesErrors = new Dictionary<string, List<string>>();
        var mainAccountCodeResponse = new ApiResponse<IEnumerable<MainAccountCode>>(HttpStatusCode.OK, mainAccountCodesErrors);

        var deliveryBodyCodesErrors = new Dictionary<string, List<string>>();
        var deliveryBodyCodeResponse = new ApiResponse<IEnumerable<DeliveryBodyCode>>(HttpStatusCode.OK, deliveryBodyCodesErrors);

        var mainAccountCodes = new List<MainAccountCode>()
        {
            new MainAccountCode()
            {
                Code = "AccountCodeValue"
            },
        };
        mainAccountCodeResponse.Data = mainAccountCodes;

        var deliveryBodyCodes = new List<DeliveryBodyCode>()
        {
            new DeliveryBodyCode()
            {
                Code = "RP00"
            },
            new DeliveryBodyCode()
            {
                Code = "RP01"
            }
        };
        deliveryBodyCodeResponse.Data = deliveryBodyCodes;

        _cachedReferenceDataApiMock.GetMainAccountCodesForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(mainAccountCodeResponse));

        _cachedReferenceDataApiMock.GetDeliveryBodyCodesForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(deliveryBodyCodeResponse));

        _validator = new BulkInvoiceValidator(_referenceDataApiMock, _cachedReferenceDataApiMock);
    }

    [Fact]
    public async Task CreateBulkInvoices_ShouldReturnOk()
    {
        // Arrange
        var bulkInvoices = new BulkInvoices
        {
            SchemeType = "Scheme1",
            Reference = "Reference1",
            Invoices = new List<PaymentRequestsBatch>
            {
                _paymentRequestsBatchTestData
            }
        };

		_paymentRequestsBatchService.CreateBulkAsync(bulkInvoices, Arg.Any<LoggedInUser>()).Returns(bulkInvoices);

        // Act
        var result = await InvoicePostEndpoints.CreateBulkInvoices(bulkInvoices, _validator, _paymentRequestsBatchService, _eventQueueService, _mockedDataService);

        result.GetCreatedStatusCode().Should().Be(200);
    }

    [Fact]
    public async Task CreateBulkInvoices_ShouldReturnBadRequest_ValidationFailed()
    {
        var bulkInvoices = new BulkInvoices
        {
            SchemeType = "Scheme1",
            Invoices = new List<PaymentRequestsBatch>
            {
                _paymentRequestsBatchTestData
            }
        };

		_paymentRequestsBatchService.CreateBulkAsync(bulkInvoices, Arg.Any<LoggedInUser>()).ReturnsNull();

        var result = await InvoicePostEndpoints.CreateBulkInvoices(bulkInvoices, _validator, _paymentRequestsBatchService, _eventQueueService, _mockedDataService);

        result.GetBadRequestResultValue<HttpValidationProblemDetails>().Should().NotBeNull();
        result?.GetBadRequestResultValue<HttpValidationProblemDetails>()?.Errors.Should().ContainKey("Reference");
    }

    [Fact]
    public async Task CreateBulkInvoices_ShouldReturnBadRequest()
    {
        var bulkInvoices = new BulkInvoices
        {
            SchemeType = "Scheme1",
            Reference = "Reference1",
            Invoices = new List<PaymentRequestsBatch>
            {
                _paymentRequestsBatchTestData
            }
        };

		_paymentRequestsBatchService.CreateBulkAsync(bulkInvoices, Arg.Any<LoggedInUser>()).ReturnsNull();
        _eventQueueService.CreateMessage(bulkInvoices.Reference, "failed", "bulk-invoice-creation-failed", "Bulk invoice creation failed").Returns(Task.CompletedTask);

        var result = await InvoicePostEndpoints.CreateBulkInvoices(bulkInvoices, _validator, _paymentRequestsBatchService, _eventQueueService, _mockedDataService);

        result.GetCreatedStatusCode().Should().Be(400);
    }
}