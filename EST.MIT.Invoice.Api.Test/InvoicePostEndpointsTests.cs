using System.Net;
using System.Net.WebSockets;
using EST.MIT.Invoice.Api.Services.API.Interfaces;
using EST.MIT.Invoice.Api.Services.API.Models;
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

    private readonly IEventQueueService _eventQueueService =
        Substitute.For<IEventQueueService>();

    private readonly Invoice invoiceTestData = InvoiceTestData.CreateInvoice();

    private readonly IValidator<Invoice> _validator;

    public InvoicePostEndpointTests()
    {
        var errors = new Dictionary<string, List<string>>();
        var paymentSchemesResponse = new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK, errors);
        var orgnisationErrors = new Dictionary<string, List<string>>();

        var organisationRespnse = new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK, orgnisationErrors);

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

        _referenceDataApiMock
            .GetSchemeTypesAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(paymentSchemesResponse));

        var paymentTypesResponse = new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.OK, errors);
        var paymentTypes = new List<PaymentType>()
        {
            new PaymentType()
            {
                Code = "DOM"
            }
        };
        paymentTypesResponse.Data = paymentTypes;

        _referenceDataApiMock
            .GetPaymentTypesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(paymentTypesResponse));


        _referenceDataApiMock
            .GetOrganisationsAsync(Arg.Any<string>())
            .Returns(Task.FromResult(organisationRespnse));

        _validator = new InvoiceValidator(_referenceDataApiMock);
    }

    [Fact]
    public async Task PostInvoicebySchemeAndInvoiceId_WhenInvoiceDoesNotExist()
    {
        var invoice = invoiceTestData;

        _cosmosService.Create(invoice).Returns(invoice);
        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-created", "Invoice created").Returns(Task.CompletedTask);

        var result = await InvoicePostEndpoints.CreateInvoice(invoice, _validator, _cosmosService, _eventQueueService);

        result.GetCreatedStatusCode().Should().Be(201);
        result.GetCreatedResultValue<Invoice>().Should().BeEquivalentTo(invoice);
    }

    [Fact]
    public async Task PostInvoicebySchemeAndInvoiceId_WhenCreateReturnsNull()
    {
        var invoice = invoiceTestData;

        _cosmosService.Create(invoice).ReturnsNull();
        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-created", "Invoice created").Returns(Task.CompletedTask);

        var result = await InvoicePostEndpoints.CreateInvoice(invoice, _validator, _cosmosService, _eventQueueService);

        result.GetCreatedStatusCode().Should().Be(400);
    }

    [Fact]
    public async Task PostInvoice_When_SchemeType_Is_Missing_Should_Return_Status_Code_400()
    {
        //Arrange
        Invoice invoice = new Invoice()
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
                    DeliveryBody = "Test Org",
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
                            Description = "Description"
                        }
                    }
                }
            }
        };

        _cosmosService.Create(invoice).Returns(invoice);
        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-created", "Invoice created").Returns(Task.CompletedTask);

        //Act
        var result = await InvoicePostEndpoints.CreateInvoice(invoice, _validator, _cosmosService, _eventQueueService);

        //Assert
        result.GetCreatedStatusCode().Should().Be(400);
    }

    [Fact]
    public async Task PostInvoice_When_PaymentType_Is_Missing_Should_Return_Status_Code_400()
    {
        //Arrange
        Invoice invoice = new Invoice()
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
                    DeliveryBody = "Test Org",
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
                            Description = "Description"
                        }
                    }
                }
            }
        };

        _cosmosService.Create(invoice).Returns(invoice);
        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-created", "Invoice created").Returns(Task.CompletedTask);

        //Act
        var result = await InvoicePostEndpoints.CreateInvoice(invoice, _validator, _cosmosService, _eventQueueService);

        //Assert
        result.GetCreatedStatusCode().Should().Be(400);
    }

    [Fact]
    public async Task PostInvoice_When_InvoiceType_Is_Missing_InvoiceHeader_FRN_IsMissing_InvoiceLine_SchemeCode_Is_Missing_Should_Return_Status_Code_400()
    {
        //Arrange
        Invoice invoice = new Invoice()
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
                    DeliveryBody = "Test Org",
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
                            Description = "Description"
                        }
                    }
                }
            }
        };

        _cosmosService.Create(invoice).Returns(invoice);
        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-created", "Invoice created").Returns(Task.CompletedTask);

        //Act
        var result = await InvoicePostEndpoints.CreateInvoice(invoice, _validator, _cosmosService, _eventQueueService);

        //Assert
        result.GetCreatedStatusCode().Should().Be(400);
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
            PaymentRequests = new List<InvoiceHeader>
            {
                new()
                {
                    Value = 123456789
                }
            }
        };

        _eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-create-failed", "Invoice creation failed").Returns(Task.CompletedTask);
        var result = await InvoicePostEndpoints.CreateInvoice(invoice, _validator, _cosmosService, _eventQueueService);

        result.GetBadRequestResultValue<HttpValidationProblemDetails>().Should().NotBeNull();
        result?.GetBadRequestResultValue<HttpValidationProblemDetails>()?.Errors.Should().ContainKey(errorKey);
    }
}
