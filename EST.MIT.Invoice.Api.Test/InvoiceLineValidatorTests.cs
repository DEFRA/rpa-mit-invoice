using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using EST.MIT.Invoice.Api.Services.Api.Models;
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

        private readonly FieldsRoute route = new()
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


            var schemeCodes = new List<SchemeCode>()
            {
                new SchemeCode()
                {
                    Code = "DR5678"
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

            _referenceDataApiMock
            .GetSchemeCodesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(schemeCodeResponse));

            _referenceDataApiMock
            .GetFundCodesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(fundCodeResponse));

            _invoiceLineValidator = new InvoiceLineValidator(_referenceDataApiMock, route);
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
                SchemeCode = "DR5678"
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
                SchemeCode = "DR5678",
                Value = 234.8M
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
                Value = 234.8M
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
                SchemeCode = "DR5678",
                Value = 4567.89M
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.SchemeCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Description);
            response.ShouldNotHaveValidationErrorFor(x => x.FundCode);
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
        public async Task Given_InvoiceLine_When_Currency_Is_Invalid_Then_InvoiceLine_Fails(string? currency)
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = currency ?? "",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "DR5678",
                Value = 4567.89M
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.SchemeCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Description);
            response.ShouldNotHaveValidationErrorFor(x => x.FundCode);
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
                SchemeCode = "DR5678",
                Value = 4567.89M
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.SchemeCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Description);
            response.ShouldNotHaveValidationErrorFor(x => x.FundCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Currency);
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
                SchemeCode = "DR5678",
                Value = value
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.SchemeCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Description);
            response.ShouldNotHaveValidationErrorFor(x => x.FundCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Currency);
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
                SchemeCode = "DR5678",
                Value = value
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SchemeCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Description);
            response.ShouldNotHaveValidationErrorFor(x => x.FundCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Currency);
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
                SchemeCode = "DR5678",
                Value = 0
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SchemeCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Description);
            response.ShouldNotHaveValidationErrorFor(x => x.FundCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Currency);
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
                SchemeCode = "DR5678",
                Value = 30
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
                Value = 30
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
                SchemeCode = "DR5678",
                Value = 30
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
                SchemeCode = "DR5678",
                Value = 30
            };

            //Act
            var response = await _invoiceLineValidator.TestValidateAsync(invoiceLine);

            //Assert           
            Assert.True(response.Errors[0].ErrorMessage.Equals("Fund Code is invalid for this route"));
        }
    }
}
