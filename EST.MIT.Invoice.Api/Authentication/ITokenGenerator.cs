using Azure.Core;

namespace EST.MIT.Invoice.Api.Authentication;

#pragma warning disable 1591

public interface ITokenGenerator
{
    Task<AccessToken> GetTokenAsync(string postgresSqlAAD, CancellationToken cancellationToken = default);
}