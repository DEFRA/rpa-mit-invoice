using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EST.MIT.Invoice.Api.Repositories
{
	public class PgDbContext
	{
        private PgDbSettings _dbSettings;

        public PgDbContext(PgDbSettings dbSettings)
        {
            _dbSettings = dbSettings;
        }

        public IDbConnection CreateConnection()
        {
            var connectionString = $"Host={_dbSettings.Server}; Database={_dbSettings.Database}; Username={_dbSettings.Username}; Password={_dbSettings.Password}; Port={_dbSettings.Port};";
            return new NpgsqlConnection(connectionString);
        }

        public async Task Init()
        {
            await _initTables();
        }

        private async Task _initTables()
        {
            // create table and index if they don't exist
            using var connection = CreateConnection();

            var sql = "CREATE TABLE IF NOT EXISTS Invoices (" +
                    "Id VARCHAR, " +
                    "SchemeType VARCHAR, " +
                    "Data VARCHAR, " +
                    "Reference VARCHAR, " +
                    "Value DECIMAL(13,2), " +
                    "Status VARCHAR, " +
                    "CreatedBy VARCHAR, " +
                    "UpdatedBy VARCHAR, " +
                    "Created TIMESTAMP, " +
                    "Updated TIMESTAMP, " +
                    "PRIMARY KEY (Id, SchemeType));";
            await connection.ExecuteAsync(sql);

            sql = "CREATE UNIQUE INDEX IF NOT EXISTS invoices_indx1 on Invoices (id, schemeType);";
            await connection.ExecuteAsync(sql);
        }
    }
}