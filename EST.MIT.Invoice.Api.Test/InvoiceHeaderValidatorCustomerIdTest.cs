using System.Net;
using EST.MIT.Invoice.Api.Services.API.Interfaces;
using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.API.Models;
using FluentValidation.TestHelper;
using Invoices.Api.Models;
using Newtonsoft.Json.Linq;
using NSubstitute;

namespace EST.MIT.Invoice.Api.Test
{
    public class InvoiceHeaderValidatorCustomerIdTest
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

        public InvoiceHeaderValidatorCustomerIdTest()
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
        public async Task Given_InvoiceHeader_When_All_Is_Ok_Then_InvoiceHeader_Passes()
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
                FirmReferenceNumber = 9999999999,
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
        [InlineData(100000000, 1000000000, "100000")]
        [InlineData(0, 1000000000, "100000")]
        [InlineData(100000000, 0, "100000")]
        [InlineData(100000000, 1000000000, "")]
        [InlineData(0, 0, "")]
        [InlineData(0, 0, null)]
        public async Task Given_InvoiceHeader_When_SBI_FRN_And_VendorId_Supplied_Then_InvoiceHeader_Fails(int sbi, long frn, string vendorId)
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
                SingleBusinessIdentifier = sbi,
                VendorID = vendorId,
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

            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("Invoice must only have Single Business Identifier (SBI), Firm Reference Number (FRN) or Vendor ID")) == 1);
        }

        [Theory]
        [InlineData(10000000)]
        [InlineData(1000000000)]
        public async Task Given_InvoiceHeader_When_SBI_Is_Invalid_Then_InvoiceHeader_Fails(int sbi)
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

            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("SBI is not in valid range (105000000 .. 999999999)")) == 1);
        }

        [Theory]
        [InlineData(100000000)]
        [InlineData(10000000000)]
        public async Task Given_InvoiceHeader_When_FRN_Is_Invalid_Then_InvoiceHeader_Fails(long frn)
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

            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("FRN is not in valid range (1000000000 .. 9999999999)")) == 1);
        }

        [Theory]
        [InlineData("10000")]
        [InlineData("1000000")]
        public async Task Given_InvoiceHeader_When_VendorID_Is_Invalid_Then_InvoiceHeader_Fails(string vendorID)
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
                VendorID = vendorID,
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

            Assert.Single(response.Errors);
            Assert.True(response.Errors.Count(x => x.ErrorMessage.Contains("VendorID must be 6 characters")) == 1);
        }
    }
}

