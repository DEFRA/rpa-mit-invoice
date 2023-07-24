using FluentValidation.TestHelper;
using Invoices.Api.Models;

namespace EST.MIT.Invoice.Api.Test
{
    public class InvoiceHeaderValidatorTest
    {
        private readonly InvoiceHeaderValidator _invoiceHeaderValidator;

        public InvoiceHeaderValidatorTest()
        {
            _invoiceHeaderValidator = new InvoiceHeaderValidator();
        }

        [Fact]
        public void Given_InvoiceHeader_When_AgreementNumber_Is_Null_Then_InvoiceHeader_Fails()
        {
            //Arrange
            InvoiceHeader invoiceHeader = new InvoiceHeader()
            {
                AppendixReferences = new AppendixReferences(),
                ContractNumber = "ED34566",
                DeliveryBody = "XYZ",
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
                        SchemeCode = "WE4567"
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                SourceSystem = "SOURCE SYSTEM",
                Value = 2345678.65M
            };

            //Act
            var response = _invoiceHeaderValidator.TestValidate(invoiceHeader);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.AgreementNumber);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public void Given_InvoiceHeader_When_FRN_Is_Empty_Then_InvoiceHeader_Fails()
        {
            //Arrange
            InvoiceHeader invoiceHeader = new InvoiceHeader()
            {
                AgreementNumber = "ERT456",
                AppendixReferences = new AppendixReferences(),
                ContractNumber = "ED34566",
                DeliveryBody = "XYZ",
                DueDate = DateTime.Now.ToString(),
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 23456,
                        Currency = "£",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567"
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                SourceSystem = "SOURCE SYSTEM",
                Value = 2345678.65M
            };

            //Act
            var response = _invoiceHeaderValidator.TestValidate(invoiceHeader);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.FRN);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public void Given_InvoiceHeader_When_SourceSystem_Is_Empty_Then_InvoiceHeader_Fails()
        {
            //Arrange
            InvoiceHeader invoiceHeader = new InvoiceHeader()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                ContractNumber = "ED34566",
                DeliveryBody = "XYZ",
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
                        SchemeCode = "WE4567"
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 2345678.65M
            };

            //Act
            var response = _invoiceHeaderValidator.TestValidate(invoiceHeader);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.SourceSystem);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public void Given_InvoiceHeader_When_PaymentRequestNumber_Is_Empty_Then_InvoiceHeader_Fails()
        {
            //Arrange
            InvoiceHeader invoiceHeader = new InvoiceHeader()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                ContractNumber = "ED34566",
                DeliveryBody = "XYZ",
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
                        SchemeCode = "WE4567"
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                Value = 2345678.65M
            };

            //Act
            var response = _invoiceHeaderValidator.TestValidate(invoiceHeader);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public void Given_InvoiceHeader_When_DueDate_Is_Empty_Then_InvoiceHeader_Fails()
        {
            //Arrange
            InvoiceHeader invoiceHeader = new InvoiceHeader()
            {
                AgreementNumber = "ER456G",
                AppendixReferences = new AppendixReferences(),
                ContractNumber = "ED34566",
                DeliveryBody = "XYZ",
                FRN = 1000000000,
                InvoiceLines = new List<InvoiceLine>()
                {
                    new InvoiceLine()
                    {
                        Value = 23456,
                        Currency = "£",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567"
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 2345678.65M
            };

            //Act
            var response = _invoiceHeaderValidator.TestValidate(invoiceHeader);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.DueDate);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public void Given_InvoiceHeader_When_No_Field_Is_Empty_Then_InvoiceHeader_Pass()
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
                        Value = 23456,
                        Currency = "£",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567"
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 2345678.65M
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
            response.Errors.Count.Equals(0);
        }
    }
}
