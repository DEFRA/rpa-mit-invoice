using FluentValidation.TestHelper;
using Invoices.Api.Models;

namespace EST.MIT.Invoice.Api.Test
{
    public class InvoiceLineValidatorTests
    {
        private InvoiceLineValidator _invoiceLineValidator;

        public InvoiceLineValidatorTests()
        {
            _invoiceLineValidator = new InvoiceLineValidator();
        }

        [Fact]
        public void Given_InvoiceLine_When_Value_Is_Empty_Then_InvoiceLine_Fails()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "£",
                Description = "Description",
                FundCode ="34ERTY6",
                SchemeCode ="DR5678"
            };

            //Act
            var response = _invoiceLineValidator.TestValidate(invoiceLine);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.Value);
            response.Errors.Count.Equals(1);
            //response.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Given_InvoiceLine_When_Description_Is_Empty_Then_InvoiceLine_Fails()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "£",
                Description = "Description",
                FundCode ="34ERTY6",
                SchemeCode ="DR5678",
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
                Currency = "£",
                Description = "Description",
                FundCode ="34ERTY6",
                SchemeCode = "DR5678",
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
                Currency = "£",
                Description = "Description",
                FundCode ="34ERTY6",
                SchemeCode ="DR5678",
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
            response.Errors.Count.Equals(0);
        }
    }
}
