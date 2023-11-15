using System;
using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Repositories.Entities;

namespace EST.MIT.Invoice.Api.Repositories.Interfaces
{
	public interface IPaymentRequestsBatchRepository
	{
        Task<IEnumerable<InvoiceEntity>> GetBySchemeAndIdAsync(string scheme, string id);
        Task<IEnumerable<InvoiceEntity>> GetInvoicesForApprovalByUserIdAsync(string userId);
        Task<InvoiceEntity> CreateAsync(InvoiceEntity invoice);
        Task<BulkInvoicesEntity?> CreateBulkAsync(BulkInvoicesEntity invoices);
        Task<InvoiceEntity> UpdateAsync(InvoiceEntity invoice);
        Task<string> DeleteBySchemeAndIdAsync(string scheme, string id);
    }
}

