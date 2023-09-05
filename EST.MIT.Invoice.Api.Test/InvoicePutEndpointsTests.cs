using Invoices.Api.Services;
using Invoices.Api.Endpoints;
using NSubstitute;
using FluentAssertions;
using Invoices.Api.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using NSubstitute.ReturnsExtensions;
using EST.MIT.Invoice.Api.Services.Api.Models;
using System.Net;

namespace Invoices.Api.Test;

public class InvoicePutEndpointTests
{
    private readonly ICosmosService _cosmosService =
        Substitute.For<ICosmosService>();

    private readonly IReferenceDataApi _referenceDataApiMock =
        Substitute.For<IReferenceDataApi>();

    private readonly ICachedReferenceDataApi _cachedReferenceDataApiMock =
        Substitute.For<ICachedReferenceDataApi>();

    private readonly IQueueService _queueService =
        Substitute.For<IQueueService>();

    private readonly IEventQueueService _eventQueueService =
        Substitute.For<IEventQueueService>();

    private readonly IValidator<Invoice> _validator;

    private readonly Invoice invoiceTestData = InvoiceTestData.CreateInvoice();

    public InvoicePutEndpointTests()
    {
        var errors = new Dictionary<string, List<string>>();
        var orgnisationErrors = new Dictionary<string, List<string>>();
        var schemeCodeErrors = new Dictionary<string, List<string>>();
        var fundCodeErrors = new Dictionary<string, List<string>>();
        var combinationsForRouteErrors = new Dictionary<string, List<string>>();
        var mainAccountErrors = new Dictionary<string, List<string>>();

        var schemeCodeResponse = new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.OK, schemeCodeErrors);
        var paymentSchemesResponse = new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK, errors);
        var organisationRespnse = new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK, orgnisationErrors);
        var paymentTypesResponse = new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.OK, errors);
        var fundCodeResponse = new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.OK, fundCodeErrors);
        var combinationsForRouteResponse = new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.OK, combinationsForRouteErrors);
        var mainAccountResponse = new ApiResponse<IEnumerable<MainAccount>>(HttpStatusCode.OK, mainAccountErrors);

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
                Code = "123456789"
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

        var mainAccounts = new List<MainAccount>()
        {
            new MainAccount()
            {
                Code = "AccountA"
            }
        };
        mainAccountResponse.Data = mainAccounts;

        _referenceDataApiMock
            .GetPaymentTypesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(paymentTypesResponse));
        _referenceDataApiMock
             .GetOrganisationsAsync(Arg.Any<string>())
             .Returns(Task.FromResult(organisationRespnse));

        _referenceDataApiMock
            .GetSchemeCodesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(schemeCodeResponse));

        _referenceDataApiMock
            .GetSchemeTypesAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(paymentSchemesResponse));

        _referenceDataApiMock
             .GetFundCodesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
             .Returns(Task.FromResult(fundCodeResponse));

        _cachedReferenceDataApiMock
            .GetCombinationsListForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(combinationsForRouteResponse));

        _validator = new InvoiceValidator(_referenceDataApiMock, _cachedReferenceDataApiMock);
        _referenceDataApiMock
            .GetMainAccountsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(mainAccountResponse));

        _validator = new InvoiceValidator(_referenceDataApiMock);
    }

    [Fact]
    public async Task PutInvoicebySchemeAndInvoiceId_WhenInvoiceExists()
    {
        var invoice = invoiceTestData;

        _cosmosService.Update(invoice).Returns(invoice);
        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-updated", "Invoice updated").Returns(Task.CompletedTask);

        var result = await InvoicePutEndpoints.UpdateInvoice(invoice.Id, invoice, _cosmosService, _queueService, _validator, _eventQueueService);

        result.GetOkObjectResultStatusCode().Should().Be(200);
        result.GetOkObjectResultValue<Invoice>().Should().BeEquivalentTo(invoice);

        await _queueService.DidNotReceive().CreateMessage("");
    }

    [Fact]
    public async Task PostInvoicebySchemeAndInvoiceId_WhenCreateReturnsNull()
    {
        var invoice = invoiceTestData;

        _cosmosService.Update(invoice).ReturnsNull();
        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-updated", "Invoice updated").Returns(Task.CompletedTask);

        var result = await InvoicePutEndpoints.UpdateInvoice(invoice.Id, invoice, _cosmosService, _queueService, _validator, _eventQueueService);

        result.GetCreatedStatusCode().Should().Be(400);
    }

    [Fact]
    public async Task PutInvoicebySchemeAndInvoiceId_WhenApproved()
    {
        var invoice = InvoiceTestData.CreateInvoice("approved");

        _cosmosService.Update(invoice).Returns(invoice);
        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-updated", "Invoice updated").Returns(Task.CompletedTask);

        var result = await InvoicePutEndpoints.UpdateInvoice(invoice.Id, invoice, _cosmosService, _queueService, _validator, _eventQueueService);

        result.GetOkObjectResultStatusCode().Should().Be(200);
        result.GetOkObjectResultValue<Invoice>().Should().BeEquivalentTo(invoice);

        var expectedMessage = JsonSerializer.Serialize(new InvoiceGenerator { Id = invoice.Id, Scheme = invoice.SchemeType });
        await _queueService.Received().CreateMessage(expectedMessage);
    }

    [Theory]
    [ClassData(typeof(InvoiceValidationTestData))]
    public async Task PostInvoicebySchemeAndInvoiceId_WhenInvoiceMissingInvoiceProperties(string id, string scheme, string status, string errorKey)
    {
        var invoice = new Invoice
        {
            Id = id,
            SchemeType = scheme,
            Status = status,
            InvoiceType = "ap",
            AccountType = "ap",
        };

        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-validation-failed", "Invoice validation failed").Returns(Task.CompletedTask);
        var result = await InvoicePutEndpoints.UpdateInvoice(id, invoice, _cosmosService, _queueService, _validator, _eventQueueService);

        result.GetBadRequestResultValue<HttpValidationProblemDetails>().Should().NotBeNull();
        result?.GetBadRequestResultValue<HttpValidationProblemDetails>()?.Errors.Should().ContainKey(errorKey);
    }
}