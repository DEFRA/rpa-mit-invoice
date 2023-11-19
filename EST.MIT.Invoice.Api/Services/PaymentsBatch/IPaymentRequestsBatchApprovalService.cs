using EST.MIT.Invoice.Api.Models;

namespace EST.MIT.Invoice.Api.Services.PaymentsBatch;

public interface IPaymentRequestsBatchApprovalService
{
    Task<List<PaymentRequestsBatch>> GetAllInvoicesForApprovalAsync();
    Task<List<PaymentRequestsBatch>> GetAllInvoicesForApprovalByUserIdAsync(string userId);
    Task<PaymentRequestsBatch?> GetInvoiceForApprovalByUserIdAndInvoiceIdAsync(string userId, string invoiceId);
}