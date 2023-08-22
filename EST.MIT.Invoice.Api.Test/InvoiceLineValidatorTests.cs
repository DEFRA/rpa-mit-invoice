using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.API.Interfaces;
using FluentValidation.TestHelper;
using Invoices.Api.Models;
using NSubstitute;

namespace EST.MIT.Invoice.Api.Test
{
    public class InvoiceLineValidatorTests
    {
        private readonly InvoiceLineValidator _invoiceLineValidator;

        private readonly IReferenceDataApi _referenceDataApiMock =
     Substitute.For<IReferenceDataApi>();

        private SchemeCodeRoute route = new SchemeCodeRoute()
        {
            PaymentType = "AP",
            InvoiceType = "AP",
            Organisation = "Test Org",
            SchemeType = "bps"
        };

        public InvoiceLineValidatorTests()
        {
            _invoiceLineValidator = new InvoiceLineValidator(_referenceDataApiMock,route );
        }

        [Fact]
        public void Given_InvoiceLine_When_Value_Is_Empty_Then_InvoiceLine_Fails()
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
            var response = _invoiceLineValidator.TestValidate(invoiceLine);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.Value);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public void Given_InvoiceLine_When_Description_Is_Empty_Then_InvoiceLine_Fails()
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
            var response = _invoiceLineValidator.TestValidate(invoiceLine);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.Description);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public void Given_InvoiceLine_When_SchemeCode_Is_Empty_Then_InvoiceLine_Fails()
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
            var response = _invoiceLineValidator.TestValidate(invoiceLine);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.SchemeCode);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public void Given_InvoiceLine_When_No_Field_Is_Empty_Then_InvoiceLine_Pass()
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
            var response = _invoiceLineValidator.TestValidate(invoiceLine);

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
        public void Given_InvoiceLine_When_Currency_Is_Invalid_Then_InvoiceLine_Fails(string? currency)
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
            var response = _invoiceLineValidator.TestValidate(invoiceLine);

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
        public void Given_InvoiceLine_When_Currency_Is_Valid_Then_InvoiceLine_Passes(string currency)
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
            var response = _invoiceLineValidator.TestValidate(invoiceLine);

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
        public void Given_InvoiceLine_When_Value_Has_Correct_Decimal_Places_Then_InvoiceLine_Passes(decimal value)
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
            var response = _invoiceLineValidator.TestValidate(invoiceLine);

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
        public void Given_InvoiceLine_When_Value_Has_More_Than_2DP_Then_InvoiceLine_Fails(decimal value)
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
            var response = _invoiceLineValidator.TestValidate(invoiceLine);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SchemeCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Description);
            response.ShouldNotHaveValidationErrorFor(x => x.FundCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Currency);
            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice line value cannot be more than 2dp")) == 1);
        }

        [Fact]
        public void Given_InvoiceLine_When_Value_Is_Equal_To_Zero_Then_InvoiceLine_Fails()
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
            var response = _invoiceLineValidator.TestValidate(invoiceLine);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SchemeCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Description);
            response.ShouldNotHaveValidationErrorFor(x => x.FundCode);
            response.ShouldNotHaveValidationErrorFor(x => x.Currency);
            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice line value must be non-zero")) == 1);
        }
    }
}
