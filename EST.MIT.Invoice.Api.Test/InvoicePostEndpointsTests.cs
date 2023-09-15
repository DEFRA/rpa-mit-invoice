using System.Net;
using System.Net.WebSockets;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using EST.MIT.Invoice.Api.Services.Api.Models;
using Invoices.Api.Services;
using Invoices.Api.Endpoints;
using NSubstitute;
using FluentAssertions;
using Invoices.Api.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Moq;
using NSubstitute.ReturnsExtensions;

namespace Invoices.Api.Test;

public class InvoicePostEndpointTests
{
    private readonly ICosmosService _cosmosService =
        Substitute.For<ICosmosService>();

    private readonly IReferenceDataApi _referenceDataApiMock =
        Substitute.For<IReferenceDataApi>();

    private readonly ICachedReferenceDataApi _cachedReferenceDataApiMock =
        Substitute.For<ICachedReferenceDataApi>();

    private readonly IEventQueueService _eventQueueService =
        Substitute.For<IEventQueueService>();

    private readonly PaymentRequestsBatch _paymentRequestsBatchTestData = PaymentRequestsBatchTestData.CreateInvoice();

    private readonly IValidator<PaymentRequestsBatch> _validator;

    public InvoicePostEndpointTests()
    {
        var errors = new Dictionary<string, List<string>>();
        var paymentSchemesResponse = new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK, errors);
        var orgnisationErrors = new Dictionary<string, List<string>>();
        var schemeCodeErrors = new Dictionary<string, List<string>>();
        var fundCodeErrors = new Dictionary<string, List<string>>();
        var combinationsForRouteErrors = new Dictionary<string, List<string>>();

