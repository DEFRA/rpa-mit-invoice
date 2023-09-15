using System.Net;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using EST.MIT.Invoice.Api.Services.Api.Models;
using FluentAssertions;
using FluentValidation;
using Invoices.Api.Endpoints;
using Invoices.Api.Models;
using Invoices.Api.Services;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Invoices.Api.Test;

public class InvoiceBulkPostEndpointsTest
{
    private readonly ICosmosService _cosmosService =
        Substitute.For<ICosmosService>();
    private readonly IEventQueueService _eventQueueService =
        Substitute.For<IEventQueueService>();
    private readonly IReferenceDataApi _referenceDataApiMock =
         Substitute.For<IReferenceDataApi>();
    private readonly ICachedReferenceDataApi _cachedReferenceDataApiMock =
        Substitute.For<ICachedReferenceDataApi>();
    private readonly IValidator<BulkInvoices> _validator;
    private readonly PaymentRequestsBatch _paymentRequestsBatchTestData = PaymentRequestsBatchTestData.CreateInvoice();

    public InvoiceBulkPostEndpointsTest()
    {
        var paymentSchemeErrors = new Dictionary<string, List<string>>();
        var orgnisationErrors = new Dictionary<string, List<string>>();
        var payTypesErrors = new Dictionary<string, List<string>>();
        var schemeCodeErrors = new Dictionary<string, List<string>>();
        var fundCodeErrors = new Dictionary<string, List<string>>();
        var combinationsForRouteErrors = new Dictionary<string, List<string>>();

        var response = new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK, paymentSchemeErrors);
        var organisationRespnse = new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK, orgnisationErrors);
        var paymentTypeResponse = new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.OK, payTypesErrors);
        var schemeCodeResponse = new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.OK, schemeCodeErrors);
        var fundCodeResponse = new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.OK, fundCodeErrors);
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

        _referenceDataApiMock
            .GetSchemeCodesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(schemeCodeResponse));

        _referenceDataApiMock
            .GetFundCodesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(fundCodeResponse));

        _cachedReferenceDataApiMock
            .GetCombinationsListForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(combinationsForRouteResponse));

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

        _cosmosService.CreateBulk(bulkInvoices).Returns(bulkInvoices);

        // Act
        var result = await InvoicePostEndpoints.CreateBulkInvoices(bulkInvoices, _validator, _cosmosService, _eventQueueService);

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

        _cosmosService.CreateBulk(bulkInvoices).ReturnsNull();

        var result = await InvoicePostEndpoints.CreateBulkInvoices(bulkInvoices, _validator, _cosmosService, _eventQueueService);

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

        _cosmosService.CreateBulk(bulkInvoices).ReturnsNull();
        _eventQueueService.CreateMessage(bulkInvoices.Reference, "failed", "bulk-invoice-creation-failed", "Bulk Invoice creation failed").Returns(Task.CompletedTask);

        var result = await InvoicePostEndpoints.CreateBulkInvoices(bulkInvoices, _validator, _cosmosService, _eventQueueService);

        result.GetCreatedStatusCode().Should().Be(400);
    }
}