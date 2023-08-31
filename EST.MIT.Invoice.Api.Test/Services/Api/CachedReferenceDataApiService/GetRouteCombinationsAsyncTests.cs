using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Services.Api;
using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Util;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text;
using System.Text.Json;
using EST.MIT.Invoice.Api.Test.Services.Api.ReferenceDataApiService;
using NSubstitute;

namespace EST.MIT.Invoice.Api.Test.Services.Api.CachedReferenceDataApiService
{
    public class GetRouteCombinationsAsyncTests
    {
        private readonly IReferenceDataRepository _mockReferenceDataRepository;
        private readonly ILogger<CachedReferenceDataApi> _mockLogger;
        private readonly IHttpContentDeserializer _mockHttpContentDeserializer;
        private readonly IMemoryCache _mockMemoryCache;

        private CachedReferenceDataApi _service;

        private readonly string _invoiceType = "RPA";
        private readonly string _organisation = "EST";
        private readonly string _paymentType = "AP";
        private readonly string _schemeType = "BPS";

        public GetRouteCombinationsAsyncTests()
        {
            _mockReferenceDataRepository = Substitute.For<IReferenceDataRepository>();
            _mockLogger = Substitute.For<ILogger<CachedReferenceDataApi>>();
            _mockHttpContentDeserializer = Substitute.For<IHttpContentDeserializer>();
            _mockMemoryCache = Substitute.For<IMemoryCache>();

            var routeCombinations = new List<RouteCombination>()
            {
                new RouteCombination()
                {
                    AccountCode = "AccountCodeValue",
                    DeliveryBodyCode = "DeliveryBodyCodeValue",
                    SchemeCode = "SchemeCodeValue",
                },
            };

            _mockHttpContentDeserializer.DeserializeListAsync<RouteCombination>(Arg.Any<HttpContent>())
                .Returns(x => Task.FromResult((IEnumerable<RouteCombination>)routeCombinations));

            _mockReferenceDataRepository.GetRouteCombinationsListAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(routeCombinations), Encoding.UTF8, "application/json")
                }));

            _service = new CachedReferenceDataApi(_mockReferenceDataRepository, _mockLogger, _mockHttpContentDeserializer, _mockMemoryCache);
        }

        [Fact]
        public async Task GetRouteCombinationsAsync_ReturnsDataFromCache_WhenDataExists()
        {
            // Arrange
            var cacheKey = new { invoiceType = this._invoiceType, organisation = this._organisation, paymentType = this._paymentType, schemeType = this._schemeType };
            var cacheValue = new List<RouteCombination> { new RouteCombination() };

            // Set up NSubstitute
            _mockMemoryCache.TryGetValue(cacheKey, out Arg.Any<IEnumerable<RouteCombination>>())
                .Returns(x =>
                {
                    x[1] = cacheValue;  // Set the "out" parameter
                    return true;
                });

            // Act
            var result = await _service.GetRouteCombinationsAsync(this._invoiceType, this._organisation, this._paymentType, this._schemeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetRouteCombinationsAsync_ReturnsDataFromApi_WhenNotInCache()
        {
            // Arrange
            var cacheKey = new { invoiceType = this._invoiceType, organisation = this._organisation, paymentType = this._paymentType, schemeType = this._schemeType };

            // Mock cache to return false for TryGetValue indicating data is not in the cache.
            _mockMemoryCache.TryGetValue(cacheKey, out Arg.Any<IEnumerable<RouteCombination>>()).Returns(false);

            // Act
            var result = await _service.GetRouteCombinationsAsync(this._invoiceType, this._organisation, this._paymentType, this._schemeType);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();

            // The data should match what's returned from the API/repo mock.
            result.Data.First().AccountCode.Should().Be("AccountCodeValue");
            result.Data.First().DeliveryBodyCode.Should().Be("DeliveryBodyCodeValue");
            result.Data.First().SchemeCode.Should().Be("SchemeCodeValue");
        }

        [Fact]
        public async Task GetDeliveryBodyCodesAsync_API_Returns_NoContent()
        {
            // Arrange
            var cacheKey = new { invoiceType = this._invoiceType, organisation = this._organisation, paymentType = this._paymentType, schemeType = this._schemeType };

            // Mock cache to return false for TryGetValue indicating data is not in the cache.
            _mockMemoryCache.TryGetValue(cacheKey, out Arg.Any<IEnumerable<RouteCombination>>()).Returns(false);

            this._mockReferenceDataRepository.GetRouteCombinationsListAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

            // Act
            var result = await _service.GetRouteCombinationsAsync(this._invoiceType, this._organisation, this._paymentType, this._schemeType);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task GetDeliveryBodyCodesAsync_Deserialize_Fail()
        {
            // Arrange
            var cacheKey = new { invoiceType = this._invoiceType, organisation = this._organisation, paymentType = this._paymentType, schemeType = this._schemeType };

            // Mock cache to return false for TryGetValue indicating data is not in the cache.
            _mockMemoryCache.TryGetValue(cacheKey, out Arg.Any<IEnumerable<RouteCombination>>()).Returns(false);

            this._mockReferenceDataRepository.GetRouteCombinationsListAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("123")
                }));

            // Act
            _service = new CachedReferenceDataApi(_mockReferenceDataRepository, _mockLogger, new HttpContentDeserializer(), _mockMemoryCache);
            var result = await _service.GetRouteCombinationsAsync(this._invoiceType, this._organisation, this._paymentType, this._schemeType);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            result.Data.Should().BeEmpty();
            result.Errors.Should().ContainKey("deserializing");
        }

        [Fact]
        public async Task GetDeliveryBodyCodesAsync_API_Returns_NotFound()
        {
            // Arrange
            var cacheKey = new { invoiceType = this._invoiceType, organisation = this._organisation, paymentType = this._paymentType, schemeType = this._schemeType };

            // Mock cache to return false for TryGetValue indicating data is not in the cache.
            _mockMemoryCache.TryGetValue(cacheKey, out Arg.Any<IEnumerable<RouteCombination>>()).Returns(false);

            this._mockReferenceDataRepository.GetRouteCombinationsListAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)));

            // Act
            var result = await _service.GetRouteCombinationsAsync(this._invoiceType, this._organisation, this._paymentType, this._schemeType);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task GetDeliveryBodyCodesAsync_API_Returns_BadRequest()
        {
            // Arrange
            var cacheKey = new { invoiceType = this._invoiceType, organisation = this._organisation, paymentType = this._paymentType, schemeType = this._schemeType };

            // Mock cache to return false for TryGetValue indicating data is not in the cache.
            _mockMemoryCache.TryGetValue(cacheKey, out Arg.Any<IEnumerable<RouteCombination>>()).Returns(false);

            this._mockReferenceDataRepository.GetRouteCombinationsListAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)));

            // Act
            var result = await _service.GetRouteCombinationsAsync(this._invoiceType, this._organisation, this._paymentType, this._schemeType);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Errors.Should().ContainKey($"{HttpStatusCode.BadRequest}");
        }

        [Fact]
        public async Task GetDeliveryBodyCodesAsync_API_Returns_Unexpected()
        {
            // Arrange
            var cacheKey = new { invoiceType = this._invoiceType, organisation = this._organisation, paymentType = this._paymentType, schemeType = this._schemeType };

            // Mock cache to return false for TryGetValue indicating data is not in the cache.
            _mockMemoryCache.TryGetValue(cacheKey, out Arg.Any<IEnumerable<RouteCombination>>()).Returns(false);

            this._mockReferenceDataRepository.GetRouteCombinationsListAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => Task.FromResult(new HttpResponseMessage((HttpStatusCode)418)));

            // Act
            var result = await _service.GetRouteCombinationsAsync(this._invoiceType, this._organisation, this._paymentType, this._schemeType);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Errors.Should().ContainKey($"{HttpStatusCode.InternalServerError}");
        }

        [Fact]
        public async Task GetDeliveryBodyCodesAsync_ResponseDataTaskIsFaulted_LogsErrorAndHandlesException()
        {
            // Arrange
            var cacheKey = new { invoiceType = this._invoiceType, organisation = this._organisation, paymentType = this._paymentType, schemeType = this._schemeType };

            // Mock cache to return false for TryGetValue indicating data is not in the cache.
            _mockMemoryCache.TryGetValue(cacheKey, out Arg.Any<IEnumerable<RouteCombination>>()).Returns(false);

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("BAD DATA", Encoding.UTF8, "application/json")
            };

            this._mockReferenceDataRepository.GetRouteCombinationsListAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => Task.FromResult(response));

            var mockLogger = new Mock<ILogger<CachedReferenceDataApi>>();

            // Act
            _service = new CachedReferenceDataApi(_mockReferenceDataRepository, mockLogger.Object, new FaultedHttpContentDeserializer(), _mockMemoryCache);
            var result = await _service.GetRouteCombinationsAsync(this._invoiceType, this._organisation, this._paymentType, this._schemeType);

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
        public async Task GetDeliveryBodyCodesAsync_ResponseDataIsNull_ReturnsNotFound()
        {
            // Arrange
            var cacheKey = new { invoiceType = this._invoiceType, organisation = this._organisation, paymentType = this._paymentType, schemeType = this._schemeType };

            // Mock cache to return false for TryGetValue indicating data is not in the cache.
            _mockMemoryCache.TryGetValue(cacheKey, out Arg.Any<IEnumerable<RouteCombination>>()).Returns(false);

            var responseData = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[]", Encoding.UTF8, "application/json") // Empty array simulates no data
            };

            this._mockReferenceDataRepository.GetRouteCombinationsListAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => Task.FromResult(responseData));

            // Act
            _service = new CachedReferenceDataApi(_mockReferenceDataRepository, _mockLogger, new HttpContentDeserializer(), _mockMemoryCache);
            var result = await _service.GetRouteCombinationsAsync(this._invoiceType, this._organisation, this._paymentType, this._schemeType);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
    }
}
