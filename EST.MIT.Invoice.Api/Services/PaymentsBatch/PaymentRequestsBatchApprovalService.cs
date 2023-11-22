using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Repositories.Entities;
using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Util;

namespace EST.MIT.Invoice.Api.Services.PaymentsBatch;

public class PaymentRequestsBatchApprovalService : IPaymentRequestsBatchApprovalService
{
    private readonly IPaymentRequestsBatchApprovalsRepository _paymentRequestsBatchApprovalsRepository;

    public PaymentRequestsBatchApprovalService(IPaymentRequestsBatchApprovalsRepository paymentRequestsBatchApprovalsRepository)
    {
		_paymentRequestsBatchApprovalsRepository = paymentRequestsBatchApprovalsRepository;
    }

    public async Task<List<PaymentRequestsBatch>> GetAllInvoicesForApprovalAsync()
    {
	    var invoicesForApproval = await _paymentRequestsBatchApprovalsRepository.GetAllInvoicesForApprovalAsync();

		return InvoiceMapper.MapToInvoice(invoicesForApproval);
    }

    public async Task<List<PaymentRequestsBatch>> GetAllInvoicesForApprovalByUserIdAsync(string userId)
    {
	    var invoicesForApproval = await _paymentRequestsBatchApprovalsRepository.GetAllInvoicesForApprovalByUserIdAsync(userId);

		return InvoiceMapper.MapToInvoice(invoicesForApproval);
    }

    public async Task<PaymentRequestsBatch?> GetInvoiceForApprovalByUserIdAndInvoiceIdAsync(string userId, string invoiceId)
    {
        var invoice = await _paymentRequestsBatchApprovalsRepository.GetInvoiceForApprovalByUserIdAndInvoiceIdAsync(userId, invoiceId);

        return InvoiceMapper.MapToPaymentRequestsBatch(invoice);
    }
}