using EST.MIT.Invoice.Api.Services;
using EST.MIT.Invoice.Api.Endpoints;
using NSubstitute;
using FluentAssertions;
using EST.MIT.Invoice.Api.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using NSubstitute.ReturnsExtensions;
using EST.MIT.Invoice.Api.Services.Api.Models;
using System.Net;
using EST.MIT.Invoice.Api.Services.Api;
using EST.MIT.Invoice.Api.Services.PaymentsBatch;

namespace EST.MIT.Invoice.Api.Test;

public class InvoicePutEndpointTests
{
    private readonly IPaymentRequestsBatchService _paymentRequestsBatchService =
        Substitute.For<IPaymentRequestsBatchService>();

    private readonly IReferenceDataApi _referenceDataApiMock =
        Substitute.For<IReferenceDataApi>();

    private readonly ICachedReferenceDataApi _cachedReferenceDataApiMock =
        Substitute.For<ICachedReferenceDataApi>();

    private readonly IPaymentQueueService _paymentQueueService =
        Substitute.For<IPaymentQueueService>();

    private readonly IEventQueueService _eventQueueService =
        Substitute.For<IEventQueueService>();

    private readonly IMockedDataService _mockedDataService = new MockedDataService();

    private readonly IValidator<PaymentRequestsBatch> _validator;

    private readonly PaymentRequestsBatch _paymentRequestsBatchTestData = PaymentRequestsBatchTestData.CreateInvoice(InvoiceStatuses.Approved);

    public InvoicePutEndpointTests()
    {
        var errors = new Dictionary<string, List<string>>();
        var orgnisationErrors = new Dictionary<string, List<string>>();
        var schemeCodeErrors = new Dictionary<string, List<string>>();
        var fundCodeErrors = new Dictionary<string, List<string>>();
        var combinationsForRouteErrors = new Dictionary<string, List<string>>();
        var marketingYearErrors = new Dictionary<string, List<string>>();

        var schemeCodeResponse = new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.OK, schemeCodeErrors);
        var paymentSchemesResponse = new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK, errors);
        var organisationRespnse = new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK, orgnisationErrors);
        var paymentTypesResponse = new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.OK, errors);
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
        paymentSchemesResponse.Data = paymentSchemes;

        var organisation = new List<Organisation>()
        {
            new Organisation()
            {
                 Code = "Test Org"
            }
        };

        organisationRespnse.Data = organisation;

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

        var paymentTypes = new List<PaymentType>()
        {
            new PaymentType()
            {
                Code = "DOM"
            }
        };
        paymentTypesResponse.Data = paymentTypes;

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
            .GetPaymentTypesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(paymentTypesResponse));
        _referenceDataApiMock
             .GetOrganisationsAsync(Arg.Any<string>())
             .Returns(Task.FromResult(organisationRespnse));

        _cachedReferenceDataApiMock
            .GetSchemeCodesForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(schemeCodeResponse));

        _referenceDataApiMock
            .GetSchemeTypesAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(paymentSchemesResponse));

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

        _cachedReferenceDataApiMock
            .GetMainAccountCodesForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(mainAccountCodeResponse));

        _cachedReferenceDataApiMock.GetDeliveryBodyCodesForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(deliveryBodyCodeResponse));

        _validator = new PaymentRequestsBatchValidator(_referenceDataApiMock, _cachedReferenceDataApiMock);
    }

    [Fact]
    public async Task PutInvoicebySchemeAndInvoiceId_WhenInvoiceExists()
    {
        var invoice = _paymentRequestsBatchTestData;

        _paymentRequestsBatchService.UpdateAsync(invoice, Arg.Any<LoggedInUser>()).Returns(invoice);
        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-updated", "Invoice updated").Returns(Task.CompletedTask);

        var result = await InvoicePutEndpoints.UpdateInvoice(invoice.Id, invoice, _paymentRequestsBatchService, _paymentQueueService, _validator, _eventQueueService, _mockedDataService);

        result.GetOkObjectResultStatusCode().Should().Be(200);
        result.GetOkObjectResultValue<PaymentRequestsBatch>().Should().BeEquivalentTo(invoice);

        await _paymentQueueService.DidNotReceive().CreateMessage("");
    }

    [Fact]
    public async Task PostInvoicebySchemeAndInvoiceId_WhenCreateReturnsNull()
    {
        var invoice = _paymentRequestsBatchTestData;

		_paymentRequestsBatchService.UpdateAsync(invoice, Arg.Any<LoggedInUser>()).ReturnsNull();
        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-updated", "Invoice updated").Returns(Task.CompletedTask);

        var result = await InvoicePutEndpoints.UpdateInvoice(invoice.Id, invoice, _paymentRequestsBatchService, _paymentQueueService, _validator, _eventQueueService, _mockedDataService);

        result.GetCreatedStatusCode().Should().Be(400);
    }

    [Fact]
    public async Task PutInvoicebySchemeAndInvoiceId_WhenApproved()
    {
        var invoice = PaymentRequestsBatchTestData.CreateInvoice(InvoiceStatuses.Approved);

		_paymentRequestsBatchService.UpdateAsync(invoice, Arg.Any<LoggedInUser>()).Returns(invoice);
        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-updated", "Invoice updated").Returns(Task.CompletedTask);

        var result = await InvoicePutEndpoints.UpdateInvoice(invoice.Id, invoice, _paymentRequestsBatchService, _paymentQueueService, _validator, _eventQueueService, _mockedDataService);

        result.GetOkObjectResultStatusCode().Should().Be(200);
        result.GetOkObjectResultValue<PaymentRequestsBatch>().Should().BeEquivalentTo(invoice);

        var expectedMessage = JsonSerializer.Serialize(new InvoiceGenerator { Id = invoice.Id, Scheme = invoice.SchemeType });
        await _paymentQueueService.Received().CreateMessage(expectedMessage);
    }

    [Theory]
    [ClassData(typeof(PaymentRequestsBatchValidationTestData))]
    public async Task PostInvoicebySchemeAndInvoiceId_WhenInvoiceMissingInvoiceProperties(string id, string scheme, string status, string errorKey)
    {
        var invoice = new PaymentRequestsBatch
        {
            Id = id,
            SchemeType = scheme,
            Status = status,
            AccountType = "ap",
        };

        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-validation-failed", "Invoice validation failed").Returns(Task.CompletedTask);
        var result = await InvoicePutEndpoints.UpdateInvoice(id, invoice, _paymentRequestsBatchService, _paymentQueueService, _validator, _eventQueueService, _mockedDataService);

        result.GetBadRequestResultValue<HttpValidationProblemDetails>().Should().NotBeNull();
        result?.GetBadRequestResultValue<HttpValidationProblemDetails>()?.Errors.Should().ContainKey(errorKey);
    }
}