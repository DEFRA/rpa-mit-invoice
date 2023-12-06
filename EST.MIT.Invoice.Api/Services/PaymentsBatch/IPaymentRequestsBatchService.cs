using EST.MIT.Invoice.Api.Models;

namespace EST.MIT.Invoice.Api.Services.PaymentsBatch;

public interface IPaymentRequestsBatchService
{
    Task<List<PaymentRequestsBatch>> GetByIdAsync(string id);
    Task<List<PaymentRequestsBatch>> GetByPaymentRequestIdAsync(string paymentRequestId);
    Task<List<PaymentRequestsBatch>> GetBySchemeAndIdAsync(string scheme, string id);
    Task<List<PaymentRequestsBatch>> GetInvoicesByUserIdAsync(string userId);
	Task<PaymentRequestsBatch> CreateAsync(PaymentRequestsBatch invoice, LoggedInUser loggedInUser);
    Task<BulkInvoices?> CreateBulkAsync(BulkInvoices invoices, LoggedInUser loggedInUser);
    Task<PaymentRequestsBatch> UpdateAsync(PaymentRequestsBatch invoice, LoggedInUser loggedInUser);
    Task<string> DeleteBySchemeAndIdAsync(string scheme, string id);
}