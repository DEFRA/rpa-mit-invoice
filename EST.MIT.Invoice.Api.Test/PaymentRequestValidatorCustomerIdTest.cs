using System.Net;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using EST.MIT.Invoice.Api.Services.Api.Models;
using FluentValidation.TestHelper;
using EST.MIT.Invoice.Api.Models;
using NSubstitute;

namespace EST.MIT.Invoice.Api.Test
{
    public class PaymentRequestValidatorCustomerIdTest
    {
        private PaymentRequestValidator _paymentRequestValidator;

        private readonly IReferenceDataApi _referenceDataApiMock =
            Substitute.For<IReferenceDataApi>();

        private readonly ICachedReferenceDataApi _cachedReferenceDataApiMock =
            Substitute.For<ICachedReferenceDataApi>();

        private readonly FieldsRoute route = new()
        {
            PaymentType = "AP",
            AccountType = "AP",
            Organisation = "Test Org",
            SchemeType = "bps"
        };

        public PaymentRequestValidatorCustomerIdTest()
        {
            var schemeCodeErrors = new Dictionary<string, List<string>>();
            var fundCodeErrors = new Dictionary<string, List<string>>();
            var combinationsForRouteErrors = new Dictionary<string, List<string>>();
            var marketingYearErrors = new Dictionary<string, List<string>>();

            var schemeCodeResponse = new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.OK, schemeCodeErrors);
            var fundCodeResponse = new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.OK, fundCodeErrors);
            var marketingYearResponse = new ApiResponse<IEnumerable<MarketingYear>>(HttpStatusCode.OK, marketingYearErrors);
            var combinationsForRouteResponse = new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.OK, combinationsForRouteErrors);

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
                    Code ="2023"
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

            _cachedReferenceDataApiMock
                .GetSchemeCodesForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult(schemeCodeResponse));

            _cachedReferenceDataApiMock.GetFundCodesForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
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

            _cachedReferenceDataApiMock
                .GetMainAccountCodesForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult(mainAccountCodeResponse));

            _cachedReferenceDataApiMock.GetDeliveryBodyCodesForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult(deliveryBodyCodeResponse));

            _referenceDataApiMock
                .GetMarketingYearsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult(marketingYearResponse));

            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, "status");
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_All_Is_Ok_Then_InvoiceHeader_Passes()
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                SourceSystem = "4ADTRT",
                Currency = "GBP",
                DueDate = DateTime.Now.ToString(),
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 10M,
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "SchemeCodeValue",
                        MainAccount = "AccountCodeValue",
                        DeliveryBody = "RP00",
                        MarketingYear = 2023,
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 10M,
                FRN = 9999999999,
            };

            //Act
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            Assert.Empty(response.Errors);
        }

        [Theory]
        [InlineData(100000000, 1000000000, "100000")]
        [InlineData(0, 1000000000, "100000")]
        [InlineData(100000000, 0, "100000")]
        [InlineData(100000000, 1000000000, "")]
        [InlineData(0, 0, "")]
        [InlineData(0, 0, null)]
        public async Task Given_InvoiceHeader_When_SBI_FRN_And_Vendor_Supplied_Then_InvoiceHeader_Fails(int sbi, long frn, string vendor)
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                SourceSystem = "4ADTRT",
                Currency = "GBP",
                DueDate = DateTime.Now.ToString(),
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 10M,
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "SchemeCodeValue",
                        MainAccount = "AccountCodeValue",
                        DeliveryBody = "RP00",
                        MarketingYear = 2023,
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 10M,
                FRN = frn,
                SBI = sbi,
                Vendor = vendor,
            };

            //Act 
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);

            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice must only have SBI, FRN or Vendor")) == 1);
        }

        [Theory]
        [InlineData(10000000)]
        [InlineData(1000000000)]
        public async Task Given_InvoiceHeader_When_SBI_Is_Invalid_Then_InvoiceHeader_Fails(int sbi)
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                SourceSystem = "4ADTRT",
                Currency = "GBP",
                DueDate = DateTime.Now.ToString(),
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 10M,
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "SchemeCodeValue",
                        MainAccount = "AccountCodeValue",
                        DeliveryBody = "RP00",
                        MarketingYear = 2023,
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 10M,
                SBI = sbi,
            };

            //Act
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);

            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("SBI is not in valid range (105000000 .. 999999999)")) == 1);
        }

        [Theory]
        [InlineData(100000000)]
        [InlineData(10000000000)]
        public async Task Given_InvoiceHeader_When_FRN_Is_Invalid_Then_InvoiceHeader_Fails(long frn)
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                SourceSystem = "4ADTRT",
                Currency = "GBP",
                DueDate = DateTime.Now.ToString(),
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 10M,
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "SchemeCodeValue",
                        MainAccount = "AccountCodeValue",
                        DeliveryBody = "RP00",
                        MarketingYear = 2023,
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 10M,
                FRN = frn,
            };

            //Act
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);

            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("FRN is not in valid range (1000000000 .. 9999999999)")) == 1);
        }

        [Theory]
        [InlineData("10000")]
        [InlineData("1000000")]
        public async Task Given_InvoiceHeader_When_Vendor_Is_Invalid_Then_InvoiceHeader_Fails(string vendor)
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                SourceSystem = "4ADTRT",
                Currency = "GBP",
                DueDate = DateTime.Now.ToString(),
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 10M,
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "SchemeCodeValue",
                        MainAccount = "AccountCodeValue",
                        DeliveryBody = "RP00",
                        MarketingYear = 2023,
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 10M,
                Vendor = vendor,
            };

            //Act 
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);

            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Vendor must be 6 characters")) == 1);
        }
    }
}

