using System.Net;
using System.Text;
using System.Text.Json;
using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Services.Api;
using EST.MIT.Invoice.Api.Services.API.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace EST.MIT.Invoice.Api.Test.Services.Api.ReferenceDataApiService;
public class GetPaymentTypesAsyncTests
{
    private readonly Mock<IReferenceDataRepository> _mockReferenceDataRepository;
    private string _invoiceType = "RPA";
    private string _organisation = "EST";
    private string _schemeType = "BPS";

    public GetPaymentTypesAsyncTests()
    {
        _mockReferenceDataRepository = new Mock<IReferenceDataRepository>();
    }

    [Fact]
    public void GetPaymentTypesAsync_Returns_List()
    {
        _mockReferenceDataRepository.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
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

        var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());

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
        _mockReferenceDataRepository.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());

        var response = service.GetPaymentTypesAsync(_invoiceType, _organisation, _schemeType).Result;

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public void GetPaymentTypesAsync_Deserialise_Fail()
    {
        _mockReferenceDataRepository.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("123")
        });

        var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());

        var response = service.GetPaymentTypesAsync(_invoiceType, _organisation, _schemeType).Result;

        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Data.Should().BeEmpty();
        response.Errors.Should().ContainKey("deserializing");
    }

    [Fact]
    public void GetPaymentTypesAsync_API_Returns_NotFound()
    {
        _mockReferenceDataRepository.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());

        var response = service.GetPaymentTypesAsync(_invoiceType, _organisation, _schemeType).Result;

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
    }

    [Fact]
    public void GetPaymentTypesAsync_API_Returns_BadRequest()
    {
        _mockReferenceDataRepository.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());

        var response = service.GetPaymentTypesAsync(_invoiceType, _organisation, _schemeType).Result;

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.BadRequest}");
    }

    [Fact]
    public void GetPaymentTypesAsync_API_Returns_Unexpected()
    {
        _mockReferenceDataRepository.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .ReturnsAsync(new HttpResponseMessage((HttpStatusCode)418));

        var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());

        var response = service.GetPaymentTypesAsync(_invoiceType, _organisation, _schemeType).Result;

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Errors.Should().ContainKey($"{HttpStatusCode.InternalServerError}");
    }

    [Fact]
    public async Task GetPaymentTypesAsync_ResponseDataTaskIsFaulted_ThrowsException()
    {
        // Arrange
        var mockRepository = new Mock<IReferenceDataRepository>();
        var mockLogger = new Mock<ILogger<ReferenceDataApi>>();

        var responseData = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("", Encoding.UTF8, "application/json")
        };

        mockRepository.Setup(x => x.GetPaymentTypesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(() => throw new InvalidOperationException());

        var service = new ReferenceDataApi(mockRepository.Object, mockLogger.Object);

        // Act and Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.GetPaymentTypesAsync(_invoiceType, _organisation, _schemeType));
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

        var service = new ReferenceDataApi(mockRepository.Object, mockLogger.Object);

        // Act
        var result = await service.GetPaymentTypesAsync(_invoiceType, _organisation, _schemeType);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }


}
