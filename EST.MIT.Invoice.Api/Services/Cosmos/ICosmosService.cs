using Invoices.Api.Models;

namespace Invoices.Api.Services;

public interface ICosmosService
{
    Task<List<Invoice>> Get(string sqlCosmosQuery);
    Task<Invoice> Create(Invoice invoice);
    Task<BulkInvoices?> CreateBulk(BulkInvoices invoices);
    Task<Invoice> Update(Invoice invoice);
    Task<string> Delete(string id, string scheme);
}