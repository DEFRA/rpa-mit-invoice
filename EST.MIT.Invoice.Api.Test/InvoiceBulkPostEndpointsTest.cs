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
    private readonly IValidator<BulkInvoices> _validator;
    private readonly Invoice invoiceTestData = InvoiceTestData.CreateInvoice();

    public InvoiceBulkPostEndpointsTest()
    {
        var paymentSchemeErrors = new Dictionary<string, List<string>>();
        var orgnisationErrors = new Dictionary<string, List<string>>();
        var payTypesErrors = new Dictionary<string, List<string>>();
        var schemeCodeErrors = new Dictionary<string, List<string>>();
        var deliveryBodyCodesErrors = new Dictionary<string, List<string>>();
        var fundCodeErrors = new Dictionary<string, List<string>>();

        var response = new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK, paymentSchemeErrors);
        var organisationRespnse = new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK, orgnisationErrors);
        var paymentTypeResponse = new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.OK, payTypesErrors);
        var schemeCodeResponse = new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.OK, schemeCodeErrors);
        var deliveryBodyCodesResponse = new ApiResponse<IEnumerable<DeliveryBodyCode>>(HttpStatusCode.OK, deliveryBodyCodesErrors);
        var fundCodeResponse = new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.OK, fundCodeErrors);

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
                Code = "123456789"
            }
        };
        schemeCodeResponse.Data = schemeCodes;

        var deliveryBodyCodes = new List<DeliveryBodyCode>()
        {
            new DeliveryBodyCode()
            {
                Code = "RP00",
                Description =  "England"
            },
            new DeliveryBodyCode()
            {
                Code = "RP01",
                Description =  "Scotland"
            }
        };
        deliveryBodyCodesResponse.Data = deliveryBodyCodes;

        var fundCodes = new List<FundCode>()
        {
            new FundCode()
            {
                Code = "123456789"
            }
        };
        fundCodeResponse.Data = fundCodes;

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
            .GetDeliveryBodyCodesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(deliveryBodyCodesResponse));

        _referenceDataApiMock
            .GetFundCodesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(fundCodeResponse));

        _validator = new BulkInvoiceValidator(_referenceDataApiMock);
    }

    [Fact]
    public async Task CreateBulkInvoices_ShouldReturnOk()
    {
        // Arrange
        var bulkInvoices = new BulkInvoices
        {
            SchemeType = "Scheme1",
            Reference = "Reference1",
            Invoices = new List<Invoice>
            {
                invoiceTestData
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
            Invoices = new List<Invoice>
            {
                invoiceTestData
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
            Invoices = new List<Invoice>
            {
                invoiceTestData
            }
        };

        _cosmosService.CreateBulk(bulkInvoices).ReturnsNull();
        _eventQueueService.CreateMessage(bulkInvoices.Reference, "failed", "bulk-invoice-creation-falied", "Bulk invoice creation failed").Returns(Task.CompletedTask);

        var result = await InvoicePostEndpoints.CreateBulkInvoices(bulkInvoices, _validator, _cosmosService, _eventQueueService);

        result.GetCreatedStatusCode().Should().Be(400);
    }
}