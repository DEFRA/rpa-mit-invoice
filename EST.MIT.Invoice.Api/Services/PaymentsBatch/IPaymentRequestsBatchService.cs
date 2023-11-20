using EST.MIT.Invoice.Api.Models;

namespace EST.MIT.Invoice.Api.Services.PaymentsBatch;

public interface IPaymentRequestsBatchService
{
    Task<List<PaymentRequestsBatch>> GetBySchemeAndIdAsync(string scheme, string id);
    Task<List<PaymentRequestsBatch>> GetInvoicesByUserIdAsync(string userId);
	Task<PaymentRequestsBatch> CreateAsync(PaymentRequestsBatch invoice);
    Task<BulkInvoices?> CreateBulkAsync(BulkInvoices invoices);
    Task<PaymentRequestsBatch> UpdateAsync(PaymentRequestsBatch invoice);
    Task<string> DeleteBySchemeAndIdAsync(string scheme, string id);
}