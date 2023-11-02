using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using EST.MIT.Invoice.Api.Services.Api.Models;
using FluentValidation.TestHelper;
using EST.MIT.Invoice.Api.Models;
using NSubstitute;
using System.Net;

namespace EST.MIT.Invoice.Api.Test;

public class BulkInvoiceDuplicateIdValidationTests
{
    private readonly BulkInvoiceValidator _bulkInvoiceValidator;

    private readonly IReferenceDataApi _referenceDataApiMock =
     Substitute.For<IReferenceDataApi>();

    private readonly ICachedReferenceDataApi _cachedReferenceDataApiMock =
        Substitute.For<ICachedReferenceDataApi>();

    public BulkInvoiceDuplicateIdValidationTests()
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
                Code = "AP"
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

        _bulkInvoiceValidator = new BulkInvoiceValidator(_referenceDataApiMock, _cachedReferenceDataApiMock);
    }

    [Fact]
    public async Task Given_BulkInvoice_When_Nested_Class_Invoice_PaymentRequests_Property_PaymentRequestId_Is_Duplicated_Then_Failure_Message_PaymentRequestId_Is_Duplicated_In_This_Batch()
    {
        //Arrange
        BulkInvoices bulkInvoices = new BulkInvoices()
        {
            Reference = "erty",
            SchemeType = "AP",

            Invoices = new List<PaymentRequestsBatch> {
                new PaymentRequestsBatch {
                    Id = "SDEF",
                    AccountType = "AP",
                    Organisation = "Test Org",
                    Reference = "123456789",
                    SchemeType = "bps",
                    PaymentType = "AP",
                    CreatedBy = "Test User",
                    Status = "status",
                    PaymentRequests = new List<PaymentRequest> {
                        new PaymentRequest {
                            PaymentRequestId = "123456789",
                            SourceSystem = "Manual",
                            MarketingYear = 2023,
                            PaymentRequestNumber = 123456789,
                            AgreementNumber = "123456789",
                            Currency = "GBP",
                            Value = 100,
                            DueDate = "2023-01-01",
                            AppendixReferences = new AppendixReferences {
                                ClaimReferenceNumber = "123456789"
                            },
                            InvoiceLines = new List<InvoiceLine> {
                                new InvoiceLine {
                                    Description = "Test Description",
                                    Value = 100,
                                    SchemeCode = "SchemeCodeValue",
                                    FundCode = "123456789",
                                    MainAccount = "AccountCodeValue",
                                    DeliveryBody = "RP00",
                                    MarketingYear = 2023,
                                }
                            },
                            FRN = 1000000000,
                        }
                    }
                },

                new PaymentRequestsBatch {
                    Id = "SDEF",
                    AccountType = "AP",
                    Organisation = "Test Org",
                    Reference = "123456789",
                    SchemeType = "bps",
                    PaymentType = "AP",
                    CreatedBy = "Test User",
                    Status = "status",
                    PaymentRequests = new List<PaymentRequest> {
                        new PaymentRequest {
                            PaymentRequestId = "123456789",
                            SourceSystem = "Manual",
                            MarketingYear = 2023,
                            PaymentRequestNumber = 123456789,
                            AgreementNumber = "123456789",
                            Currency = "GBP",
                            Value = 100,
                            DueDate = "2023-01-01",
                            AppendixReferences = new AppendixReferences {
                                ClaimReferenceNumber = "123456789"
                            },
                            InvoiceLines = new List<InvoiceLine> {
                                new InvoiceLine {
                                    Description = "Test Description",
                                    Value = 100,
                                    SchemeCode = "SchemeCodeValue",
                                    FundCode = "123456789",
                                    MainAccount = "AccountCodeValue",
                                    DeliveryBody = "RP00",
                                    MarketingYear = 2023,
                                }
                            },
                            FRN = 1000000000,
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

