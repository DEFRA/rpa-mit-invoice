using Invoices.Api.Models;

namespace Invoices.Api.Services;

public interface ICosmosService
{
    Task<List<PaymentRequestsBatch>> Get(string sqlCosmosQuery);
    Task<PaymentRequestsBatch> Create(PaymentRequestsBatch paymentRequestsBatch);
    Task<BulkInvoices?> CreateBulk(BulkInvoices invoices);
    Task<PaymentRequestsBatch> Update(PaymentRequestsBatch paymentRequestsBatch);
    Task<string> Delete(string id, string scheme);
}