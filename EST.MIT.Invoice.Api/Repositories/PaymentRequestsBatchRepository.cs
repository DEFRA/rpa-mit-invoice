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

        public async Task<IEnumerable<InvoiceEntity>> GetInvoicesForApprovalByUserIdAsync(string userId)
        {
            var allInvoices = getAllMockedInvoiceEntities().Where(x => x.Status.ToLower() == InvoiceStatuses.Awaiting.ToLower());

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            //TODO: fxs will need to update this so that when the approvals stuff is in we can use that id
            return allInvoices;
        }

        public async Task<InvoiceEntity> CreateAsync(InvoiceEntity invoice)
        {
            using var connection = await _dbContext.CreateConnectionAsync();
            var sql = "INSERT INTO Invoices (Id, SchemeType, Data, Reference, Value, Status, ApproverId, ApproverEmail, ApprovedBy, Approved, CreatedBy, Created) " +
            "VALUES (@Id, @SchemeType, @Data, @Reference, @Value, @Status, @ApproverId, @ApproverEmail, @ApprovedBy, @Approved, @CreatedBy, @Created)";
            await connection.ExecuteAsync(sql, invoice);
            return invoice;
        }

        public async Task<BulkInvoicesEntity?> CreateBulkAsync(BulkInvoicesEntity entities)
        {
            using var connection = await _dbContext.CreateConnectionAsync();
            var sql = "INSERT INTO Invoices (Id, SchemeType, Data, Reference, Value, Status, ApproverId, ApproverEmail, ApprovedBy, Approved, CreatedBy, Created) " +
            "VALUES (@Id, @SchemeType, @Data, @Reference, @Value, @Status, @ApproverId, @ApproverEmail, @ApprovedBy, @Approved, @CreatedBy, @Created)";
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
                    "ApproverId = @ApproverId, " +
                    "ApproverEmail = @ApproverEmail, " +
                    "ApprovedBy = @ApprovedBy, " +
                    "Approved = @Approved, " +
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

        private List<InvoiceEntity> getAllMockedInvoiceEntities()
        {
            return new List<InvoiceEntity>()
            {
                new InvoiceEntity()
                {
                    Id = "3841b9d8-1ea0-4e73-9b47-98eb6cbed75a",
                    SchemeType = "LS",
                    Data = "{\"id\":\"3841b9d8-1ea0-4e73-9b47-98eb6cbed75a\",\"accountType\":\"AR\",\"organisation\":\"NE\",\"paymentType\":\"EU\",\"schemeType\":\"LS\",\"paymentRequests\":[{\"paymentRequestId\":\"1_AA0YZHA0\",\"sourceSystem\":\"Manual\",\"frn\":1234567890,\"value\":19.5,\"currency\":\"GBP\",\"description\":null,\"originalInvoiceNumber\":\"A1\",\"originalSettlementDate\":\"2015-01-01T00:00:00+00:00\",\"recoveryDate\":\"2015-01-01T00:00:00+00:00\",\"invoiceCorrectionReference\":\"PR\",\"invoiceLines\":[{\"value\":10.25,\"fundCode\":\"EXQ00\",\"mainAccount\":\"SOS360\",\"schemeCode\":\"40161\",\"marketingYear\":2023,\"deliveryBody\":\"FC99\",\"description\":\"G00 - Gross value of claim\",\"currency\":null},{\"value\":9.25,\"fundCode\":\"EXQ00\",\"mainAccount\":\"SOS310\",\"schemeCode\":\"40164\",\"marketingYear\":2023,\"deliveryBody\":\"FC99\",\"description\":\"G00 - Gross value of claim\",\"currency\":null}],\"marketingYear\":2014,\"paymentRequestNumber\":1,\"agreementNumber\": \"1\",\"dueDate\":\"01/01/2015\",\"appendixReferences\":null,\"sbi\":0,\"vendor\":null}],\"status\":\"awaiting\",\"reference\":null,\"approverId\":\"2341b9d8-1ea0-4e73-9b47-98eb6cbed75a\",\"approverEmail\":\"test.email@defra.gov.uk\",\"approvedBy\":\"approver.email@defra.gov.uk\",\"approved\":\"2023-10-30T08:47:32.890385+00:00\",\"created\":\"2023-10-30T08:47:32.890385+00:00\",\"updated\":\"2023-10-30T08:49:59.4200863+00:00\",\"createdBy\":\"1\",\"updatedBy\":\"1\"}",
                    Reference = "123456789",
                    Value = 19.5M,
                    Status = "awaiting", // dont just change this status, create a new invoice with a different status
                    ApproverId = "2341b9d8-1ea0-4e73-9b47-98eb6cbed75a",
                    ApproverEmail = "test.email@defra.gov.uk",
                    ApprovedBy = "approver.email@defra.gov.uk",
                    Approved = new DateTime(2023, 11, 02, 11, 27, 45, 540, DateTimeKind.Utc),
                    CreatedBy = "1",
                    Created = new DateTime(2023, 11, 02, 11, 27, 45, 540, DateTimeKind.Utc),
                    UpdatedBy = "1",
                    Updated = new DateTime(2023, 11, 02, 11, 45, 52, 175, DateTimeKind.Utc),
                },
            };
        }
    }
}