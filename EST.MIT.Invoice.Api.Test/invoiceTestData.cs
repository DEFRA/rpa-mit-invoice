using Invoices.Api.Models;

namespace Invoices.Api.Test;

public static class InvoiceTestData
{
    public static Invoice CreateInvoice(string status = "awaiting")
    {
        return new Invoice
        {
            Id = "123456789",
            InvoiceType = "AP",
            AccountType = "AP",
            Organisation = "Test Org",
            SchemeType = "bps",
            Status = status,
            Headers = new List<InvoiceHeader> {
                new InvoiceHeader {
                    PaymentRequestId = "123456789",
                    FRN = 123456789,
                    SourceSystem = "Manual",
                    MarketingYear = 2023,
                    Ledger = "AP",
                    DeliveryBody = "Test Org",
                    PaymentRequestNumber = 123456789,
                    AgreementNumber = "123456789",
                    ContractNumber = "123456789",
                    Value = 100,
                    DueDate = "2023-01-01",
                    AppendixReferences = new AppendixReferences {
                        ClaimReferenceNumber = "123456789"
                    },
                    InvoiceLines = new List<InvoiceLine> {
                        new InvoiceLine {
                            Currency = "GBP",
                            Description = "Test Description",
                            Value = 100,
                            SchemeCode = "123456789",
                            FundCode = "123456789"
                        }
                    }
                }
            }
        };
    }
}
