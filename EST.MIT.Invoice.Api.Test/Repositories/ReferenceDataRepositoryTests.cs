using EST.MIT.Invoice.Api.Repositories;
using FluentAssertions;
using Moq;
using Moq.Contrib.HttpClient;
using System.Net;

namespace EST.MIT.Invoice.Api.Test.Repositories;

public class ReferenceDataRepositoryTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

    public ReferenceDataRepositoryTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
    }

    [Fact]
    public void GetSchemesListAsync_Returns_200()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new ReferenceDataRepository(factory);

        var response = repo.GetSchemesListAsync();

        response.Result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void HandleHttpResponseError_Handed_FailCode()
    {
        _mockHttpMessageHandler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.BadRequest, "Test BadRequest");

        var factory = _mockHttpMessageHandler.CreateClientFactory();

        Mock.Get(factory).Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() =>
        {
            var client = _mockHttpMessageHandler.CreateClient();
            client.BaseAddress = new Uri("https://localhost");
            return client;
        });

        var repo = new ReferenceDataRepository(factory);

        var response = repo.GetSchemesListAsync();

        response.Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Result.Content.ReadAsStringAsync().Result.Should().Be("Test BadRequest");
    }
}
