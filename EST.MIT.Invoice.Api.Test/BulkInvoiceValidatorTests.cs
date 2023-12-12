using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using EST.MIT.Invoice.Api.Models;
using NSubstitute;
using System.Net;
using FluentAssertions;

namespace EST.MIT.Invoice.Api.Test
{
    public class BulkInvoiceValidatorTests
    {
        private readonly BulkInvoiceValidator _bulkInvoiceValidator;

        private readonly IReferenceDataApi _referenceDataApiMock = Substitute.For<IReferenceDataApi>();
        private readonly ICachedReferenceDataApi _cachedReferenceDataApiMock = Substitute.For<ICachedReferenceDataApi>();

        public BulkInvoiceValidatorTests()
        {
            var schemeTypeErrors = new Dictionary<string, List<string>>();
            var orgnisationErrors = new Dictionary<string, List<string>>();
            var payTypesErrors = new Dictionary<string, List<string>>();
            var schemeCodeErrors = new Dictionary<string, List<string>>();
            var fundCodeErrors = new Dictionary<string, List<string>>();
            var combinationsForRouteErrors = new Dictionary<string, List<string>>();
            var marketingYearErrors = new Dictionary<string, List<string>>();

            var schemeTypesResponse = new ApiResponse<IEnumerable<PaymentScheme>>(HttpStatusCode.OK, schemeTypeErrors);
            var organisationRespnse = new ApiResponse<IEnumerable<Organisation>>(HttpStatusCode.OK, orgnisationErrors);
            var paymentTypeResponse = new ApiResponse<IEnumerable<PaymentType>>(HttpStatusCode.OK, payTypesErrors);
            var schemeCodeResponse = new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.OK, schemeCodeErrors);
            var fundCodeResponse = new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.OK, fundCodeErrors);
            var marketingYearResponse = new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.OK, marketingYearErrors);
            var combinationsForRouteResponse = new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.OK, combinationsForRouteErrors);
            
            var schemeTypes = new List<PaymentScheme>()
            {
                new PaymentScheme()
                {
                    Code = "bps"
                }
            };
            schemeTypesResponse.Data = schemeTypes;

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
                    Code = "FUNDCODE"
                }
            };
            fundCodeResponse.Data = fundCodes;

            var marketingYears = new List<MarketingYear>()
            {
                new MarketingYear() {
                    Code ="2022"
                }
            };
            marketingYearResponse.Data = marketingYears;

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
                .Returns(Task.FromResult(schemeTypesResponse));

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
                .GetMarketingYearsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult(marketingYearResponse));

            _referenceDataApiMock
                .GetFundCodesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult(fundCodeResponse));

            _cachedReferenceDataApiMock
                .GetCombinationsListForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult(combinationsForRouteResponse));


            var mainAccountCodesErrors = new Dictionary<string, List<string>>();
            var mainAccountCodeResponse = new ApiResponse<IEnumerable<MainAccountCode>>(HttpStatusCode.OK, mainAccountCodesErrors);

            var deliveryBodyCodesErrors = new Dictionary<string, List<string>>();
            var deliveryBodyCodeResponse = new ApiResponse<IEnumerable<DeliveryBodyCode>>(HttpStatusCode.OK, deliveryBodyCodesErrors);

            var mainAccountCodes = new List<MainAccountCode>()
            {
                new MainAccountCode()
                {
                    Code = "AccountCodeValue"
                },
            };
            mainAccountCodeResponse.Data = mainAccountCodes;

            var deliveryBodyCodes = new List<DeliveryBodyCode>()
            {
                new DeliveryBodyCode()
                {
                    Code = "RP00"
                },
                new DeliveryBodyCode()
                {
                    Code = "RP01"
                }
            };
            deliveryBodyCodeResponse.Data = deliveryBodyCodes;

            _referenceDataApiMock
                .GetMainAccountCodesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult(mainAccountCodeResponse));

            _referenceDataApiMock
                .GetDeliveryBodyCodesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult(deliveryBodyCodeResponse));

            _bulkInvoiceValidator = new BulkInvoiceValidator(_referenceDataApiMock, _cachedReferenceDataApiMock);
        }

        [Fact]
        public async Task Given_Bulk_Invoices_When_Reference_Is_Empty_Then_Validation_Error_Is_Returned()
        {
            //Arrange
            var bulkInvoices = new BulkInvoices()
            {
                Reference = "",
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
                }
            };

            var result = await _bulkInvoiceValidator.ValidateAsync(bulkInvoices);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Reference");
        }

        [Fact]
        public async Task Given_Bulk_Invoices_When_SchemeType_Is_Empty_Then_Validation_Error_Is_Returned()
        {
            //Arrange
            var bulkInvoices = new BulkInvoices()
            {
                Reference = "bps",
                SchemeType = "",

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
                }
            };

            var result = await _bulkInvoiceValidator.ValidateAsync(bulkInvoices);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "SchemeType");
        }

        [Fact]
        public async Task Given_Bulk_Invoices_When_Invoices_Are_Empty_Then_Validation_Error_Is_Returned()
        {
            //Arrange
            var bulkInvoices = new BulkInvoices()
            {
                Reference = "bps",
                SchemeType = "AP",

                Invoices = new List<PaymentRequestsBatch>()
            };

            var result = await _bulkInvoiceValidator.ValidateAsync(bulkInvoices);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Invoices");
        }
    }
}

