using EST.MIT.Invoice.Api.Repositories.Entities;

namespace EST.MIT.Invoice.Api.Repositories.Interfaces
{
    public interface IPaymentRequestsBatchApprovalsRepository
    {
	    Task<IEnumerable<InvoiceEntity>> GetAllInvoicesForApprovalAsync();
	    Task<IEnumerable<InvoiceEntity>> GetAllInvoicesForApprovalByUserIdAsync(string userId);
	    Task<InvoiceEntity> GetInvoiceForApprovalByUserIdAndInvoiceIdAsync(string userId, string invoiceId);
    }
}

