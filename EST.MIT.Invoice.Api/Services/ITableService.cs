using Invoices.Api.Models;
using Invoices.Api.Services.Models;

namespace Invoices.Api.Services;

public interface ITableService
{
    Task<InvoiceEntity?> GetInvoice(string type, string id);
    Task<bool> CreateInvoice(Invoice invoice);
    Task<bool> UpdateInvoice(Invoice invoice);
}
