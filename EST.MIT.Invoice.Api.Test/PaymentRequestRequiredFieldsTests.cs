using Invoices.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace EST.MIT.Invoice.Api.Test
{
    public class PaymentRequestRequiredFieldsTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void Test_FRN_For_Value_Not_In_Range()
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AppendixReferences = new AppendixReferences(),
                ContractNumber = "ED34566",
                DueDate = DateTime.Now.ToString(),
                FRN = 1,
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 23456,
                        Currency = "£",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567",
                        DeliveryBody = "XYZ",
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                SourceSystem = "SOURCE SYSTEM",
                Value = 2345678.65M,
                FirmReferenceNumber = 1000000000
            };

            //Act
            var error = ValidateModel(paymentRequest);

            //Assert
            Assert.Single(error);
            Assert.NotNull(error.FirstOrDefault(x => x.ErrorMessage != null && x.ErrorMessage.Contains("FRN must be between 1000000000 and 9999999999")));
        }

        [Fact]
        public void Test_Marketing_Year_For_Value_Not_In_Range()
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AppendixReferences = new AppendixReferences(),
                ContractNumber = "ED34566",
                DueDate = DateTime.Now.ToString(),
                FRN = 1000000000,
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 23456,
                        Currency = "£",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567",
                        DeliveryBody = "RP00",
                    }
                },
                MarketingYear = 1996,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                SourceSystem = "SOURCE SYSTEM",
                Value = 2345678.65M
            };

            //Act
            var error = ValidateModel(paymentRequest);

            //Assert
            Assert.True(error.Count(x => x.ErrorMessage.Contains("Marketing Year must be between 2021 and 2099 ")) == 1);
        }

        [Fact]
        public void Test_Value_Field_For_Value_Not_In_Range()
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AppendixReferences = new AppendixReferences(),
                ContractNumber = "ED34566",
                DueDate = DateTime.Now.ToString(),
                FRN = 1000000000,
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 23456,
                        Currency = "£",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567",
                        DeliveryBody = "XYZ",
                    }
                },
                MarketingYear = 2021,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                SourceSystem = "SOURCE SYSTEM",
                Value = -2345678.65M
            };

            //Act
            var error = ValidateModel(paymentRequest);

            //Assert
            Assert.True(error.Count(x => x.ErrorMessage.Contains("Value must be between 0 and 999999999999.99")) == 1);
        }

        [Fact]
        public void Test_InvoiceHeader_For_Good_Data()
        {
            //Arrange
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                AppendixReferences = new AppendixReferences(),
                ContractNumber = "ED34566",
                DueDate = DateTime.Now.ToString(),
                FRN = 1000000000,
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 23456,
                        Currency = "£",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567",
                        DeliveryBody = "XYZ",
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                SourceSystem = "SOURCE SYSTEM",
                Value = 2345678.65M,
                FirmReferenceNumber = 1000000000
            };

            //Act
            var error = ValidateModel(paymentRequest);

            //Assert
            Assert.True(error.Count == 0);
        }
    }
}
