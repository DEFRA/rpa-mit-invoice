using System.Diagnostics.CodeAnalysis;
using Dapper;
using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Repositories.Entities;
using EST.MIT.Invoice.Api.Repositories.Interfaces;

namespace EST.MIT.Invoice.Api.Repositories
{
    // Not really worth doing a series of unit tests for these methods since we could only really test that the correct SQL was being passed,
    // but considerable effort would be needed to mock the DB interfaces
    [ExcludeFromCodeCoverage]
    public class PaymentRequestsBatchApprovalsRepository : IPaymentRequestsBatchApprovalsRepository
	{
        private readonly IPgDbContext _dbContext;

        public PaymentRequestsBatchApprovalsRepository(IPgDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<InvoiceEntity>> GetAllInvoicesForApprovalAsync()
        {
	        using var connection = await _dbContext.CreateConnectionAsync();
	        var sql = "SELECT SchemeType, Id, Data, Reference, Value, Status, ApproverId, ApproverEmail, ApprovedBy, Approved, CreatedBy, Updated, Created, Updated FROM Invoices WHERE Status = @Status";
	        var parameters = new { Status = InvoiceStatuses.AwaitingApproval };
	        return await connection.QueryAsync<InvoiceEntity>(sql, parameters);
		}

        public async Task<IEnumerable<InvoiceEntity>> GetAllInvoicesForApprovalByUserIdAsync(string userId)
        {
	        if (string.IsNullOrWhiteSpace(userId))
	        {
		        throw new ArgumentNullException(nameof(userId));
	        }

	        using var connection = await _dbContext.CreateConnectionAsync();
	        var sql = "SELECT SchemeType, Id, Data, Reference, Value, Status, ApproverId, ApproverEmail, ApprovedBy, Approved, CreatedBy, Updated, Created, Updated FROM Invoices WHERE (ApproverId = @ApproverId AND Status = @Status) OR @FetchAll";
	        var parameters = new { Status = InvoiceStatuses.AwaitingApproval, ApproverId = userId, FetchAll = true };   // TODO: Remove the FetchAll override when AD User access is patched in
	        return await connection.QueryAsync<InvoiceEntity>(sql, parameters);
		}
		
        public async Task<InvoiceEntity> GetInvoiceForApprovalByUserIdAndInvoiceIdAsync(string userId, string invoiceId)
        {
	        if (string.IsNullOrWhiteSpace(userId))
	        {
		        throw new ArgumentNullException(nameof(userId));
	        }

	        using var connection = await _dbContext.CreateConnectionAsync();
	        var sql = "SELECT SchemeType, Id, Data, Reference, Value, Status, ApproverId, ApproverEmail, ApprovedBy, Approved, CreatedBy, Updated, Created, Updated FROM Invoices WHERE Id = @InvoiceId AND ApproverId = @ApproverId AND Status = @Status";
	        var parameters = new { Status = InvoiceStatuses.AwaitingApproval, ApproverId = userId, InvoiceId = invoiceId };
	        return await connection.QueryFirstAsync<InvoiceEntity>(sql, parameters);
		}
	}
}