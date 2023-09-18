using Invoices.Api.Models;

namespace Invoices.Api.Services.PaymentsBatch;

public interface IPaymentRequestsBatchService
{
    Task<List<PaymentRequestsBatch>> GetBySchemeAndIdAsync(string scheme, string id);
    Task<PaymentRequestsBatch> CreateAsync(PaymentRequestsBatch invoice);
    Task<BulkInvoices?> CreateBulkAsync(BulkInvoices invoices);
    Task<PaymentRequestsBatch> UpdateAsync(PaymentRequestsBatch invoice);
    Task<string> DeleteBySchemeAndIdAsync(string scheme, string id);
}