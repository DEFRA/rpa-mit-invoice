using FluentValidation.TestHelper;
using Invoices.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoices.Api.Test;

public class BulkInvoiceValidationTests
{
    private readonly BulkInvoiceValidator _invoiceValidator;

    public BulkInvoiceValidationTests()
    {
        _invoiceValidator = new BulkInvoiceValidator();
    }

    [Fact]
    public async Task Given_BulkInvoice_When_Nested_Class_Invoice_Id_Is_Duplicated_Then_Failure_Message_Invoice_ID_Is_Duplicated_In_This_Batch()
    {
        //Arrange
        BulkInvoices bulkInvoices = new BulkInvoices()
        {
            Reference = "erty",
            SchemeType = "AP",

            Invoices = new List<Invoice> {

                new Invoice {
                    Id = "SDEF",
                    InvoiceType = "AP",
                    AccountType = "AP",
                    Organisation = "Test Org",
                    Reference = "123456789",
                    SchemeType = "bps",
                    PaymentType = "AP",
                    CreatedBy = "Test User",
                    Status = "status",
                    PaymentRequests = new List<InvoiceHeader> {
                        new InvoiceHeader {
                            PaymentRequestId = "123456789",
                            SourceSystem = "Manual",
                            MarketingYear = 2023,
                            DeliveryBody = "Test Org",
                            PaymentRequestNumber = 123456789,
                            AgreementNumber = "123456789",
                            ContractNumber = "123456789",
                            Value = 100,
                            DueDate = "2023-01-01",
                            FRN = 1000000000,
                            AppendixReferences = new AppendixReferences {
                                ClaimReferenceNumber = "123456789"
                            },
                            InvoiceLines = new List<InvoiceLine> {
                                new InvoiceLine {
                                    Currency = "GBP",
                                    Description = "Test Description",
                                    Value = 100,
                                    SchemeCode = "123456789",
                                    FundCode = "123456789",
                                }
                            }
                        }
                    }
                },

                new Invoice {
                    Id = "SDEF",
                    InvoiceType = "AP",
                    AccountType = "AP",
                    Organisation = "Test Org",
                    Reference = "123456789",
                    SchemeType = "bps",
                    PaymentType = "AP",
                    CreatedBy = "Test User",
                    Status = "status",
                    PaymentRequests = new List<InvoiceHeader> {
                        new InvoiceHeader {
                            PaymentRequestId = "123456789",
                            SourceSystem = "Manual",
                            MarketingYear = 2023,
                            DeliveryBody = "Test Org",
                            PaymentRequestNumber = 123456789,
                            AgreementNumber = "123456789",
                            ContractNumber = "123456789",
                            Value = 100,
                            DueDate = "2023-01-01",
                            FRN = 1000000000,
                            AppendixReferences = new AppendixReferences {
                                ClaimReferenceNumber = "123456789"
                            },
                            InvoiceLines = new List<InvoiceLine> {
                                new InvoiceLine {
                                    Currency = "GBP",
                                    Description = "Test Description",
                                    Value = 100,
                                    SchemeCode = "123456789",
                                    FundCode = "123456789",
                                }
                            }
                        }
                    }
                }
            }
        };

        //Act
        var response = await _invoiceValidator.TestValidateAsync(bulkInvoices);
    }
}

