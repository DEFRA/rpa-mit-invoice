using System.Net;
using System.Text;
using System.Text.Json;
using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Services.Api;
using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Util;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace EST.MIT.Invoice.Api.Test.Services.Api.ReferenceDataApiService;
public class GetPaymentTypesAsyncTests
{
    private readonly Mock<IReferenceDataRepository> _mockReferenceDataRepositoryMock;
    private readonly Mock<IHttpContentDeserializer> _httpContentDeserializerMock;
    private readonly string _invoiceType = "RPA";
    private readonly string _organisation = "EST";
    private readonly string _schemeType = "BPS";

    public GetPaymentTypesAsyncTests()
    {
        _mockReferenceDataRepositoryMock = new Mock<IReferenceDataRepository>();
        _httpContentDeserializerMock = new Mock<IHttpContentDeserializer>();

        _httpContentDeserializerMock.Setup(x => x.DeserializeListAsync<PaymentType>(It.IsAny<HttpContent>()))
            .ReturnsAsync(new List<PaymentType>()
            {
                new PaymentType()
                {
                    Code = "DOM",
                    Description =  "Domestic"
                },
                new PaymentType()
                {
                    Code = "EU",
                    Description =  "European Union"
                }
            });
    }

    [Fact]
    public void GetPaymentTypesAsync_Returns_List()
    {
        _mockReferenceDataRepositoryMock.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<PaymentType>()
                {
                    new PaymentType()
                    {
                        Code = "DOM",
                        Description =  "Domestic"
                    },
                    new PaymentType()
                    {
                        Code = "EU",
                        Description =  "European Union"
                    }
                }))
            });

        var service = new ReferenceDataApi(_mockReferenceDataRepositoryMock.Object, Mock.Of<ILogger<ReferenceDataApi>>(), _httpContentDeserializerMock.Object);

        var response = service.GetPaymentTypesAsync(_invoiceType, _organisation, _schemeType).Result;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeOfType<List<PaymentType>>();
        response.Data.Should().HaveCount(2);
        response.Data.Should().BeEquivalentTo(new List<PaymentType>()
        {
            new PaymentType()
            {
                Code = "DOM",
                Description =  "Domestic"
            },
            new PaymentType()
            {
                Code = "EU",
                Description =  "European Union"
            }
        });

    }

    [Fact]
    public void GetPaymentTypesAsync_API_Returns_NoContent()
    {
        _mockReferenceDataRepositoryMock.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new ReferenceDataApi(_mockReferenceDataRepositoryMock.Object, Mock.Of<ILogger<ReferenceDataApi>>(), _httpContentDeserializerMock.Object);

        var response = service.GetPaymentTypesAsync(_invoiceType, _organisation, _schemeType).Result;

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public void GetPaymentTypesAsync_Deserialise_Fail()
    {
        _mockReferenceDataRepositoryMock.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("123")
        });

        var service = new ReferenceDataApi(_mockReferenceDataRepositoryMock.Object, Mock.Of<ILogger<ReferenceDataApi>>(), new HttpContentDeserializer());

        var response = service.GetPaymentTypesAsync(_invoiceType, _organisation, _schemeType).Result;

        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Data.Should().BeEmpty();
        response.Errors.Should().ContainKey("deserializing");
    }

    [Fact]
    public void GetPaymentTypesAsync_API_Returns_NotFound()
    {
        _mockReferenceDataRepositoryMock.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var service = new ReferenceDataApi(_mockReferenceDataRepositoryMock.Object, Mock.Of<ILogger<ReferenceDataApi>>(), _httpContentDeserializerMock.Object);

        var response = service.GetPaymentTypesAsync(_invoiceType, _organisation, _schemeType).Result;

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public void GetPaymentTypesAsync_API_Returns_BadRequest()
    {
        _mockReferenceDataRepositoryMock.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new ReferenceDataApi(_mockReferenceDataRepositoryMock.Object, Mock.Of<ILogger<ReferenceDataApi>>(), _httpContentDeserializerMock.Object);

        var response = service.GetPaymentTypesAsync(_invoiceType, _organisation, _schemeType).Result;

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.BadRequest}");
    }

    [Fact]
    public void GetPaymentTypesAsync_API_Returns_Unexpected()
    {
        _mockReferenceDataRepositoryMock.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .ReturnsAsync(new HttpResponseMessage((HttpStatusCode)418));

        var service = new ReferenceDataApi(_mockReferenceDataRepositoryMock.Object, Mock.Of<ILogger<ReferenceDataApi>>(), _httpContentDeserializerMock.Object);

        var response = service.GetPaymentTypesAsync(_invoiceType, _organisation, _schemeType).Result;

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.InternalServerError}");
    }

    [Fact]
    public async Task GetPaymentTypesAsync_ResponseDataTaskIsFaulted_LogsErrorAndHandlesException()
    {
        // Arrange
        var mockRepository = new Mock<IReferenceDataRepository>();
        var mockLogger = new Mock<ILogger<ReferenceDataApi>>();

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("BAD DATA", Encoding.UTF8, "application/json")
        };

        mockRepository
            .Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(response);

        var service = new ReferenceDataApi(mockRepository.Object, mockLogger.Object, new FaultedHttpContentDeserializer());

        // Act
        var result = await service.GetPaymentTypesAsync(_invoiceType, _organisation, _schemeType);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeEmpty();
        result.Errors.Should().ContainKey($"deserializing");

        var errors = result.Errors["deserializing"].ToList();

        Assert.Equal("An error occurred while processing the response.", errors[0]);

        // Verify error logging
        mockLogger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Exactly(2));
    }

    [Fact]
    public async Task GetPaymentTypesAsync_ResponseDataIsNull_ReturnsNotFound()
    {
        // Arrange
        var mockRepository = new Mock<IReferenceDataRepository>();
        var mockLogger = new Mock<ILogger<ReferenceDataApi>>();

        var responseData = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("[]", Encoding.UTF8, "application/json") // Empty array simulates no data
        };

        mockRepository.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(responseData);

        var service = new ReferenceDataApi(mockRepository.Object, mockLogger.Object, new HttpContentDeserializer());

        // Act
        var result = await service.GetPaymentTypesAsync(_invoiceType, _organisation, _schemeType);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }


}
