using Invoices.Api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EST.MIT.Invoice.Api.Test
{
    public class InvoiceLineRequiredFieldsTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void Test_Value_Field_For_Value_Not_In_Range()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "£",
                Description = "Description",
                FundCode = "34ERTY6",
                SchemeCode = "DR5678",
                Value = -4567.89M
            };

            //Act
            var error = ValidateModel(invoiceLine);

            //Assert
            Assert.True(error.Count(x => x.ErrorMessage.Contains("Value must be between 0 and 999999999999.99")) == 1);
        }

        [Fact]
        public void Test_Description_For_Null()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "£",
                FundCode = "34ERTY6",
                SchemeCode = "DR5678",
                Value = 4567.89M
            };

            //Act
            var error = ValidateModel(invoiceLine);

            //Assert
            Assert.True(error.Count(x => x.ErrorMessage.Contains("Description must be stated")) == 1);
        }

        [Fact]
        public void Test_SchemeCode_For_Null()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "£",
                FundCode = "34ERTY6",
                Value = 4567.89M
            };

            //Act
            var error = ValidateModel(invoiceLine);

            //Assert
            Assert.True(error.Count(x => x.ErrorMessage.Contains("SchemeCode must be specified")) == 1);
        }

        [Fact]
        public void Test_InvoiceLine_For_Good_Data()
        {
            //Arrange
            InvoiceLine invoiceLine = new InvoiceLine()
            {
                Currency = "£",
                FundCode = "34ERTY6",
                Value = 4567.89M,
                SchemeCode = "4RT567",
                Description = "Description"
            };

            //Act
            var error = ValidateModel(invoiceLine);

            //Assert
            Assert.True(error.Count == 0);
        }
    }
}
