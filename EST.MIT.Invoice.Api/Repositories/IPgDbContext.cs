using System.Data;

namespace EST.MIT.Invoice.Api.Repositories;

public interface IPgDbContext
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);

    Task Init();
}