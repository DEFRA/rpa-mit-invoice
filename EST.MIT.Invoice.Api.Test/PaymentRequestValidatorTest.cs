using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using EST.MIT.Invoice.Api.Models;
using NSubstitute;
using System.Net;
using FluentValidation.TestHelper;

namespace EST.MIT.Invoice.Api.Test
{
    public class PaymentRequestValidatorTest
    {
        private PaymentRequestValidator _paymentRequestValidator;

        private readonly IReferenceDataApi _referenceDataApiMock = Substitute.For<IReferenceDataApi>();
        private readonly ICachedReferenceDataApi _cachedReferenceDataApiMock = Substitute.For<ICachedReferenceDataApi>();

        private FieldsRoute route = new()
        {
            PaymentType = "DOM",
            AccountType = "AR",
            Organisation = "Test Org",
            SchemeType = "bps"
        };

        public PaymentRequestValidatorTest()
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

            _cachedReferenceDataApiMock
                .GetDeliveryBodyCodesForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult(deliveryBodyCodeResponse));

            _referenceDataApiMock
                 .GetMarketingYearsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                 .Returns(Task.FromResult(marketingYearResponse));

            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, "status");
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_AgreementNumber_Is_Null_Then_InvoiceHeader_Fails()
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AppendixReferences = new AppendixReferences(),
                DueDate = DateTime.Now.ToString(),
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 23456,
                        Currency = "GBP",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567",
                        MainAccount = "AccountA",
                        DeliveryBody = "RP00",
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                SourceSystem = "SOURCE SYSTEM",
                Value = 2345678.65M
            };

            //Act
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.AgreementNumber);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_SourceSystem_Is_Empty_Then_InvoiceHeader_Fails()
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                DueDate = DateTime.Now.ToString(),
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 23456,
                        Currency = "GBP",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567",
                        MainAccount = "AccountA",
                        DeliveryBody = "XYZ",
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 2345678.65M
            };

            //Act
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.SourceSystem);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_PaymentRequestNumber_Is_Empty_Then_InvoiceHeader_Fails()
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                DueDate = DateTime.Now.ToString(),
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 23456,
                        Currency = "GBP",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567",
                        MainAccount = "AccountA",
                        DeliveryBody = "XYZ",
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                Value = 2345678.65M
            };

            //Act
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_DueDate_Is_Empty_Then_InvoiceHeader_Fails()
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 23456,
                        Currency = "GBP",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567",
                        MainAccount = "AccountA",
                        DeliveryBody = "XYZ",

                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 2345678.65M
            };

            //Act
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.DueDate);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_No_Field_Is_Empty_Then_InvoiceHeader_Pass()
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                Vendor = "1",
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                SourceSystem = "4ADTRT",
                DueDate = DateTime.Now.ToString(),
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 2345678.65M,
                        Currency = "GBP",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "SchemeCodeValue",
                        MainAccount = "AccountCodeValue",
                        DeliveryBody = "rp00",
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 2345678.65M
            };

            //Act
            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, InvoiceStatuses.Approved);
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
            response.Errors.Count.Equals(0);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(10.000)]
        [InlineData(10.100)]
        [InlineData(-10)]
        [InlineData(-10.000)]
        [InlineData(-10.100)]
        public async Task Given_InvoiceHeader_When_Value_Has_Correct_Decimal_Places_Then_InvoiceHeader_Passes(decimal value)
        {
            // this is because decimal places that have a value of 0 are not
            // counted unless they are followed by a non-zero value

            //Arrange
            route = new() { AccountType = "AP", Organisation = "Test Org", SchemeType = "bps", PaymentType = "DOM" };

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
                        Value = value,
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
                Value = value,
                FRN = 1000000000,
            };

            //Act
            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, InvoiceStatuses.Approved);
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            Assert.Empty(response.Errors);
        }

        [Theory]
        [InlineData(10.101)]
        [InlineData(10.1234)]
        [InlineData(10.00001)]
        public async Task Given_InvoiceHeader_When_Value_Has_More_Than_2DP_Then_InvoiceHeader_Fails(decimal value)
        {
            //Arrange
            route = new() { AccountType = "AP", Organisation = "Test Org", SchemeType = "bps", PaymentType = "DOM" };

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
                        Value = value,
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
                Value = value,
                FRN = 1000000000,
            };

            //Act
            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, InvoiceStatuses.Approved);
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);

            Assert.Equal(2, response.Errors.Count);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice value cannot be more than 2dp")) == 1);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice line value cannot be more than 2dp")) == 1);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_Value_Is_Equal_To_Zero_Then_InvoiceHeader_Fails()
        {
            //Arrange
            route = new() { AccountType = "AP", Organisation = "Test Org", SchemeType = "bps", PaymentType = "DOM" };

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
                        Value = 0,
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
                Value = 0,
                FRN = 1000000000,
            };

            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, InvoiceStatuses.Approved);

            //Act
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            Assert.Equal(2, response.Errors.Count);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice value must be non-zero")) == 1);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice line value must be non-zero")) == 1);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_The_Status_Field_Is_PendingApproval_Or_Approved_And_Invoice_Value_IsGreaterThan_Zero_And_InvoiceLines_IsNot_Empty_Then_InvoiceHeader_Pass()
        {
            //Arrange
            route = new() { AccountType = "AP", Organisation = "Test Org", SchemeType = "bps", PaymentType = "DOM" };

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
                        Value = 1.2M,
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
                Value = 1.2M,
                FRN = 1000000000
            };

            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, InvoiceStatuses.AwaitingApproval);

            //Act
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            Assert.Empty(response.Errors);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_The_Status_Field_Is_PendingApproval_Or_Approved_And_Invoice_Value_Is_Zero_And_InvoiceLines_IsNot_Empty_Then_InvoiceHeader_Fail()
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                SourceSystem = "4ADTRT",
                DueDate = DateTime.Now.ToString(),
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 20M,
                        Currency = "GBP",
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
                Value = 0M,
                FRN = 1000000000
            };

            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, InvoiceStatuses.AwaitingApproval);

            //Act
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.Value);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_The_Status_Field_Is_PendingApproval_Or_Approved_And_Invoice_Value_IsNot_Zero_And_InvoiceLines_Is_Empty_Then_InvoiceHeader_Fail()
        {
            //Arrange
            route = new() { AccountType = "AP", Organisation = "Test Org", SchemeType = "bps", PaymentType = "DOM" };

            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                SourceSystem = "4ADTRT",
                Currency = "GBP",
                DueDate = DateTime.Now.ToString(),
                InvoiceLines = new List<InvoiceLine>(),
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 20M,
                FRN = 1000000000
            };

            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, InvoiceStatuses.AwaitingApproval);

            //Act
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.InvoiceLines);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("'Invoice Lines' must not be empty.")) == 1);
            Assert.Single(response.Errors);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_The_Status_Field_Is_PendingApproval_Or_Approved_And_Invoice_Value_Is_Zero_And_InvoiceLines_Is_Empty_Then_InvoiceHeader_Fail()
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                SourceSystem = "4ADTRT",
                DueDate = DateTime.Now.ToString(),
                InvoiceLines = new List<InvoiceLine>(),
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 0M,
                FRN = 1000000000
            };

            //Act
            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, InvoiceStatuses.AwaitingApproval);
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldHaveValidationErrorFor(x => x.Value);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("'Invoice Lines' must not be empty.")) == 1);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice value must be non-zero")) == 1);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_AccountType_Is_AR_And_OriginalInvoiceNumber_OriginalSettlementDate_RecoveryDate_Properties_Are_Null_Then_InvoiceHeader_Fails()
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
                        Value = 1.2M,
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
                Value = 1.2M,
                FRN = 1000000000
            };

            //Act
            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, InvoiceStatuses.AwaitingApproval);
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.OriginalInvoiceNumber);
            response.ShouldHaveValidationErrorFor(x => x.OriginalSettlementDate);
            response.ShouldHaveValidationErrorFor(x => x.RecoveryDate);

            Assert.Equal(3, response.Errors.Count);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Please input Original AP Reference")) == 1);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Please input Original AP Settlement Date")) == 1);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Please input earliest date possible recovery identified")) == 1);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_AccountType_Is_AR_And_OriginalInvoiceNumber_OriginalSettlementDate_RecoveryDate_Properties_Are_Given_Then_InvoiceHeader_Pass()
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
                        Value = 1.2M,
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
                Value = 1.2M,
                FRN = 1000000000,
                RecoveryDate = DateTime.Now,
                OriginalSettlementDate = DateTime.Now,
                OriginalInvoiceNumber = "45RTFGR"
            };

            //Act
            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, InvoiceStatuses.AwaitingApproval);
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldNotHaveAnyValidationErrors();
            Assert.Empty(response.Errors);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_AccountType_IsNot_AR__And_OriginalInvoiceNumber_OriginalSettlementDate_RecoveryDate_Properties_Are_Given_Then_InvoiceHeader_Pass()
        {
            //Arrange
            route = new() { AccountType = "AP", Organisation = "Test Org", SchemeType = "bps", PaymentType = "DOM" };

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
                        Value = 1.2M,
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
                Value = 1.2M,
                FRN = 1000000000,
                RecoveryDate = DateTime.Now,
                OriginalSettlementDate = DateTime.Now,
                OriginalInvoiceNumber = "45RTFGR"
            };

            //Act
            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, InvoiceStatuses.AwaitingApproval);
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldNotHaveAnyValidationErrors();
            Assert.Empty(response.Errors);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_AccountType_Is_AR_And_OriginalInvoiceNumber_IsNull_And_OriginalSettlementDate_RecoveryDate_Properties_Are_Given_Then_InvoiceHeader_Fail()
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
                        Value = 1.2M,
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
                Value = 1.2M,
                FRN = 1000000000,
                RecoveryDate = DateTime.Now,
                OriginalSettlementDate = DateTime.Now
            };

            //Act
            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, InvoiceStatuses.AwaitingApproval);
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.OriginalInvoiceNumber);
            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Please input Original AP Reference")) == 1);
        }

        [Theory]
        [InlineData("GBP")]
        [InlineData("EUR")]
        public async Task Given_PaymentRequest_When_Currency_Is_Valid_Then_PaymentRequest_Passes(string? currency)
        {
            //Arrange
            route = new() { AccountType = "AP", Organisation = "Test Org", SchemeType = "bps", PaymentType = "DOM" };

            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                SourceSystem = "4ADTRT",
                Currency = currency,
                DueDate = DateTime.Now.ToString(),
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 0,
                FRN = 1000000000,
            };

            //Act
            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, "new");
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldNotHaveValidationErrorFor(x => x.Currency);
            Assert.Empty(response.Errors);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("NOT GBP")]
        [InlineData("NOT EUR")]
        [InlineData("12345")]
        public async Task Given_PaymentRequest_When_Currency_Is_Invalid_Then_PaymentRequest_Fails(string? currency)
        {
            //Arrange
            route = new() { AccountType = "AP", Organisation = "Test Org", SchemeType = "bps", PaymentType = "DOM" };

            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                SourceSystem = "4ADTRT",
                Currency = currency,
                DueDate = DateTime.Now.ToString(),
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 0,
                FRN = 1000000000,
            };

            //Act
            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, "new");
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Currency must be GBP or EUR")) == 1);
        }

        [Theory]
        [InlineData(10, 5, 4)]
        [InlineData(10, 3, 6)]
        [InlineData(10, -5, -5)]
        [InlineData(10, -5, 5)]
        public async Task Given_InvoiceHeader_When_Value_Does_Not_Equal_Sum_Of_InvoiceLines_Then_InvoiceHeader_Fails(decimal invoiceValue, decimal invoiceLine1Value, decimal invoiceLine2Value)
        {
            //Arrange
            route = new() { AccountType = "AP", Organisation = "Test Org", SchemeType = "bps", PaymentType = "DOM" };

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
                        Value = invoiceLine1Value,
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "SchemeCodeValue",
                        MainAccount = "AccountCodeValue",
                        DeliveryBody = "RP00",
                        MarketingYear = 2023,
                    },
                    new InvoiceLine()
                    {
                        Value = invoiceLine2Value,
                        Currency = "GBP",
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
                Value = invoiceValue,
                FRN = 1000000000,
            };

            //Act
            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, InvoiceStatuses.AwaitingApproval);
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);

            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains($"Invoice Value ({invoiceValue}) does not equal the sum of Line Values ({invoiceLine1Value + invoiceLine2Value}")) == 1);
        }

        [Theory]
        [InlineData(1000000000, 999999999, 1)]
        [InlineData(1000000000, 3, 999999997)]
        [InlineData(-1000000000, 1, -1000000001)]
        public async Task Given_InvoiceHeader_When_Absolute_Value_Is_Not_Less_Than_1_Billion_Then_InvoiceHeader_Fails(decimal invoiceValue, decimal invoiceLine1Value, decimal invoiceLine2Value)
        {
            //Arrange
            route = new() { AccountType = "AP", Organisation = "Test Org", SchemeType = "bps", PaymentType = "DOM" };

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
                        Value = invoiceLine1Value,
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "SchemeCodeValue",
                        MainAccount = "AccountCodeValue",
                        DeliveryBody = "RP00",
                        MarketingYear = 2023,
                    },
                    new InvoiceLine()
                    {
                        Value = invoiceLine2Value,
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
                Value = invoiceValue,
                FRN = 1000000000,
            };

            //Act
            _paymentRequestValidator = new PaymentRequestValidator(_referenceDataApiMock, _cachedReferenceDataApiMock, route, InvoiceStatuses.Approved);
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);

            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("The ABS invoice value must be less than 1 Billion")) == 1);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_PaymentRequestId_Is_Null_Or_Empty_Then_Failure_Message_PaymentRequestId_Is_Missing_Is_Thrown()
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                PaymentRequestId = string.Empty,
                SourceSystem = "Manual",
                MarketingYear = 2023,
                PaymentRequestNumber = 123456789,
                AgreementNumber = "123456789",
                Value = 100,
                DueDate = "2023-01-01",
                AppendixReferences = new AppendixReferences
                {
                    ClaimReferenceNumber = "123456789"
                },

                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine
                    {
                        Currency = "GBP",
                        Description = "Test Description",
                        Value = 100,
                        SchemeCode = "123456789",
                        FundCode = "123456789",
                        MainAccount = "AccountCodeValue",
                        DeliveryBody = "RP00",
                    }
                }
            };

            //Act
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("PaymentRequestId is missing")) == 1);
            Assert.True(response.Errors[0].ErrorMessage == "PaymentRequestId is missing");
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_PaymentRequestId_Characters_Are_Greater_Than_Twenty_Then_Failure_Message_PaymentRequestId_Must_Not_Be_More_Than_Twenty_Is_Thrown()
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                PaymentRequestId = "123456789ABCDEFGHIJKLMNOP",
                SourceSystem = "Manual",
                MarketingYear = 2023,
                PaymentRequestNumber = 123456789,
                AgreementNumber = "123456789",
                Value = 100,
                DueDate = "2023-01-01",
                AppendixReferences = new AppendixReferences
                {
                    ClaimReferenceNumber = "123456789"
                },
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine
                    {
                        Currency = "GBP",
                        Description = "Test Description",
                        Value = 100,
                        SchemeCode = "123456789",
                        FundCode = "123456789",
                        MainAccount = "AccountCodeValue",
                        DeliveryBody = "RP00",
                    }
                }
            };

            //Act
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("PaymentRequestId must not be more than 20 characters")) == 1);
            Assert.True(response.Errors[0].ErrorMessage == "PaymentRequestId must not be more than 20 characters");
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_PaymentRequestId_Contain_No_Characters_Then_Fialure_Message_PaymentRequestId_Must_Be_Atleast_One_Character_Is_Thrown()
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                PaymentRequestId = "",
                SourceSystem = "Manual",
                MarketingYear = 2023,
                PaymentRequestNumber = 123456789,
                AgreementNumber = "123456789",
                Value = 100,
                DueDate = "2023-01-01",
                AppendixReferences = new AppendixReferences
                {
                    ClaimReferenceNumber = "123456789"
                },
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine
                    {
                        Currency = "GBP",
                        Description = "Test Description",
                        Value = 100,
                        SchemeCode = "123456789",
                        FundCode = "123456789",
                        MainAccount = "AccountCodeValue",
                        DeliveryBody = "RP00",
                    }
                }
            };

            //Act
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert            
            Assert.True(response.Errors[1].ErrorMessage == "PaymentRequestId must contain at least one character");
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_PaymentRequestId_Contain_Spaces_Then_Failure_Message_PaymentRequestIsId_Cannot_Contain_Spaces_Is_Thrown()
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                PaymentRequestId = " S DEF ",
                SourceSystem = "Manual",
                MarketingYear = 2023,
                PaymentRequestNumber = 123456789,
                AgreementNumber = "123456789",
                Value = 100,
                DueDate = "2023-01-01",
                AppendixReferences = new AppendixReferences
                {
                    ClaimReferenceNumber = "123456789"
                },
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine
                    {
                        Currency = "GBP",
                        Description = "Test Description",
                        Value = 100,
                        SchemeCode = "123456789",
                        FundCode = "123456789",
                        MainAccount = "AccountCodeValue",
                        DeliveryBody = "RP00",
                    }
                }
            };

            //Act
            var response = await _paymentRequestValidator.TestValidateAsync(paymentRequest);

            //Assert
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("PaymentRequestId cannot contain spaces")) == 1);
            Assert.True(response.Errors[0].ErrorMessage == "PaymentRequestId cannot contain spaces");
        }
    }
}

