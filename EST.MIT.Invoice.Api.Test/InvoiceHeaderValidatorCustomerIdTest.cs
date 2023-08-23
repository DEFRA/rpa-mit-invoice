using FluentValidation.TestHelper;
using Invoices.Api.Models;
using Newtonsoft.Json.Linq;

namespace EST.MIT.Invoice.Api.Test
{
    public class InvoiceHeaderValidatorCustomerIdTest
    {
        private readonly InvoiceHeaderValidator _invoiceHeaderValidator;

        public InvoiceHeaderValidatorCustomerIdTest()
        {
            _invoiceHeaderValidator = new InvoiceHeaderValidator();
        }

        [Fact]
        public void Given_InvoiceHeader_When_All_Is_Ok_Then_InvoiceHeader_Passes()
        {
            //Arrange
            InvoiceHeader invoiceHeader = new InvoiceHeader()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                ContractNumber = "ED34566",
                DeliveryBody = "XYZ",
                SourceSystem = "4ADTRT",
                DueDate = DateTime.Now.ToString(),
                FRN = 1000000000,
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 10M,
                        Currency = "GBP",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567"
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 10M,
                FirmReferenceNumber = "1000000000",
            };

            //Act
            var response = _invoiceHeaderValidator.TestValidate(invoiceHeader);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.ContractNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.DeliveryBody);
            response.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            response.ShouldNotHaveValidationErrorFor(x => x.FRN);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            Assert.Empty(response.Errors);
        }

        [Fact]
        public void Given_InvoiceHeader_When_SBI_And_FRN_Supplied_Then_InvoiceHeader_Fails()
        {
            //Arrange
            InvoiceHeader invoiceHeader = new InvoiceHeader()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                ContractNumber = "ED34566",
                DeliveryBody = "XYZ",
                SourceSystem = "4ADTRT",
                DueDate = DateTime.Now.ToString(),
                FRN = 1000000000,
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 10M,
                        Currency = "GBP",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567"
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 10M,
                FirmReferenceNumber = "1000000000",
                SingleBusinessIdentifier = "100000000",
            };

            //Act
            var response = _invoiceHeaderValidator.TestValidate(invoiceHeader);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.ContractNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.DeliveryBody);
            response.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            response.ShouldNotHaveValidationErrorFor(x => x.FRN);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);

            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice must only have an SBI or FRN, not both")) == 1);
        }

        [Theory]
        [InlineData("10000000")]
        [InlineData("1000000000")]
        public void Given_InvoiceHeader_When_SBI_Is_Invalid_Then_InvoiceHeader_Fails(string sbi)
        {
            //Arrange
            InvoiceHeader invoiceHeader = new InvoiceHeader()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                ContractNumber = "ED34566",
                DeliveryBody = "XYZ",
                SourceSystem = "4ADTRT",
                DueDate = DateTime.Now.ToString(),
                FRN = 1000000000,
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 10M,
                        Currency = "GBP",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567"
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 10M,
                SingleBusinessIdentifier = sbi,
            };

            //Act
            var response = _invoiceHeaderValidator.TestValidate(invoiceHeader);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.ContractNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.DeliveryBody);
            response.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            response.ShouldNotHaveValidationErrorFor(x => x.FRN);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);

            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("SBI must be 9 characters long")) == 1);
        }

        [Theory]
        [InlineData("100000000")]
        [InlineData("10000000000")]
        public void Given_InvoiceHeader_When_FRN_Is_Invalid_Then_InvoiceHeader_Fails(string frn)
        {
            //Arrange
            InvoiceHeader invoiceHeader = new InvoiceHeader()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                ContractNumber = "ED34566",
                DeliveryBody = "XYZ",
                SourceSystem = "4ADTRT",
                DueDate = DateTime.Now.ToString(),
                FRN = 1000000000,
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 10M,
                        Currency = "GBP",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567"
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 10M,
                FirmReferenceNumber = frn,
            };

            //Act
            var response = _invoiceHeaderValidator.TestValidate(invoiceHeader);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.Value);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.ContractNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.DeliveryBody);
            response.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            response.ShouldNotHaveValidationErrorFor(x => x.FRN);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);

            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("FRN must be 10 characters long")) == 1);
        }
    }
}

