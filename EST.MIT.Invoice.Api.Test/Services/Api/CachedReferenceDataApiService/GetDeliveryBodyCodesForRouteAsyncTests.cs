using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Services.Api;
using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Util;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text;
using System.Text.Json;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using EST.MIT.Invoice.Api.Test.Services.Api.ReferenceDataApiService;
using NSubstitute;

namespace EST.MIT.Invoice.Api.Test.Services.Api.CachedReferenceDataApiService
{
    public class GetDeliveryBodyCodesForRouteAsyncTests
    {
        private readonly IReferenceDataRepository _mockReferenceDataRepository;
        private readonly ILogger<CachedReferenceDataApi> _mockLogger;
        private readonly IHttpContentDeserializer _mockHttpContentDeserializer;
        private readonly ICacheService _mockCacheService;

        private CachedReferenceDataApi _service;

        private readonly string _accountType = "RPA";
        private readonly string _organisation = "EST";
        private readonly string _paymentType = "AP";
        private readonly string _schemeType = "BPS";
        private readonly List<DeliveryBodyCode> deliveryBodyCodes;


        public GetDeliveryBodyCodesForRouteAsyncTests()
        {
            _mockReferenceDataRepository = Substitute.For<IReferenceDataRepository>();
            _mockLogger = Substitute.For<ILogger<CachedReferenceDataApi>>();
            _mockHttpContentDeserializer = Substitute.For<IHttpContentDeserializer>();
            _mockCacheService = Substitute.For<ICacheService>();

            deliveryBodyCodes = new List<DeliveryBodyCode>()
            {
                new DeliveryBodyCode()
                {
                    Code = "RP00",
                    Description =  "England"
                },
                new DeliveryBodyCode()
                {
                    Code = "RP01",
                    Description =  "Scotland"
                }
            };

            _mockHttpContentDeserializer.DeserializeListAsync<DeliveryBodyCode>(Arg.Any<HttpContent>())
                .Returns(x => Task.FromResult((IEnumerable<DeliveryBodyCode>)deliveryBodyCodes));

            _mockReferenceDataRepository.GetDeliveryBodyCodesListAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(deliveryBodyCodes), Encoding.UTF8, "application/json")
                }));
            _mockCacheService.GetData<IEnumerable<DeliveryBodyCode>?>(Arg.Any<object>())
                .Returns(x => null);

            _service = new CachedReferenceDataApi(_mockReferenceDataRepository, _mockLogger, _mockHttpContentDeserializer, _mockCacheService);
        }

        [Fact]
        public async Task GetDeliveryBodyCodesForRouteAsync_ReturnsDataFromCache_WhenDataExists()
        {
            // Arrange
            _mockCacheService.GetData<IEnumerable<DeliveryBodyCode>?>(Arg.Any<object>())
                .Returns(x => deliveryBodyCodes);

            // Act
            var result = await _service.GetDeliveryBodyCodesForRouteAsync(this._accountType, this._organisation, this._paymentType, this._schemeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetCombinationsListForRouteAsync_ReturnsDataFromCache_WhenDataExists_TheSecondTime()
        {
            // Arrange
            _mockCacheService.GetData<IEnumerable<DeliveryBodyCode>?>(Arg.Any<object>())
                .Returns(x => null, x => deliveryBodyCodes);

            // Act
            var result = await _service.GetCombinationsListForRouteAsync(this._accountType, this._organisation, this._paymentType, this._schemeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetDeliveryBodyCodesForRouteAsync_ReturnsDataFromApi_WhenNotInCache()
        {
            // Act
            var result = await _service.GetDeliveryBodyCodesForRouteAsync(this._accountType, this._organisation, this._paymentType, this._schemeType);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();

            // The data should match what's returned from the API/repo mock.
            var deliveryBodyCodesResults = result.Data.ToList();
            deliveryBodyCodesResults.Should().HaveCount(2);
            deliveryBodyCodesResults[0].Code.Should().Be("RP00");
            deliveryBodyCodesResults[0].Description.Should().Be("England");
            deliveryBodyCodesResults[1].Code.Should().Be("RP01");
            deliveryBodyCodesResults[1].Description.Should().Be("Scotland");
        }

        [Fact]
        public async Task GetDeliveryBodyCodesForRouteAsync_API_Returns_NoContent()
        {
            // Arrange
            this._mockReferenceDataRepository.GetDeliveryBodyCodesListAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

            // Act
            var result = await _service.GetDeliveryBodyCodesForRouteAsync(this._accountType, this._organisation, this._paymentType, this._schemeType);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task GetDeliveryBodyCodesForRouteAsync_Deserialize_Fail()
        {
            // Arrange
            this._mockReferenceDataRepository.GetDeliveryBodyCodesListAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("123")
                }));

            // Act
            _service = new CachedReferenceDataApi(_mockReferenceDataRepository, _mockLogger, new HttpContentDeserializer(), _mockCacheService);
            var result = await _service.GetDeliveryBodyCodesForRouteAsync(this._accountType, this._organisation, this._paymentType, this._schemeType);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            result.Data.Should().BeEmpty();
            result.Errors.Should().ContainKey("deserializing");
        }

        [Fact]
        public async Task GetDeliveryBodyCodesForRouteAsync_API_Returns_NotFound()
        {
            // Arrange
            this._mockReferenceDataRepository.GetDeliveryBodyCodesListAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)));

            // Act
            var result = await _service.GetDeliveryBodyCodesForRouteAsync(this._accountType, this._organisation, this._paymentType, this._schemeType);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task GetDeliveryBodyCodesForRouteAsync_API_Returns_BadRequest()
        {
            // Arrange
            this._mockReferenceDataRepository.GetDeliveryBodyCodesListAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)));

            // Act
            var result = await _service.GetDeliveryBodyCodesForRouteAsync(this._accountType, this._organisation, this._paymentType, this._schemeType);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Errors.Should().ContainKey($"{HttpStatusCode.BadRequest}");
        }

        [Fact]
        public async Task GetDeliveryBodyCodesForRouteAsync_API_Returns_Unexpected()
        {
            // Arrange
            this._mockReferenceDataRepository.GetDeliveryBodyCodesListAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => Task.FromResult(new HttpResponseMessage((HttpStatusCode)418)));

            // Act
            var result = await _service.GetDeliveryBodyCodesForRouteAsync(this._accountType, this._organisation, this._paymentType, this._schemeType);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Errors.Should().ContainKey($"{HttpStatusCode.InternalServerError}");
        }

        [Fact]
        public async Task GetDeliveryBodyCodesForRouteAsync_ResponseDataTaskIsFaulted_LogsErrorAndHandlesException()
        {
            // Arrange
            var mockRepository = new Mock<IReferenceDataRepository>();
            var mockLogger = new Mock<ILogger<CachedReferenceDataApi>>();

            var responseData = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("BAD DATA", Encoding.UTF8, "application/json")

            };

            mockRepository.Setup(x => x.GetDeliveryBodyCodesListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(responseData);

            var service = new CachedReferenceDataApi(mockRepository.Object, mockLogger.Object, new FaultedHttpContentDeserializer(), _mockCacheService);

            // Act
            var result = await service.GetDeliveryBodyCodesForRouteAsync(this._accountType, this._organisation, this._paymentType, this._schemeType);

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
        public async Task GetDeliveryBodyCodesForRouteAsync_ResponseDataIsNull_ReturnsNotFound()
        {
            // Arrange
            var responseData = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[]", Encoding.UTF8, "application/json") // Empty array simulates no data
            };

            this._mockReferenceDataRepository.GetDeliveryBodyCodesListAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => Task.FromResult(responseData));

            // Act
            _service = new CachedReferenceDataApi(_mockReferenceDataRepository, _mockLogger, new HttpContentDeserializer(), _mockCacheService);
            var result = await _service.GetDeliveryBodyCodesForRouteAsync(this._accountType, this._organisation, this._paymentType, this._schemeType);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
    }
}
