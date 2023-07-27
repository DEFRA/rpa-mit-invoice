﻿using FluentValidation.TestHelper;
using Invoices.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        var errors = new Dictionary<string, List<string>>();
        var response = new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK, errors);
        var paymentSchemes = new List<PaymentScheme>()
        {
            new PaymentScheme()
            {
                Code = "bps"
            }
        };
        response.Data = paymentSchemes;

        _referenceDataApiMock
            .GetSchemesAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(response));

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

        _referenceDataApiMock
            .GetSchemesAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(apiResponse));

        _invoiceValidator = new InvoiceValidator(_referenceDataApiMock);

        //Arrange
        Invoice invoice = new Invoice()
        {
            Id = "123456789",
            InvoiceType = "AP",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "123456789",
            SchemeType = "bps",
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
}





