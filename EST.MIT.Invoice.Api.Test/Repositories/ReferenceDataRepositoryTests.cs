using EST.MIT.Invoice.Api.Repositories;
using FluentAssertions;
using Moq;
using Moq.Contrib.HttpClient;
using System.Net;

namespace EST.MIT.Invoice.Api.Test.Repositories;

public class ReferenceDataRepositoryTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private string _invoiceType = "RPA";
    private string _organisation = "EST";

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

        var response = repo.GetSchemesListAsync(_invoiceType, _organisation);

        response.Result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("RPA", "")]
    [InlineData("", "EST")]
    [InlineData("RPA", "EST")]
    public void GetSchemesListAsync_Returns_200_When_InvoiceType_Or_Organisation_Different_Combos(string invoiceType, string organisation)
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

        var response = repo.GetSchemesListAsync(invoiceType, organisation);

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

        var response = repo.GetSchemesListAsync(_invoiceType, _organisation);

        response.Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Result.Content.ReadAsStringAsync().Result.Should().Be("Test BadRequest");
    }
}
