﻿using EST.MIT.Invoice.Api.Repositories;
using FluentAssertions;
using Moq;
using Moq.Contrib.HttpClient;
using System.Net;

namespace EST.MIT.Invoice.Api.Test.Repositories;

public class ReferenceDataRepository_GetSchemeTypesListAsync_Tests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private string _accountType = "RPA";
    private string _organisation = "EST";

    public ReferenceDataRepository_GetSchemeTypesListAsync_Tests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
    }

    [Fact]
    public void GetSchemeTypesListAsync_Returns_200()
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

        var response = repo.GetSchemeTypesListAsync(_accountType, _organisation);

        response.Result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("RPA", "")]
    [InlineData("", "EST")]
    [InlineData("RPA", "EST")]
    public void GetSchemeTypesListAsync_Returns_200_When_AccountType_Or_Organisation_Different_Combos(string accountType, string organisation)
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

        var response = repo.GetSchemeTypesListAsync(accountType, organisation);

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

        var response = repo.GetSchemeTypesListAsync(_accountType, _organisation);

        response.Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Result.Content.ReadAsStringAsync().Result.Should().Be("Test BadRequest");
    }

    [Fact]
    public void GetOrganisationListAsync_Returns_200()
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

        var response = repo.GetOrganisationsListAsync(_accountType);

        response.Result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("")]
    [InlineData("RPA")]
    public void GetOrganisationListAsync_Returns_200_When_AccountType(string accountType)
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

        var response = repo.GetOrganisationsListAsync(accountType);

        response.Result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void HandleHttpResponseErrorForOrganisation_Handed_FailCode()
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

        var response = repo.GetOrganisationsListAsync(_accountType);

        response.Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Result.Content.ReadAsStringAsync().Result.Should().Be("Test BadRequest");
    }

}