        var organisationRespnse = new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK, orgnisationErrors);
        var schemeCodeResponse = new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.OK, schemeCodeErrors);
        var fundCodeResponse = new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.OK, fundCodeErrors);
        var paymentTypesResponse = new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.OK, errors);
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

        var schemeCodes = new List<SchemeCode>()
        {
            new SchemeCode()
            {
                Code = "SchemeCodeValue"
            }
        };
        schemeCodeResponse.Data = schemeCodes;

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

        _validator = new PaymentRequestsBatchValidator(_referenceDataApiMock, _cachedReferenceDataApiMock);
    }

    [Fact]
    public async Task PostInvoicebySchemeAndInvoiceId_WhenInvoiceDoesNotExist()
    {
        var invoice = _paymentRequestsBatchTestData;

        _cosmosService.Create(invoice).Returns(invoice);
        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "paymentRequestsBatch-created", "PaymentRequestsBatch created").Returns(Task.CompletedTask);

        var result = await InvoicePostEndpoints.CreateInvoice(invoice, _validator, _cosmosService, _eventQueueService);

        result.GetCreatedStatusCode().Should().Be(201);
        result.GetCreatedResultValue<PaymentRequestsBatch>().Should().BeEquivalentTo(invoice);
    }

    [Fact]
    public async Task PostInvoicebySchemeAndInvoiceId_WhenCreateReturnsNull()
    {
        var invoice = _paymentRequestsBatchTestData;

        _cosmosService.Create(invoice).ReturnsNull();
        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "paymentRequestsBatch-created", "PaymentRequestsBatch created").Returns(Task.CompletedTask);

        var result = await InvoicePostEndpoints.CreateInvoice(invoice, _validator, _cosmosService, _eventQueueService);

        result.GetCreatedStatusCode().Should().Be(400);
    }

    [Fact]
    public async Task PostInvoice_When_SchemeType_Is_Missing_Should_Return_Status_Code_400()
    {
        //Arrange
        PaymentRequestsBatch paymentRequestsBatch = new PaymentRequestsBatch()
        {
            Id = "123456789",
            InvoiceType = "AP",
            AccountType = "AP",
            PaymentType = "DOM",
            Organisation = "Test Org",
            Reference = "123456789",
            CreatedBy = "Test User",
            Status = "status",
            PaymentRequests = new List<InvoiceHeader> {
                new InvoiceHeader {
                    PaymentRequestId = "123456789",
                    SourceSystem = "Manual",
                    MarketingYear = 2023,
                    FRN = 1000000000,
                    PaymentRequestNumber = 123456789,
                    ContractNumber = "123456789",
                    Value = 100,
                    DueDate = "2023-01-01",
                    AgreementNumber = "DE4567",
                    AppendixReferences = new AppendixReferences {
                        ClaimReferenceNumber = "123456789"
                    },
                    InvoiceLines = new List<InvoiceLine> {
                        new InvoiceLine {
                            Currency = "GBP",
                            Value = 100,
                            SchemeCode = "123456789",
                            FundCode = "123456789",
                            Description = "Description",
                            MainAccount = "AccountA",
                            DeliveryBody = "RP00",
                        }
                    }
                }
            }
        };

        _cosmosService.Create(paymentRequestsBatch).Returns(paymentRequestsBatch);
        _eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "paymentRequestsBatch-created", "PaymentRequestsBatch created").Returns(Task.CompletedTask);

        //Act
        var result = await InvoicePostEndpoints.CreateInvoice(paymentRequestsBatch, _validator, _cosmosService, _eventQueueService);

        //Assert
        result.GetCreatedStatusCode().Should().Be(400);
    }

    [Fact]
    public async Task PostInvoice_When_PaymentType_Is_Missing_Should_Return_Status_Code_400()
    {
        //Arrange
        PaymentRequestsBatch paymentRequestsBatch = new PaymentRequestsBatch()
        {
            Id = "123456789",
            InvoiceType = "AP",
            AccountType = "AP",
            SchemeType = "XP",
            Organisation = "Test Org",
            Reference = "123456789",
            CreatedBy = "Test User",
            Status = "status",
            PaymentRequests = new List<InvoiceHeader> {
                new InvoiceHeader {
                    PaymentRequestId = "123456789",
                    SourceSystem = "Manual",
                    MarketingYear = 2023,
                    FRN = 1000000000,
                    PaymentRequestNumber = 123456789,
                    ContractNumber = "123456789",
                    Value = 100,
                    DueDate = "2023-01-01",
                    AgreementNumber = "DE4567",
                    AppendixReferences = new AppendixReferences {
                        ClaimReferenceNumber = "123456789"
                    },
                    InvoiceLines = new List<InvoiceLine> {
                        new InvoiceLine {
                            Currency = "GBP",
                            Value = 100,
                            SchemeCode = "123456789",
                            FundCode = "123456789",
                            Description = "Description",
                            MainAccount = "AccountA",
                            DeliveryBody = "RP00",
                        }
                    }
                }
            }
        };

        _cosmosService.Create(paymentRequestsBatch).Returns(paymentRequestsBatch);
        _eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "paymentRequestsBatch-created", "PaymentRequestsBatch created").Returns(Task.CompletedTask);

        //Act
        var result = await InvoicePostEndpoints.CreateInvoice(paymentRequestsBatch, _validator, _cosmosService, _eventQueueService);

        //Assert
        result.GetCreatedStatusCode().Should().Be(400);
    }

    [Fact]
    public async Task PostInvoice_When_InvoiceType_Is_Missing_InvoiceHeader_FRN_IsMissing_InvoiceLine_SchemeCode_Is_Missing_Should_Return_Status_Code_400()
    {
        //Arrange
        PaymentRequestsBatch paymentRequestsBatch = new PaymentRequestsBatch()
        {
            Id = "123456789",
            SchemeType = "XP",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "123456789",
            CreatedBy = "Test User",
            Status = "status",
            PaymentRequests = new List<InvoiceHeader> {
                new InvoiceHeader {
                    PaymentRequestId = "123456789",
                    SourceSystem = "Manual",
                    MarketingYear = 2023,
                    PaymentRequestNumber = 123456789,
                    ContractNumber = "123456789",
                    Value = 100,
                    DueDate = "2023-01-01",
                    AgreementNumber = "DE4567",
                    AppendixReferences = new AppendixReferences {
                        ClaimReferenceNumber = "123456789"
                    },
                    InvoiceLines = new List<InvoiceLine> {
                        new InvoiceLine {
                            Currency = "GBP",
                            Value = 100,
                            FundCode = "123456789",
                            Description = "Description",
                            MainAccount = "AccountA",
                            DeliveryBody = "RP00",
                        }
                    }
                }
            }
        };

        _cosmosService.Create(paymentRequestsBatch).Returns(paymentRequestsBatch);
        _eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "paymentRequestsBatch-created", "PaymentRequestsBatch created").Returns(Task.CompletedTask);

        //Act
        var result = await InvoicePostEndpoints.CreateInvoice(paymentRequestsBatch, _validator, _cosmosService, _eventQueueService);

        //Assert
        result.GetCreatedStatusCode().Should().Be(400);
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
            InvoiceType = "ap",
            AccountType = "ap",
            PaymentRequests = new List<InvoiceHeader>
            {
                new()
                {
                    Value = 123456789,
                    PaymentRequestId = "abc",
                }
            }
        };

        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "paymentRequestsBatch-create-failed", "PaymentRequestsBatch creation failed").Returns(Task.CompletedTask);
        var result = await InvoicePostEndpoints.CreateInvoice(invoice, _validator, _cosmosService, _eventQueueService);

        result.GetBadRequestResultValue<HttpValidationProblemDetails>().Should().NotBeNull();
        result?.GetBadRequestResultValue<HttpValidationProblemDetails>()?.Errors.Should().ContainKey(errorKey);
    }
}
