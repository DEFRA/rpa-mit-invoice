using EST.MIT.Invoice.Api.Services.API.Interfaces;
using EST.MIT.Invoice.Api.Services.API.Models;
using FluentValidation.TestHelper;
using Invoices.Api.Models;
using NSubstitute;
using System.Net;

namespace Invoices.Api.Test;

public class InvoiceIdValidationTests
{
    private readonly InvoiceValidator _invoiceValidator;
    private readonly IReferenceDataApi _referenceDataApiMock =
       Substitute.For<IReferenceDataApi>();

    public InvoiceIdValidationTests()
    {
        var paymentSchemeErrors = new Dictionary<string, List<string>>();
        var orgnisationErrors = new Dictionary<string, List<string>>();
        var payTypesErrors = new Dictionary<string, List<string>>();

        var response = new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK, paymentSchemeErrors);
        var organisationRespnse = new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK, orgnisationErrors);
        var paymentTypeResponse = new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.OK, payTypesErrors);

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
                Code = "AP"
            }
        };
        paymentTypeResponse.Data = paymentTypes;

        _referenceDataApiMock
         .GetSchemeTypesAsync(Arg.Any<string>(), Arg.Any<string>())
         .Returns(Task.FromResult(response));

        _referenceDataApiMock
        .GetOrganisationsAsync(Arg.Any<string>())
        .Returns(Task.FromResult(organisationRespnse));

        _referenceDataApiMock
        .GetPaymentTypesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<String>())
        .Returns(Task.FromResult(paymentTypeResponse));

        _invoiceValidator = new InvoiceValidator(_referenceDataApiMock);
    }

    [Fact]
    public async Task Given_Invoice_When_InvoiceId_Is_Null_Or_Empty_Then_Failure_Message_InvoiceId_Is_Missing_Is_Thrown()
    {
        //Arrange
        Invoice invoice = new Invoice()
        {
            Id = string.Empty,
            InvoiceType = "AP",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "123456789",
            SchemeType = "bps",
            PaymentType = "AP",
            CreatedBy = "Test User",
            Status = "status",
            PaymentRequests = new List<InvoiceHeader> {
                new InvoiceHeader {
                    PaymentRequestId = "123456789",
                    SourceSystem = "Manual",
                    MarketingYear = 2023,
                    DeliveryBody = "Test Org",
                    PaymentRequestNumber = 123456789,
                    AgreementNumber = "123456789",
                    ContractNumber = "123456789",
                    Value = 100,
                    DueDate = "2023-01-01",
                    FRN = 1000000000,
                    AppendixReferences = new AppendixReferences {
                        ClaimReferenceNumber = "123456789"
                    },
                    InvoiceLines = new List<InvoiceLine> {
                        new InvoiceLine {
                            Currency = "GBP",
                            Description = "Test Description",
                            Value = 100,
                            SchemeCode = "123456789",
                            FundCode = "123456789",
                        }
                    }
                }
            }
        };

        //Act
        var response = await _invoiceValidator.TestValidateAsync(invoice);

        //Assert
        Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice Id is missing")) == 1);
        Assert.True(response.Errors[0].ErrorMessage == "Invoice Id is missing");
    }

    [Fact]
    public async Task Given_Invoice_When_InvoiceId_Characters_Are_Greater_Than_Twenty_Then_Failure_Message_InvoiceId_Must_Not_Be_More_Than_Twenty_Is_Thrown()
    {
        //Arrange
        Invoice invoice = new Invoice()
        {
            Id = "123456789ABCDEFGHIJKLMNOP",
            InvoiceType = "AP",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "123456789",
            SchemeType = "bps",
            PaymentType = "AP",
            CreatedBy = "Test User",
            Status = "status",
            PaymentRequests = new List<InvoiceHeader> {
                new InvoiceHeader {
                    PaymentRequestId = "123456789",
                    SourceSystem = "Manual",
                    MarketingYear = 2023,
                    DeliveryBody = "Test Org",
                    PaymentRequestNumber = 123456789,
                    AgreementNumber = "123456789",
                    ContractNumber = "123456789",
                    Value = 100,
                    DueDate = "2023-01-01",
                    FRN = 1000000000,
                    AppendixReferences = new AppendixReferences {
                        ClaimReferenceNumber = "123456789"
                    },
                    InvoiceLines = new List<InvoiceLine> {
                        new InvoiceLine {
                            Currency = "GBP",
                            Description = "Test Description",
                            Value = 100,
                            SchemeCode = "123456789",
                            FundCode = "123456789",
                        }
                    }
                }
            }
        };

        //Act
        var response = await _invoiceValidator.TestValidateAsync(invoice);

        //Assert
        Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice Id must not be more than 20 characters")) == 1);
        Assert.True(response.Errors[0].ErrorMessage == "Invoice Id must not be more than 20 characters");
    }

    [Fact]
    public async Task Given_Invoice_When_InvoiceId_Contain_No_Characters_Then_Fialure_Message_InvoiceId_Must_Be_Atleast_One_Character_Is_Thrown()
    {
        //Arrange
        Invoice invoice = new Invoice()
        {
            Id = "",
            InvoiceType = "AP",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "123456789",
            SchemeType = "bps",
            PaymentType = "AP",
            CreatedBy = "Test User",
            Status = "status",
            PaymentRequests = new List<InvoiceHeader> {
                new InvoiceHeader {
                    PaymentRequestId = "123456789",
                    SourceSystem = "Manual",
                    MarketingYear = 2023,
                    DeliveryBody = "Test Org",
                    PaymentRequestNumber = 123456789,
                    AgreementNumber = "123456789",
                    ContractNumber = "123456789",
                    Value = 100,
                    DueDate = "2023-01-01",
                    FRN = 1000000000,
                    AppendixReferences = new AppendixReferences {
                        ClaimReferenceNumber = "123456789"
                    },
                    InvoiceLines = new List<InvoiceLine> {
                        new InvoiceLine {
                            Currency = "GBP",
                            Description = "Test Description",
                            Value = 100,
                            SchemeCode = "123456789",
                            FundCode = "123456789",
                        }
                    }
                }
            }
        };

        //Act
        var response = await _invoiceValidator.TestValidateAsync(invoice);

        //Assert
        Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice Id must contain at least one character")) == 1);
        Assert.True(response.Errors[1].ErrorMessage == "Invoice Id must contain at least one character");
    }

    [Fact]
    public async Task Given_Invoice_When_InvoiceId_Contain_Spaces_Then_Failure_Message_InvoiceId_Cannot_Contain_Spaces_Is_Thrown()
    {
        //Arrange
        Invoice invoice = new Invoice()
        {
            Id = " SDEF ",
            InvoiceType = "AP",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "123456789",
            SchemeType = "bps",
            PaymentType = "AP",
            CreatedBy = "Test User",
            Status = "status",
            PaymentRequests = new List<InvoiceHeader> {
                new InvoiceHeader {
                    PaymentRequestId = "123456789",
                    SourceSystem = "Manual",
                    MarketingYear = 2023,
                    DeliveryBody = "Test Org",
                    PaymentRequestNumber = 123456789,
                    AgreementNumber = "123456789",
                    ContractNumber = "123456789",
                    Value = 100,
                    DueDate = "2023-01-01",
                    FRN = 1000000000,
                    AppendixReferences = new AppendixReferences {
                        ClaimReferenceNumber = "123456789"
                    },
                    InvoiceLines = new List<InvoiceLine> {
                        new InvoiceLine {
                            Currency = "GBP",
                            Description = "Test Description",
                            Value = 100,
                            SchemeCode = "123456789",
                            FundCode = "123456789",
                        }
                    }
                }
            }
        };

        //Act
        var response = await _invoiceValidator.TestValidateAsync(invoice);

        //Assert
        Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice Id cannot contain spaces")) == 1);
        Assert.True(response.Errors[0].ErrorMessage == "Invoice Id cannot contain spaces");
    }
}


    


