using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Services.API.Models;
using EST.MIT.Invoice.Api.Services.Api;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text.Json;
using FluentAssertions;

namespace EST.MIT.Invoice.Api.Test.Services.Api;
public class ReferenceDataAPITests
{
    private readonly Mock<IReferenceDataRepository> _mockReferenceDataRepository;
    public ReferenceDataAPITests()
    {
        _mockReferenceDataRepository = new Mock<IReferenceDataRepository>();
    }

    [Fact]
    public void GetSchemesAsync_Returns_List_Organisation()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemesListAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<PaymentScheme>()
                {
                    new PaymentScheme()
                    {
                        Code = "RPA",
                        Description =  "A nice place to work"
                    },
                    new PaymentScheme()
                    {
                        Code = "AP",
                        Description =  "Another nice place to work"
                    }
                }))
            });

        var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());

        var response = service.GetSchemesAsync().Result;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeOfType<List<PaymentScheme>>();
        response.Data.Should().HaveCount(2);
        response.Data.Should().BeEquivalentTo(new List<PaymentScheme>()
        {
            new PaymentScheme()
            {
                Code = "RPA",
                Description =  "A nice place to work"
            },
            new PaymentScheme()
            {
                Code = "AP",
                Description =  "Another nice place to work"
            }
        });

    }

    [Fact]
    public void GetSchemesAsync_API_Returns_NoContent()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemesListAsync())
        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());

        var response = service.GetSchemesAsync().Result;

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public void GetSchemesAsync_Deserialise_Fail()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemesListAsync())
        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("123")
        });

        var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());

        var response = service.GetSchemesAsync().Result;

        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Data.Should().BeEmpty();
        response.Errors.Should().ContainKey("deserializing");
    }

    [Fact]
    public void GetSchemesAsync_API_Returns_NotFound()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemesListAsync())
        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());

        var response = service.GetSchemesAsync().Result;

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public void GetSchemesAsync_API_Returns_BadRequest()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemesListAsync())
        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());

        var response = service.GetSchemesAsync().Result;

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.BadRequest}");
    }

    [Fact]
    public void GetSchemesAsync_API_Returns_Unexpected()
    {
        _mockReferenceDataRepository.Setup(x => x.GetSchemesListAsync())
        .ReturnsAsync(new HttpResponseMessage((HttpStatusCode)418));

        var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());

        var response = service.GetSchemesAsync().Result;

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.InternalServerError}");
    }
}
