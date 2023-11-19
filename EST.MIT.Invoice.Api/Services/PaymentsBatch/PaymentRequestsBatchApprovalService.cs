using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Repositories.Entities;
using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Util;

namespace EST.MIT.Invoice.Api.Services.PaymentsBatch;

public class PaymentRequestsBatchApprovalService : IPaymentRequestsBatchApprovalService
{
    private readonly IPaymentRequestsBatchRepository _paymentRequestsBatchRepository;

    public PaymentRequestsBatchApprovalService(IPaymentRequestsBatchRepository paymentRequestsBatchRepository)
    {
        _paymentRequestsBatchRepository = paymentRequestsBatchRepository;
    }

    public Task<List<PaymentRequestsBatch>> GetAllInvoicesForApprovalByUserIdAsync(string userId)
    {
        return Task.FromResult(InvoiceMapper.MapToInvoice(GetAllMockedInvoiceEntities()));
    }

    public Task<PaymentRequestsBatch?> GetInvoiceForApprovalByUserIdAndInvoiceIdAsync(string userId, string invoiceId)
    {
        var invoice = GetAllMockedInvoiceEntities().Find(x => x.Id == invoiceId);

        if (invoice == null)
        {
            return Task.FromResult<PaymentRequestsBatch?>(null);
        }

        return Task.FromResult<PaymentRequestsBatch?>(InvoiceMapper.MapToPaymentRequestsBatch(invoice));
    }

    public async Task<List<PaymentRequestsBatch>> GetInvoicesByUserIdAsync(string userId)
    {
        var result = await _paymentRequestsBatchRepository.GetInvoicesByUserIdAsync(userId);
        return InvoiceMapper.MapToInvoice(result);
    }

    private List<InvoiceEntity> GetAllMockedInvoiceEntities()
    {
        return new List<InvoiceEntity>()
        {
            new InvoiceEntity()
            {
                Id = "3841b9d8-1ea0-4e73-9b47-98eb6cbed75a",
                SchemeType = "LS",
                Data = "{\"id\":\"3841b9d8-1ea0-4e73-9b47-98eb6cbed75a\",\"accountType\":\"AR\",\"organisation\":\"NE\",\"paymentType\":\"EU\",\"schemeType\":\"LS\",\"paymentRequests\":[{\"paymentRequestId\":\"1_AA0YZHA0\",\"sourceSystem\":\"Manual\",\"frn\":1234567890,\"value\":19.5,\"currency\":\"GBP\",\"description\":null,\"originalInvoiceNumber\":\"A1\",\"originalSettlementDate\":\"2015-01-01T00:00:00+00:00\",\"recoveryDate\":\"2015-01-01T00:00:00+00:00\",\"invoiceCorrectionReference\":\"PR\",\"invoiceLines\":[{\"value\":10.25,\"fundCode\":\"EXQ00\",\"mainAccount\":\"SOS360\",\"schemeCode\":\"40161\",\"marketingYear\":2023,\"deliveryBody\":\"FC99\",\"description\":\"G00 - Gross value of claim\",\"currency\":null},{\"value\":9.25,\"fundCode\":\"EXQ00\",\"mainAccount\":\"SOS310\",\"schemeCode\":\"40164\",\"marketingYear\":2023,\"deliveryBody\":\"FC99\",\"description\":\"G00 - Gross value of claim\",\"currency\":null}],\"marketingYear\":2014,\"paymentRequestNumber\":1,\"agreementNumber\":\"1\",\"dueDate\":\"01/01/2015\",\"appendixReferences\":null,\"sbi\":0,\"vendor\":null}],\"status\":\"awaiting\",\"reference\":null,\"created\":\"2023-10-30T08:47:32.890385+00:00\",\"updated\":\"2023-10-30T08:49:58.4200863+00:00\",\"createdBy\":\"1\",\"updatedBy\":\"1\"}",
                Reference = "123456789",
                Value = 19.5M,
                Status = "awaiting", // dont just change this status, create a new invoice with a different status
                CreatedBy = "1",
                Created = new DateTime(2023, 11, 02, 11, 27, 45, 540, DateTimeKind.Utc),
                UpdatedBy = "1",
                Updated = new DateTime(2023, 11, 02, 11, 45, 52, 175, DateTimeKind.Utc),
                ApproverId = "1",
				ApproverEmail = "user1@defra.gov.uk"
            }
        };
    }
}