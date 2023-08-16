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

        [Fact]
        public async Task Given_InvoiceHeader_When_PaymentRequestId_Is_Null_Or_Empty_Then_Failure_Message_PaymentRequestId_Is_Missing_Is_Thrown()
        {
            //Arrange
            InvoiceHeader invoiceHeader = new InvoiceHeader()
            {
                PaymentRequestId = string.Empty,
                SourceSystem = "Manual",
                MarketingYear = 2023,
                DeliveryBody = "Test Org",
                PaymentRequestNumber = 123456789,
                AgreementNumber = "123456789",
                ContractNumber = "123456789",
                Value = 100,
                DueDate = "2023-01-01",
                FRN = 1000000000,
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
                    }
                }
            };

            //Act
            var response = await _invoiceHeaderValidator.TestValidateAsync(invoiceHeader);

            //Assert
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("PaymentRequestId is missing")) == 1);
            Assert.True(response.Errors[0].ErrorMessage == "PaymentRequestId is missing");
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_PaymentRequestId_Characters_Are_Greater_Than_Twenty_Then_Failure_Message_PaymentRequestId_Must_Not_Be_More_Than_Twenty_Is_Thrown()
        {
            //Arrange
            InvoiceHeader invoiceHeader = new InvoiceHeader()
            {
                PaymentRequestId = "123456789ABCDEFGHIJKLMNOP",
                SourceSystem = "Manual",
                MarketingYear = 2023,
                DeliveryBody = "Test Org",
                PaymentRequestNumber = 123456789,
                AgreementNumber = "123456789",
                ContractNumber = "123456789",
                Value = 100,
                DueDate = "2023-01-01",
                FRN = 1000000000,
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
                    }
                }
            };

            //Act
            var response = await _invoiceHeaderValidator.TestValidateAsync(invoiceHeader);

            //Assert
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("PaymentRequestId must not be more than 20 characters")) == 1);
            Assert.True(response.Errors[0].ErrorMessage == "PaymentRequestId must not be more than 20 characters");
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_PaymentRequestId_Contain_No_Characters_Then_Fialure_Message_PaymentRequestId_Must_Be_Atleast_One_Character_Is_Thrown()
        {
            //Arrange
            InvoiceHeader invoiceHeader = new InvoiceHeader()
            {
                PaymentRequestId = "",
                SourceSystem = "Manual",
                MarketingYear = 2023,
                DeliveryBody = "Test Org",
                PaymentRequestNumber = 123456789,
                AgreementNumber = "123456789",
                ContractNumber = "123456789",
                Value = 100,
                DueDate = "2023-01-01",
                FRN = 1000000000,
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
                    }
                }
            };

            //Act
            var response = await _invoiceHeaderValidator.TestValidateAsync(invoiceHeader);

            //Assert            
            Assert.True(response.Errors[1].ErrorMessage == "PaymentRequestId must contain at least one character");
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_PaymentRequestId_Contain_Spaces_Then_Failure_Message_PaymentRequestIsId_Cannot_Contain_Spaces_Is_Thrown()
        {
            //Arrange
            InvoiceHeader invoiceHeader = new InvoiceHeader()
            {
                PaymentRequestId = " S DEF ",
                SourceSystem = "Manual",
                MarketingYear = 2023,
                DeliveryBody = "Test Org",
                PaymentRequestNumber = 123456789,
                AgreementNumber = "123456789",
                ContractNumber = "123456789",
                Value = 100,
                DueDate = "2023-01-01",
                FRN = 1000000000,
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
                    }
                }
            };

            //Act
            var response = await _invoiceHeaderValidator.TestValidateAsync(invoiceHeader);

            //Assert
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("PaymentRequestId cannot contain spaces")) == 1);
            Assert.True(response.Errors[0].ErrorMessage == "PaymentRequestId cannot contain spaces");
        }
    }
}

