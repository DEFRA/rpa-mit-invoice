using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using FluentValidation.TestHelper;
using Invoices.Api.Models;
using NSubstitute;
using System.Net;

namespace EST.MIT.Invoice.Api.Test
{
    public class InvoiceLineValidatorTests
    {
        private readonly InvoiceLineValidator _invoiceLineValidator;

        private readonly IReferenceDataApi _referenceDataApiMock =
            Substitute.For<IReferenceDataApi>();
        private readonly ICachedReferenceDataApi _cachedReferenceDataApiMock =
            Substitute.For<ICachedReferenceDataApi>();

        private readonly FieldsRoute _route = new()
        {
            PaymentType = "AP",
            InvoiceType = "AP",
            Organisation = "Test Org",
            SchemeType = "bps"
        };

        public InvoiceLineValidatorTests()
        {
            var schemeCodeErrors = new Dictionary<string, List<string>>();
            var schemeCodeResponse = new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.OK, schemeCodeErrors);

            var fundCodesErrors = new Dictionary<string, List<string>>();
            var fundCodeResponse = new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.OK, fundCodesErrors);

            var combinationsForRouteErrors = new Dictionary<string, List<string>>();
            var combinationsForRouteResponse = new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.OK, combinationsForRouteErrors);

            var schemeCodes = new List<SchemeCode>()
            {
                new SchemeCode()
                {
                    Code = "schemecodevalue"
                }
            };
            schemeCodeResponse.Data = schemeCodes;

            var fundCodes = new List<FundCode>()
            {
                new FundCode()
                {
                    Code = "34ERTY6"
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
            .GetSchemeCodesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(schemeCodeResponse));

            _referenceDataApiMock
            .GetFundCodesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(fundCodeResponse));

            _cachedReferenceDataApiMock.GetCombinationsListForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(combinationsForRouteResponse));

            _invoiceLineValidator = new InvoiceLineValidator(_referenceDataApiMock, _route, _cachedReferenceDataApiMock);
        }

        [Fact]
        public async Task Given_InvoiceLine_When_Value_Is_Empty_Then_InvoiceLine_Fails()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                MainAccount = "AccountCodeValue",
                DeliveryBody = "RP00"
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.Value);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public async Task Given_InvoiceLine_When_Description_Is_Empty_Then_InvoiceLine_Fails()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = 234.8M,
                MainAccount = "AccountCodeValue",
                DeliveryBody = "RP00"
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.Description);
            response.Errors.Count.Equals(2);
        }

        [Fact]
        public async Task Given_InvoiceLine_When_SchemeCode_Is_Empty_Then_InvoiceLine_Fails()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                Value = 234.8M,
                MainAccount = "AccountCodeValue",
                DeliveryBody = "RP00"
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.SchemeCode);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public async Task Given_InvoiceLine_When_SchemeCode_Is_Not_Valid_And_SchemeCode_Model_Is_Empty()
        {
            //Arrange
            var schemeCodeErrors = new Dictionary<string, List<string>>();
            var schemeCodeResponse = new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.OK, schemeCodeErrors);
            var schemeCodes = new List<SchemeCode>()
            {

            };
            schemeCodeResponse.Data = schemeCodes;

            _referenceDataApiMock
             .GetSchemeCodesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
             .Returns(Task.FromResult(schemeCodeResponse));


            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = 4567.89M,
                MainAccount = "AccountCodeValue",
                DeliveryBody = "RP00"
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.SchemeCode);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public async Task Given_InvoiceLine_When_No_Field_Is_Empty_Then_InvoiceLine_Pass()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = 4567.89M,
                MainAccount = "AccountCodeValue",
                DeliveryBody = "RP00",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.SchemeCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Description);
            response.ShouldNotHaveValidationErrorFor(x => x.FundCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Currency);
            response.ShouldNotHaveValidationErrorFor(x => x.MainAccount);
            response.ShouldNotHaveValidationErrorFor(x => x.DeliveryBody);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            Assert.Empty(response.Errors);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("NOT GBP")]
        [InlineData("NOT EUR")]
        [InlineData("12345")]
        public async Task Given_InvoiceLine_When_Currency_Is_Invalid_Then_InvoiceLine_Fails(string? currency)
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = currency ?? "",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = 4567.89M,
                MainAccount = "AccountCodeValue",
                DeliveryBody = "RP00",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.SchemeCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Description);
            response.ShouldNotHaveValidationErrorFor(x => x.FundCode);
            response.ShouldNotHaveValidationErrorFor(x => x.MainAccount);
            response.ShouldNotHaveValidationErrorFor(x => x.DeliveryBody);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Currency must be GBP or EUR")) == 1);
        }

        [Theory]
        [InlineData("GBP")]
        [InlineData("EUR")]
        public async Task Given_InvoiceLine_When_Currency_Is_Valid_Then_InvoiceLine_Passes(string currency)
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = currency,
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = 4567.89M,
                MainAccount = "AccountCodeValue",
                DeliveryBody = "RP00",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.SchemeCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Description);
            response.ShouldNotHaveValidationErrorFor(x => x.FundCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Currency);
            response.ShouldNotHaveValidationErrorFor(x => x.DeliveryBody);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            Assert.Empty(response.Errors);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(10.000)]
        [InlineData(10.100)]
        [InlineData(-10)]
        [InlineData(-10.000)]
        [InlineData(-10.100)]
        public async Task Given_InvoiceLine_When_Value_Has_Correct_Decimal_Places_Then_InvoiceLine_Passes(decimal value)
        {
            // this is because decimal places that have a value of 0 are not
            // counted unless they are followed by a non-zero value

            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = value,
                MainAccount = "AccountCodeValue",
                DeliveryBody = "RP00",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.SchemeCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Description);
            response.ShouldNotHaveValidationErrorFor(x => x.FundCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Currency);
            response.ShouldNotHaveValidationErrorFor(x => x.DeliveryBody);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            Assert.Empty(response.Errors);
        }

        [Theory]
        [InlineData(10.101)]
        [InlineData(10.1234)]
        [InlineData(10.00001)]
        public async Task Given_InvoiceLine_When_Value_Has_More_Than_2DP_Then_InvoiceLine_Fails(decimal value)
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = value,
                MainAccount = "AccountCodeValue",
                DeliveryBody = "RP00",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SchemeCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Description);
            response.ShouldNotHaveValidationErrorFor(x => x.FundCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Currency);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice line value cannot be more than 2dp")) == 1);
        }

        [Fact]
        public async Task Given_InvoiceLine_When_Value_Is_Equal_To_Zero_Then_InvoiceLine_Fails()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = 0,
                MainAccount = "AccountCodeValue",
                DeliveryBody = "RP00",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SchemeCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Description);
            response.ShouldNotHaveValidationErrorFor(x => x.FundCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Currency);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice line value must be non-zero")) == 1);
        }

        [Fact]
        public async Task Given_InvoiceLine_When_SchemeCode_Is_Valid_Then_InvoiceLine_Pass()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = 30,
                MainAccount = "AccountCodeValue",
                DeliveryBody = "RP00",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SchemeCode);
            Assert.Empty(response.Errors);
        }

        [Fact]
        public async Task Given_InvoiceLine_When_SchemeCode_Is_InValid_Then_InvoiceLine_Throws_Error_SchemeCode_Is_InValid()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "DR5699",
                Value = 30,
                MainAccount = "AccountCodeValue",
                DeliveryBody = "RP00",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert           
            Assert.True(response.Errors[0].ErrorMessage.Equals("SchemeCode is invalid"));
        }

        [Fact]
        public async Task Given_InvoiceLine_When_FundCode_Is_Valid_Then_InvoiceLine_Pass()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = 30,
                MainAccount = "AccountCodeValue",
                DeliveryBody = "RP00",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.FundCode);
            Assert.Empty(response.Errors);
        }

        [Fact]
        public async Task Given_InvoiceLine_When_FundCode_Is_InValid_Then_InvoiceLine_Throws_Error_FundCode_Is_InValid_For_This_Route()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTKK",
                SchemeCode = "schemecodevalue",
                Value = 30,
                MainAccount = "AccountCodeValue",
                DeliveryBody = "RP00",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert           
            Assert.True(response.Errors[0].ErrorMessage.Equals("Fund Code is invalid for this route"));
        }

        [Fact]
        public async Task Given_InvoiceLine_When_Fundcode_Is_InValid_And_Fund_Model_Is_Empty()
        {
            //Arrange
            var fundCodesErrors = new Dictionary<string, List<string>>();
            var fundCodeResponse = new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.OK, fundCodesErrors);

            var fundCodes = new List<FundCode>()
            {

            };
            fundCodeResponse.Data = fundCodes;

            _referenceDataApiMock
                .GetFundCodesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult(fundCodeResponse));

            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTKK",
                SchemeCode = "schemecodevalue",
                Value = 30,
                MainAccount = "AccountCodeValue",
                DeliveryBody = "RP00",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert           
            Assert.True(response.Errors[0].ErrorMessage.Equals("Fund Code is invalid for this route"));
        }

        [Fact]
        public async Task Given_InvoiceLine_When_MainAccount_Is_InValid_Then_InvoiceLine_Throws_Error_Account_Is_InValid_For_This_Route()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = 30,
                MainAccount = "AccountB",
                DeliveryBody = "RP00",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert           
            Assert.True(response.Errors[0].ErrorMessage.Equals("Account is Invalid for this route"));
        }

        [Fact]
        public async Task Given_InvoiceLine_When_DeliveryBody_Is_InValid_Then_InvoiceLine_Throws_Error_DeliveryBody_Is_InValid_For_This_Route()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = 30,
                MainAccount = "AccountCodeValue",
                DeliveryBody = "INVALIDDELIVERYBODY",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert           
            Assert.True(response.Errors[0].ErrorMessage.Equals("Delivery Body is invalid for this route"));
        }

        [Fact]
        public async Task Given_InvoiceLine_When_CombinationForRoute_Model_Is_Empty()
        {
            //Arrange
            var combinationsForRouteErrors = new Dictionary<string, List<string>>();
            var combinationsForRouteResponse = new ApiResponse<IEnumerable<CombinationForRoute>>(HttpStatusCode.OK, combinationsForRouteErrors);

            var combinationsForRoute = new List<CombinationForRoute>()
            {

            };
            combinationsForRouteResponse.Data = combinationsForRoute;

            _cachedReferenceDataApiMock.GetCombinationsListForRouteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromResult(combinationsForRouteResponse));

            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = 4567.89M,
                MainAccount = "AccountCodeValue",
                DeliveryBody = "RP00",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            Assert.True(response.Errors[0].ErrorMessage.Equals("Account is Invalid for this route"));
            Assert.True(response.Errors[1].ErrorMessage.Equals("Delivery Body is invalid for this route"));
        }

        [Fact]
        public async Task Given_InvoiceLine_Check_For_Valid_Combinations_When_Account_SchemeCode_DeliveryBody_Are_Valid_Then_InvoiceLine_Pass()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = 4567.89M,
                MainAccount = "accountcodevalue",
                DeliveryBody = "rp01",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            Assert.Empty(response.Errors);
        }

        [Fact]
        public async Task Given_InvoiceLine_Check_For_Valid_Combinations_When_Account_SchemeCode_Are_Valid_And_DeliveryBody_Is_Invalid_Then_InvoiceLine_Fails()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = 4567.89M,
                MainAccount = "accountcodevalue",
                DeliveryBody = "DELIVERYBODYINVALID",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            Assert.True(response.Errors[1].ErrorMessage.Equals("Account / Scheme / Delivery Body combination is invalid"));
        }

        [Fact]
        public async Task Given_InvoiceLine_Check_For_Valid_Combinations_When_DeliveryBody_SchemeCode_Are_Valid_And_MainAccount_Is_Invalid_Then_InvoiceLine_Fails()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = 4567.89M,
                MainAccount = "MAINACCOUNTINVALID",
                DeliveryBody = "rp01",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            Assert.True(response.Errors[1].ErrorMessage.Equals("Account / Scheme / Delivery Body combination is invalid"));
        }

        [Fact]
        public async Task Given_InvoiceLine_Check_For_Valid_Combinations_When_DeliveryBody_MainAccount_Are_Valid_And_SchemeCode_Is_Invalid_Then_InvoiceLine_Fails()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "SCHEMECODEINVALID",
                Value = 4567.89M,
                MainAccount = "accountcodevalue",
                DeliveryBody = "rp01",
                MarketingYear = 2023,
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            Assert.True(response.Errors[1].ErrorMessage.Equals("Account / Scheme / Delivery Body combination is invalid"));
        }

        [Fact]
        public async Task Given_InvoiceLine_Check_For_Valid_Combinations_When_FieldsRoute_Model_Is_Invalid_Then_InvoiceLine_Fail()
        {
            //Arrange
            FieldsRoute _inValidRoute = new()
            {
                InvoiceType = "AP",
                Organisation = "Test Org",
                SchemeType = "bps"
            };

            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = 4567.89M,
                MainAccount = "accountcodevalue",
                DeliveryBody = "rp01",
                MarketingYear = 2023,
            };

            var validator = new InvoiceLineValidator(_referenceDataApiMock, _inValidRoute, _cachedReferenceDataApiMock);

            //Act
            var response = await validator.TestValidateAsync(invoiceLine);

            //Assert
            Assert.True(response.Errors[2].ErrorMessage.Equals("Account / Scheme / Delivery Body combination is invalid"));
        }

        [Theory]
        [InlineData("", "", "", "")]
        [InlineData(null, null, null, null)]
        [InlineData("InvoiceType", "", "", "")]
        [InlineData("InvoiceType", "Organisation", "", "")]
        [InlineData("InvoiceType", "Organisation", "PaymentType", "")]
        [InlineData("", "Organisation", "", "")]
        [InlineData("", "Organisation", "PaymentType", "")]
        [InlineData("", "Organisation", "PaymentType", "SchemeType")]
        [InlineData("", "", "PaymentType", "")]
        [InlineData("", "", "PaymentType", "SchemeType")]
        [InlineData("InvoiceType", "", "PaymentType", "SchemeType")]
        [InlineData("", "", "", "SchemeType")]
        [InlineData("InvoiceType", "", "", "SchemeType")]
        [InlineData("InvoiceType", "Organisation", "", "SchemeType")]
        [InlineData("InvoiceType", "", "PaymentType", "")]
        [InlineData("", "Organisation", "", "SchemeType")]
        public async Task Given_InvoiceLine_When_Route_For_GetCombinationsForRouteList_Are_Empty_Then_It_Fails(string? invoiceType, string? organisation, string? paymentType, string? schemeType)
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "GBP",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "schemecodevalue",
                Value = 4567.89M,
                MainAccount = "AccountCodeValue",
                DeliveryBody = "RP00",
                MarketingYear = 2023,
            };

            var route = new FieldsRoute()
            {
                InvoiceType = invoiceType,
                Organisation = organisation,
                PaymentType = paymentType,
                SchemeType = schemeType
            };
            var validator = new InvoiceLineValidator(_referenceDataApiMock, route, _cachedReferenceDataApiMock);

            //Act

            var response = await validator.TestValidateAsync(invoiceLine);

            //Assert
            Assert.True(response.Errors[0].ErrorMessage.Equals("Account is Invalid for this route"));
            Assert.True(response.Errors[1].ErrorMessage.Equals("Delivery Body is invalid for this route"));
        }
    }
}
