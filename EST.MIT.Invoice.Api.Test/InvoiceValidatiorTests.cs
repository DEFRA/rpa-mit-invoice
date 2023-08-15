using FluentValidation.TestHelper;
using Invoices.Api.Models;
using EST.MIT.Invoice.Api.Services.API.Interfaces;
using NSubstitute;
using EST.MIT.Invoice.Api.Services.API.Models;
using System.Net;

namespace Invoices.Api.Test;

public class InvoiceValidatiorTests
{
    private readonly IReferenceDataApi _referenceDataApiMock =
        Substitute.For<IReferenceDataApi>();

    private InvoiceValidator _invoiceValidator;

    public InvoiceValidatiorTests()
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
    public async Task Given_Invoice_When_InvoiceHeader_FRN_Is_Empty_Then_Invoice_Fails()
    {
        //Arrange
        Invoice invoice = new Invoice()
        {
            Id = "123456789",
            InvoiceType = "AP",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "123456789",
            SchemeType = "bps",
            PaymentType = "DOM",
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
                    AppendixReferences = new AppendixReferences {
                        ClaimReferenceNumber = "123456789"
                    },
                    InvoiceLines = new List<InvoiceLine> {
                        new InvoiceLine {
                            Currency = "GBP",
                            Description = "Test Description",
                            Value = 100,
                            SchemeCode = "123456789",
                            FundCode = "123456789"
                        }
                    }
                }

            }
        };

        //Act
        var response = await _invoiceValidator.TestValidateAsync(invoice);

        //Assert         
        Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("'FRN' must not be empty")) == 1);
    }

    [Fact]
    public async Task Given_Invoice_When_InvoiceHeader_AgreementNumber_Is_Empty_And_InvoiceLine_Description_Is_Empty_Then_Invoice_Fails()
    {
        //Arrange
        Invoice invoice = new Invoice()
        {
            Id = "123456789",
            InvoiceType = "AP",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "123456789",
            SchemeType = "bps",
            PaymentType = "DOM",
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
                    AppendixReferences = new AppendixReferences {
                        ClaimReferenceNumber = "123456789"
                    },
                    InvoiceLines = new List<InvoiceLine> {
                        new InvoiceLine {
                            Currency = "GBP",
                            Value = 100,
                            SchemeCode = "123456789",
                            FundCode = "123456789"
                        }
                    }
                }
            }
        };

        //Act
        var response = await _invoiceValidator.TestValidateAsync(invoice);

        //Assert       
        Assert.True(response.Errors[0].ErrorMessage.Equals("'Agreement Number' must not be empty."));
        Assert.True(response.Errors[1].ErrorMessage.Equals("'Description' must not be empty."));
        response.Errors.Count.Equals(2);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("NOT AP")]
    [InlineData("NOT AR")]
    [InlineData("12345")]
    public async Task Given_Invoice_When_AccountType_Is_Invalid_Then_Invoice_Fails(string? accountType)
    {
        //Arrange
        Invoice invoice = new Invoice()
        {
            Id = "123456789",
            InvoiceType = "AP",
            AccountType = accountType ?? "",
            Organisation = "Test Org",
            Reference = "123456789",
            SchemeType = "bps",
            PaymentType = "EU",
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

        //Act
        var response = await _invoiceValidator.TestValidateAsync(invoice);

        //Assert         
        Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Account Type is invalid. Should be AP or AR")) == 1);
    }

    [Fact]
    public async Task Given_Invoice_When_Organisation_And_InvoiceType_Is_Not_Empty_And_SchemeType_Is_Invalid_Then_Invoice_Fails()
    {
        //Arrange
        var errors = new Dictionary<string, List<string>>();
        var apiResponse = new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK, errors);
        var paymentSchemes = new List<PaymentScheme>()
        {
            new PaymentScheme()
            {
                Code = "abc"
            }
        };
        apiResponse.Data = paymentSchemes;

        var organisationErrors = new Dictionary<string, List<string>>();
        var organisationApiResponse = new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK, organisationErrors);
        var organisation = new List<Organisation>()
        {
            new Organisation()
            {
                Code= "Test Org"
            }
        };

        organisationApiResponse.Data = organisation;

        _referenceDataApiMock.GetOrganisationsAsync(Arg.Any<string>())
            .Returns(Task.FromResult(organisationApiResponse));

        _referenceDataApiMock
            .GetSchemeTypesAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(apiResponse));

        _invoiceValidator = new InvoiceValidator(_referenceDataApiMock);

        Invoice invoice = new Invoice()
        {
            Id = "123456789",
            InvoiceType = "AP",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "123456789",
            SchemeType = "bps",
            PaymentType = "DOM",
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

        //Act
        var response = await _invoiceValidator.TestValidateAsync(invoice);

        //Assert
        Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Scheme Type is invalid")) == 1);
    }


    [Fact]
    public async Task Given_Invoice_When_And_InvoiceType_Is_Not_Empty_And_Organisation_Is_Invalid_Then_Invoice_Fails()
    {
        //Arrange
        var errors = new Dictionary<string, List<string>>();
        var apiResponse = new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK, errors);
        var paymentSchemes = new List<PaymentScheme>()
        {
            new PaymentScheme()
            {
                Code = "bps"
            }
        };
        apiResponse.Data = paymentSchemes;

        var organisationErrors = new Dictionary<string, List<string>>();
        var organisationApiResponse = new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK, organisationErrors);
        var organisation = new List<Organisation>()
        {
            new Organisation()
            {
                Code= "org"
            }
        };

        organisationApiResponse.Data = organisation;

        _referenceDataApiMock.GetOrganisationsAsync(Arg.Any<string>())
            .Returns(Task.FromResult(organisationApiResponse));

        _referenceDataApiMock
            .GetSchemeTypesAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(apiResponse));

        _invoiceValidator = new InvoiceValidator(_referenceDataApiMock);


        Invoice invoice = new Invoice()
        {
            Id = "123456789",
            InvoiceType = "AP",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "123456789",
            SchemeType = "bps",
            PaymentType = "DOM",
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

        //Act
        var response = await _invoiceValidator.TestValidateAsync(invoice);

        //Assert 
        Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Organisation is Invalid")) == 1);
    }


    [Fact]
    public async Task Given_Invoice_When_Parent_And_Child_Data_Are_Valid_Then_Invoice_Pass()
    {
        //Arrange
        Invoice invoice = new Invoice()
        {
            Id = "123456789",
            InvoiceType = "AP",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "123456789",
            SchemeType = "bps",
            PaymentType = "DOM",
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

        //Act
        var response = await _invoiceValidator.TestValidateAsync(invoice);

        //Assert
        response.Errors.Count.Equals(0);
    }

    [Fact]
    public async Task Given_Invoice_When_SchemeType_Is_Valid_But_Repo_Returns_Nothing_Then_Invoice_Fails()
    {
        //Arrange
        var errors = new Dictionary<string, List<string>>();
        var paymentSchemesResponse = new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK, errors);
        var paymentSchemes = new List<PaymentScheme>();
        paymentSchemesResponse.IsSuccess = true;
        paymentSchemesResponse.Data = paymentSchemes;

        _referenceDataApiMock
            .GetSchemeTypesAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(paymentSchemesResponse));


        Invoice invoice = new Invoice()
        {
            Id = "123456789",
            InvoiceType = "AP",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "123456789",
            SchemeType = "BPS",
            PaymentType = "DOM",
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

        //Act
        var response = await _invoiceValidator.TestValidateAsync(invoice);

        //Assert         
        Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Scheme Type is invalid")) == 1);
    }

    [Fact]
    public async Task Given_Invoice_When_SchemeType_Is_Valid_But_Repo_Fails_Then_Invoice_Fails()
    {
        //Arrange
        var errors = new Dictionary<string, List<string>>();
        var paymentSchemesResponse = new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK, errors);
        var paymentSchemes = new List<PaymentScheme>()
        {
            new PaymentScheme()
            {
                Code = "bps"
            }
        };
        paymentSchemesResponse.IsSuccess = false;
        paymentSchemesResponse.Data = paymentSchemes;

        _referenceDataApiMock
            .GetSchemeTypesAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(paymentSchemesResponse));


        Invoice invoice = new Invoice()
        {
            Id = "123456789",
            InvoiceType = "AP",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "123456789",
            SchemeType = "bps",
            PaymentType = "DOM",
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

        //Act
        var response = await _invoiceValidator.TestValidateAsync(invoice);

        //Assert         
        Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Scheme Type is invalid")) == 1);
    }

    [Fact]
    public async Task Given_Invoice_When_PaymentType_Is_Valid_But_Repo_Fails_Then_Invoice_Fails()
    {
        //Arrange
        var errors = new Dictionary<string, List<string>>();
        var paymentTypesResponse = new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.OK, errors);
        var paymentTypes = new List<PaymentType>()
        {
            new PaymentType()
            {
                Code = "DOM"
            },
            new PaymentType()
            {
                Code = "EU"
            }
        };
        paymentTypesResponse.IsSuccess = false;
        paymentTypesResponse.Data = paymentTypes;

        _referenceDataApiMock
            .GetPaymentTypesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(paymentTypesResponse));


        Invoice invoice = new Invoice()
        {
            Id = "123456789",
            InvoiceType = "AP",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "123456789",
            SchemeType = "BPS",
            PaymentType = "DOM",
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

        //Act
        var response = await _invoiceValidator.TestValidateAsync(invoice);

        //Assert         
        Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Payment Type is invalid")) == 1);
    }

    [Fact]
    public async Task Given_Invoice_When_PaymentType_Is_Valid_But_Repo_Returns_Nothing_Then_Invoice_Fails()
    {
        //Arrange
        var errors = new Dictionary<string, List<string>>();
        var paymentTypesResponse = new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.OK, errors);
        var paymentTypes = new List<PaymentType>();
        paymentTypesResponse.IsSuccess = true;
        paymentTypesResponse.Data = paymentTypes;

        _referenceDataApiMock
            .GetPaymentTypesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(paymentTypesResponse));


        Invoice invoice = new Invoice()
        {
            Id = "123456789",
            InvoiceType = "AP",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "123456789",
            SchemeType = "BPS",
            PaymentType = "DOM",
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

        //Act
        var response = await _invoiceValidator.TestValidateAsync(invoice);

        //Assert         
        Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Payment Type is invalid")) == 1);
    }
}




