﻿using EST.MIT.Invoice.Api.Services.API.Interfaces;
using EST.MIT.Invoice.Api.Services.API.Models;
using FluentValidation.TestHelper;
using Invoices.Api.Models;
using NSubstitute;
using System.Net;

namespace Invoices.Api.Test;

public class BulkInvoiceDuplicateIdValidationTests
{
    private readonly BulkInvoiceValidator _bulkInvoiceValidator;

    private readonly IReferenceDataApi _referenceDataApiMock =
     Substitute.For<IReferenceDataApi>();

    public BulkInvoiceDuplicateIdValidationTests()
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

        _bulkInvoiceValidator = new BulkInvoiceValidator(_referenceDataApiMock);
    }

    [Fact]
    public async Task Given_BulkInvoice_When_Nested_Class_Invoice_PaymentRequests_Property_PaymentRequestId_Is_Duplicated_Then_Failure_Message_PaymentRequestId__Is_Duplicated_In_This_Batch()
    {
        //Arrange
        BulkInvoices bulkInvoices = new BulkInvoices()
        {
            Reference = "erty",
            SchemeType = "AP",

            Invoices = new List<Invoice> {

                new Invoice {
                    Id = "SDEF",
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
                },

                new Invoice {
                    Id = "SDEF",
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
                }
            }
        };

        //Act
        var response = await _bulkInvoiceValidator.TestValidateAsync(bulkInvoices);

        //Assert
        Assert.True(response.Errors[0].ErrorMessage.Equals("Payment Request Id is duplicated in this batch"));
    }
}

