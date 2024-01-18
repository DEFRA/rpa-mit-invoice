using EST.MIT.Invoice.Api.Authentication;
using EST.MIT.Invoice.Api.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;

namespace RPA.MIT.ReferenceData.Api.Test.Authentication;

public class PgDbContextTests
{
    private readonly PgDbSettings _config = new();

    public PgDbContextTests()
    {
        _config.Server = "ahost.com";
        _config.Database = "a_database";
        _config.Username = "a_user";
        _config.Password = "a_password";
        _config.Port = "5432";
        _config.PostgresSqlAAD = "https://ossrdbms-aad.database.windows.net/.default";
    }

    [Fact]
    public async Task CreateConnectionAsync_NoTokenInCache_ReturnsConnection()
    {
        Mock<ITokenGenerator> mockTokenService = new();
        var connectionInterceptor = new PgDbContext(_config, mockTokenService.Object, true);
        var cancelToken = new CancellationToken();

        var connection = await connectionInterceptor.CreateConnectionAsync(cancelToken);
        mockTokenService.Verify(d => d.GetTokenAsync("https://ossrdbms-aad.database.windows.net/.default", cancelToken), Times.Once);

        Assert.NotNull(connection);
    }

    [Fact]
    public async Task CreateConnectionAsync_ExpiredTokenInCache_ReturnsConnection()
    {
        Mock<ITokenGenerator> mockTokenService = new();
        var connectionInterceptor = new PgDbContext(_config, mockTokenService.Object, true);
        var cancelToken = new CancellationToken();

        SetUpTokenService(mockTokenService);
        var connection = await connectionInterceptor.CreateConnectionAsync(cancelToken);
        mockTokenService.Verify(d => d.GetTokenAsync("https://ossrdbms-aad.database.windows.net/.default", cancelToken), Times.Once);

        Assert.NotNull(connection);
        TokenCache.AccessToken = null;
    }

    [Fact]
    public async Task CreateConnectionAsync_ValidTokenInCache_ReturnsConnection()
    {
        Mock<ITokenGenerator> mockTokenService = new();
        var connectionInterceptor = new PgDbContext(_config, mockTokenService.Object, true);
        var cancelToken = new CancellationToken();

        SetUpTokenService(mockTokenService);
        await connectionInterceptor.CreateConnectionAsync(cancelToken);

        var connection = await connectionInterceptor.CreateConnectionAsync(cancelToken);

        mockTokenService.Verify(d => d.GetTokenAsync("https://ossrdbms-aad.database.windows.net/.default", cancelToken), Times.Once);
        Assert.NotNull(connection);
        TokenCache.AccessToken = null;
    }

    [Fact]
    public async Task CreateConnectionAsync_NotProd_ReturnsConnectionWithoutToken()
    {
        Mock<ITokenGenerator> mockTokenService = new();
        var connectionInterceptor = new PgDbContext(_config, mockTokenService.Object, false);
        var cancelToken = new CancellationToken();

        var connection = await connectionInterceptor.CreateConnectionAsync(cancelToken);

        mockTokenService.Verify(d => d.GetTokenAsync("https://ossrdbms-aad.database.windows.net/.default", cancelToken), Times.Never);
        Assert.NotNull(connection);
    }

    private void SetUpTokenService(Mock<ITokenGenerator> mockTokenService)
    {
        mockTokenService.Setup(t => t.GetTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Azure.Core.AccessToken("a_token", DateTime.Now.AddDays(1)));
    }
}