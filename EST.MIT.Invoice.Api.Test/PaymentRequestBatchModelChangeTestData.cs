using EST.MIT.Invoice.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EST.MIT.Invoice.Api.Test;

public static  class PaymentRequestBatchModelChangeTestData
{
    public static PaymentRequestsBatch CreateInvoice(string status)
    {
        return new PaymentRequestsBatch
        {
            Id = "123456789",
            AccountType = "AP",
            Organisation = "Test Org",
            Reference = "123456789",
            SchemeType = "bps",
            PaymentType = "DOM",
            Status = status,
            ApproverId = "12345",
            ApproverEmail = "test.email@defra.gov.uk",
            ApprovedBy = "abc@defra.gov.uk",
            CreatedBy = "henry adetunji",
            PaymentRequests = new List<PaymentRequest> {
                new PaymentRequest {
                    PaymentRequestId = "123456789",
                    SourceSystem = "Manual",
                    MarketingYear = 2023,
                    PaymentRequestNumber = 123456789,
                    AgreementNumber = "123456789",
                    Currency = "GBP",
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
                    FRN = 9999999999,
                }
            }
        };
    }


}

