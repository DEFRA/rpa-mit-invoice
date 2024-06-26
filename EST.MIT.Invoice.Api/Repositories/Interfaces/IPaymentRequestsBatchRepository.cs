﻿using EST.MIT.Invoice.Api.Repositories.Entities;

namespace EST.MIT.Invoice.Api.Repositories.Interfaces
{
    public interface IPaymentRequestsBatchRepository
    {
        Task<IEnumerable<InvoiceEntity>> GetByIdAsync(string id);
        Task<IEnumerable<InvoiceEntity>> GetByPaymentRequestIdAsync(string paymentRequestId);
        Task<IEnumerable<InvoiceEntity>> GetBySchemeAndIdAsync(string scheme, string id);
        Task<InvoiceEntity> CreateAsync(InvoiceEntity invoice);
        Task<BulkInvoicesEntity?> CreateBulkAsync(BulkInvoicesEntity invoices);
        Task<InvoiceEntity> UpdateAsync(InvoiceEntity invoice);
        Task<string> DeleteBySchemeAndIdAsync(string scheme, string id);
        Task<IEnumerable<InvoiceEntity>> GetInvoicesByUserIdAsync(string userId);
    }
}

