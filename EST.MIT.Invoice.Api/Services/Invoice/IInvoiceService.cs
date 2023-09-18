using Invoices.Api.Models;

namespace Invoices.Api.Services;

public interface IInvoiceService
{
    Task<List<Invoice>> GetBySchemeAndIdAsync(string scheme, string id);
    Task<Invoice> CreateAsync(Invoice invoice);
    Task<BulkInvoices?> CreateBulkAsync(BulkInvoices invoices);
    Task<Invoice> UpdateAsync(Invoice invoice);
    Task<string> DeleteBySchemeAndIdAsync(string scheme, string id);
}