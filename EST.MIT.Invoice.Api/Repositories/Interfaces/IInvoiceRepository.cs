using System;
using Invoices.Api.Models;
using Invoices.Api.Repositories.Entities;

namespace Invoices.Api.Repositories.Interfaces
{
	public interface IInvoiceRepository
	{
        Task<IEnumerable<InvoiceEntity>> GetBySchemeAndIdAsync(string scheme, string id);
        Task<InvoiceEntity> CreateAsync(InvoiceEntity invoice);
        Task<BulkInvoicesEntity?> CreateBulkAsync(BulkInvoicesEntity invoices);
        Task<InvoiceEntity> UpdateAsync(InvoiceEntity invoice);
        Task<string> DeleteBySchemeAndIdAsync(string scheme, string id);
    }
}

