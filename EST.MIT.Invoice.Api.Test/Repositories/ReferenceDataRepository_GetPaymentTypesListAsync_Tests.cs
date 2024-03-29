﻿using EST.MIT.Invoice.Api.Repositories;
using FluentAssertions;
using Moq;
using Moq.Contrib.HttpClient;
using System.Net;

namespace EST.MIT.Invoice.Api.Test.Repositories;

public class ReferenceDataRepository_GetPaymentTypesListAsync_Tests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private string _accountType = "RPA";
    private string _organisation = "EST";
    private string _schemeType = "BPS";

    public ReferenceDataRepository_GetPaymentTypesListAsync_Tests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
    }

    [Fact]
    public void GetPaymentTypesListAsync_Returns_200()
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

        var response = repo.GetPaymentTypesListAsync(_accountType, _organisation, _schemeType);

        response.Result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("", "", "")]
    [InlineData("RPA", "", "")]
    [InlineData("", "EST", "")]
    [InlineData("", "", "BPS")]
    [InlineData("RPA", "EST", "")]
    [InlineData("RPA", "", "BPS")]
    [InlineData("", "EST", "BPS")]
    [InlineData("RPA", "EST", "BPS")]
    public void GetPaymentTypesListAsync_Returns_200_When_AccountType_Or_Organisation_Or_SchemeType_Different_Combos(string accountType, string organisation, string schemeType)
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

        var response = repo.GetPaymentTypesListAsync(accountType, organisation, schemeType);

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

        var response = repo.GetPaymentTypesListAsync(_accountType, _organisation, _schemeType);

        response.Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Result.Content.ReadAsStringAsync().Result.Should().Be("Test BadRequest");
    }
}
