using System.Data;
using Dapper;
using EST.MIT.Invoice.Api.Authentication;
using Npgsql;

namespace EST.MIT.Invoice.Api.Repositories
{
    public class PgDbContext : IPgDbContext
    {
        private readonly PgDbSettings _dbSettings;
        private readonly bool _isProd;
        private readonly ITokenGenerator _tokenService;

        public PgDbContext(PgDbSettings dbSettings, ITokenGenerator tokenService, bool isProd)
        {
            _dbSettings = dbSettings;
            _isProd = isProd;
            _tokenService = tokenService;
        }

        public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
        {
            if (_isProd)
            {
                if ((!TokenCache.AccessToken.HasValue) || (DateTime.Now >= TokenCache.AccessToken.Value.ExpiresOn.AddMinutes(-1)))
                {
                    TokenCache.AccessToken = await _tokenService.GetTokenAsync(_dbSettings.PostgresSqlAAD!, cancellationToken);
                }

                _dbSettings.Password = TokenCache.AccessToken.Value.Token;
            }

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
            using var connection = await CreateConnectionAsync();

            var sql = "CREATE TABLE IF NOT EXISTS Invoices (" +
                    "Id VARCHAR, " +
                    "SchemeType VARCHAR, " +
                    "Data VARCHAR, " +
                    "Reference VARCHAR, " +
                    "Value DECIMAL(13,2), " +
                    "Status VARCHAR, " +
                    "ApproverId VARCHAR, " +
                    "ApproverEmail VARCHAR, " +
                    "ApprovedBy VARCHAR, " +
                    "Approved TIMESTAMP, " +
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