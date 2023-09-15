using Invoices.Api.Models;

namespace Invoices.Api.Test;

public static class PaymentRequestsBatchTestData
{
    public static PaymentRequestsBatch CreateInvoice(string status = "awaiting")
    {
        return new PaymentRequestsBatch
        {
            Id = "123456789",
            InvoiceType = "AP",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "123456789",
            SchemeType = "bps",
            PaymentType = "DOM",
            Status = status,
            CreatedBy = "Test User",
            PaymentRequests = new List<PaymentRequest> {
                new PaymentRequest {
                    PaymentRequestId = "123456789",
                    FRN = 123456789,
                    SourceSystem = "Manual",
                    MarketingYear = 2023,
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
                            SchemeCode = "SchemeCodeValue",
                            FundCode = "123456789",
                            MainAccount = "AccountCodeValue",
                            DeliveryBody = "RP00",
                            MarketingYear = 2023,
                        }
                    },
                    FirmReferenceNumber = 1000000000,
                }
            }
        };
    }
}
