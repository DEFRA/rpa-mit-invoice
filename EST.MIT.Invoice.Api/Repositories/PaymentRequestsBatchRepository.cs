using System.Diagnostics.CodeAnalysis;
using Dapper;
using EST.MIT.Invoice.Api.Repositories.Entities;
using EST.MIT.Invoice.Api.Repositories.Interfaces;

namespace EST.MIT.Invoice.Api.Repositories
{
    // Not really worth doing a series of unit tests for these methods since we could only really test that the correct SQL was being passed,
    // but considerable effort would be needed to mock the DB interfaces
    [ExcludeFromCodeCoverage]
    public class PaymentRequestsBatchRepository : IPaymentRequestsBatchRepository
    {
		private readonly PgDbContext _dbContext;

		public PaymentRequestsBatchRepository(PgDbContext dbContext)
		{
			_dbContext = dbContext;
		}

        public async Task<IEnumerable<InvoiceEntity>> GetBySchemeAndIdAsync(string schemeType, string id)
		{
            using var connection = await _dbContext.CreateConnectionAsync();
            var sql = "SELECT * FROM Invoices WHERE SchemeType = @SchemeType and Id = @Id";
			var parameters = new { SchemeType = schemeType, Id = id };
            return await connection.QueryAsync<InvoiceEntity>(sql, parameters);
        }

        public async Task<InvoiceEntity> CreateAsync(InvoiceEntity invoice)
		{
            using var connection = await _dbContext.CreateConnectionAsync();
            var sql = "INSERT INTO Invoices (Id, SchemeType, Data, Reference, Value, Status, CreatedBy, Created) " +
            "VALUES (@Id, @SchemeType, @Data, @Reference, @Value, @Status, @CreatedBy, @Created)";
            await connection.ExecuteAsync(sql, invoice);
			return invoice;
        }

		public async Task<BulkInvoicesEntity?> CreateBulkAsync(BulkInvoicesEntity entities)
		{
            using var connection = await _dbContext.CreateConnectionAsync();
            var sql = "INSERT INTO Invoices (Id, SchemeType, Data, Reference, Value, Status, CreatedBy, Created) " +
            "VALUES (@Id, @SchemeType, @Data, @Reference, @Value, @Status, @CreatedBy, @Created)";
            foreach (var invoice in entities.Invoices)
            {
                await connection.ExecuteAsync(sql, invoice);
            }
            return entities;
        }

        public async Task<InvoiceEntity> UpdateAsync(InvoiceEntity invoice)
		{
            using var connection = await _dbContext.CreateConnectionAsync();
            var sql = "UPDATE Invoices " +
                    "SET SchemeType = @SchemeType, " +
                    "Data = @Data, " +
                    "Reference = @Reference, " +
                    "Value = @Value, " +
                    "Status = @Status, " +
                    "UpdatedBy = @UpdatedBy, " +
                    "Updated = @Updated " +
                    "WHERE Id = @Id";
            await connection.ExecuteAsync(sql, invoice);
            return invoice;
        }

        public async Task<string> DeleteBySchemeAndIdAsync(string schemeType, string id)
		{
            using var connection = await _dbContext.CreateConnectionAsync();
            var sql = "DELETE FROM Invoices WHERE Id = @Id AND SchemeType = @schemeType";
            await connection.ExecuteAsync(sql, new { SchemeType = schemeType, Id = id });
            return id;
        }
    }
}