using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.API.Interfaces;
using EST.MIT.Invoice.Api.Services.API.Models;
using FluentValidation.TestHelper;
using Invoices.Api.Models;
using NSubstitute;
using System.Net;

namespace EST.MIT.Invoice.Api.Test
{
    public class InvoiceHeaderValidatorTest
    {
        private readonly InvoiceHeaderValidator _invoiceHeaderValidator;

        private readonly IReferenceDataApi _referenceDataApiMock =
     Substitute.For<IReferenceDataApi>();

        private readonly FieldsRoute route = new()
        {
            PaymentType = "AP",
            InvoiceType = "AP",
            Organisation = "Test Org",
            SchemeType = "bps"
        };

        public InvoiceHeaderValidatorTest()
        {
            var schemeCodeErrors = new Dictionary<string, List<string>>();
            var fundCodeErrors = new Dictionary<string, List<string>>();
            var schemeCodeResponse = new ApiResponse<IEnumerable<SchemeCode>>(HttpStatusCode.OK, schemeCodeErrors);
            var fundCodeResponse = new ApiResponse<IEnumerable<FundCode>>(HttpStatusCode.OK, fundCodeErrors);

            var schemeCodes = new List<SchemeCode>()
            {
                new SchemeCode()
                {
                    Code = "WE4567"
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

            _referenceDataApiMock
            .GetSchemeCodesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(schemeCodeResponse));

            _referenceDataApiMock
            .GetFundCodesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(fundCodeResponse));

            _invoiceHeaderValidator = new InvoiceHeaderValidator(_referenceDataApiMock, route);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_AgreementNumber_Is_Null_Then_InvoiceHeader_Fails()
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
                        Currency = "GBP",
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
            var response = await _invoiceHeaderValidator.TestValidateAsync(invoiceHeader);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.AgreementNumber);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_FRN_Is_Empty_Then_InvoiceHeader_Fails()
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
                        Currency = "GBP",
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
            var response = await _invoiceHeaderValidator.TestValidateAsync(invoiceHeader);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.FRN);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_SourceSystem_Is_Empty_Then_InvoiceHeader_Fails()
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
                        Currency = "GBP",
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
            var response = await _invoiceHeaderValidator.TestValidateAsync(invoiceHeader);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.SourceSystem);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_PaymentRequestNumber_Is_Empty_Then_InvoiceHeader_Fails()
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
                        Currency = "GBP",
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
            var response = await _invoiceHeaderValidator.TestValidateAsync(invoiceHeader);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_DueDate_Is_Empty_Then_InvoiceHeader_Fails()
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
                        Currency = "GBP",
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
            var response = await _invoiceHeaderValidator.TestValidateAsync(invoiceHeader);

            //Assert
            response.ShouldHaveValidationErrorFor(x => x.DueDate);
            response.Errors.Count.Equals(1);
        }

        [Fact]
        public async Task Given_InvoiceHeader_When_No_Field_Is_Empty_Then_InvoiceHeader_Pass()
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
                        Currency = "GBP",
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
            var response = await _invoiceHeaderValidator.TestValidateAsync(invoiceHeader);

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

        [Theory]
        [InlineData("GBP", "EUR")]
        [InlineData("EUR", "GBP")]
        public async Task Given_InvoiceHeader_When_Currencies_Are_Different_Then_InvoiceHeader_Fails(string currencyOne, string currencyTwo)
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
                        Value = 2345678.00M,
                        Currency = currencyOne,
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567"
                    },
                    new InvoiceLine()
                    {
                        Value = 0.65M,
                        Currency = currencyTwo,
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567"
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 2345678.65M,
                FirmReferenceNumber = "1000000000",
            };

            //Act
            var response = await _invoiceHeaderValidator.TestValidateAsync(invoiceHeader);

            //Assert
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Cannot mix currencies in an invoice")) == 1);
            Assert.Single(response.Errors);
        }

        // 

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
                        Value = value,
                        Currency = "GBP",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567"
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = value,
                FirmReferenceNumber = "1000000000",
            };

            //Act
            var response = await _invoiceHeaderValidator.TestValidateAsync(invoiceHeader);

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

        [Theory]
        [InlineData(10.101)]
        [InlineData(10.1234)]
        [InlineData(10.00001)]
        public async Task Given_InvoiceHeader_When_Value_Has_More_Than_2DP_Then_InvoiceHeader_Fails(decimal value)
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
                        Value = value,
                        Currency = "GBP",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567"
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = value,
                FirmReferenceNumber = "1000000000",
            };

            //Act
            var response = await _invoiceHeaderValidator.TestValidateAsync(invoiceHeader);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.ContractNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.DeliveryBody);
            response.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            response.ShouldNotHaveValidationErrorFor(x => x.FRN);
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
                        Value = 0,
                        Currency = "GBP",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567"
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = 0,
                FirmReferenceNumber = "1000000000",
            };

            //Act
            var response = await _invoiceHeaderValidator.TestValidateAsync(invoiceHeader);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.ContractNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.DeliveryBody);
            response.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            response.ShouldNotHaveValidationErrorFor(x => x.FRN);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            Assert.Equal(2, response.Errors.Count);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice value must be non-zero")) == 1);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice line value must be non-zero")) == 1);
        }

        [Theory]
        [InlineData(10, 5, 4)]
        [InlineData(10, 3, 6)]
        [InlineData(10, -5, -5)]
        [InlineData(10, -5, 5)]
        public async Task Given_InvoiceHeader_When_Value_Does_Not_Equal_Sum_Of_InvoiceLines_Then_InvoiceHeader_Fails(decimal invoiceValue, decimal invoiceLine1Value, decimal invoiceLine2Value)
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
                        Value = invoiceLine1Value,
                        Currency = "GBP",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567"
                    },
                    new InvoiceLine()
                    {
                        Value = invoiceLine2Value,
                        Currency = "GBP",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567"
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = invoiceValue,
                FirmReferenceNumber = "1000000000",
            };

            //Act
            var response = await _invoiceHeaderValidator.TestValidateAsync(invoiceHeader);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.ContractNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.DeliveryBody);
            response.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            response.ShouldNotHaveValidationErrorFor(x => x.FRN);
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
                        Value = invoiceLine1Value,
                        Currency = "GBP",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567"
                    },
                    new InvoiceLine()
                    {
                        Value = invoiceLine2Value,
                        Currency = "GBP",
                        Description = "ABD",
                        FundCode = "FUNDCODE",
                        SchemeCode = "WE4567"
                    }
                },
                MarketingYear = 2022,
                PaymentRequestId = "1234",
                PaymentRequestNumber = 123456,
                Value = invoiceValue,
                FirmReferenceNumber = "1000000000",
            };

            //Act
            var response = await _invoiceHeaderValidator.TestValidateAsync(invoiceHeader);

            //Assert
            response.ShouldNotHaveValidationErrorFor(x => x.SourceSystem);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestId);
            response.ShouldNotHaveValidationErrorFor(x => x.MarketingYear);
            response.ShouldNotHaveValidationErrorFor(x => x.PaymentRequestNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AgreementNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);
            response.ShouldNotHaveValidationErrorFor(x => x.ContractNumber);
            response.ShouldNotHaveValidationErrorFor(x => x.DeliveryBody);
            response.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            response.ShouldNotHaveValidationErrorFor(x => x.FRN);
            response.ShouldNotHaveValidationErrorFor(x => x.InvoiceLines);
            response.ShouldNotHaveValidationErrorFor(x => x.AppendixReferences);

            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("The ABS invoice value must be less than 1 Billion")) == 1);
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

