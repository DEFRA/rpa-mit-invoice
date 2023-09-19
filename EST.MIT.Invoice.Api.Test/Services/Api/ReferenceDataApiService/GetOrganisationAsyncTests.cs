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

namespace EST.MIT.Invoice.Api.Test.Services.Api.ReferenceDataApiService
{
    public class ReferenceDataApiOrganisationTests
    {
        private readonly Mock<IReferenceDataRepository> _mockReferenceDataRepository;
        private readonly Mock<IHttpContentDeserializer> _httpContentDeserializerMock;
        private readonly string _accountType = "RPA";

        private readonly ReferenceDataApi _referenceDataApi;
        public ReferenceDataApiOrganisationTests()
        {
            _mockReferenceDataRepository = new Mock<IReferenceDataRepository>();
            _httpContentDeserializerMock = new Mock<IHttpContentDeserializer>();

            _httpContentDeserializerMock.Setup(x => x.DeserializeListAsync<Organisation>(It.IsAny<HttpContent>()))
             .ReturnsAsync(new List<Organisation>()
             {
                        new Organisation()
                        {
                            Code = "RPA",
                            Description =  "First Organisation"
                        },
                        new Organisation()
                        {
                            Code = "RPA",
                            Description =  "Second Organisation"
                        }
             });

            _referenceDataApi = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>(), _httpContentDeserializerMock.Object);
        }

        [Fact]
        public void GetOrganisationAsync_Returns_Valid_Organisation()
        {
            //Arrange
            _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new List<Organisation>()
                    {
                    new Organisation()
                    {
                        Code = "RPA",
                        Description =  "First Organisation"
                    },
                    new Organisation()
                    {
                        Code = "RPA",
                        Description =  "Second Organisation"
                    }
                    }))
                });

            //Act
            var response = _referenceDataApi.GetOrganisationsAsync(_accountType).Result;

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.IsSuccess.Should().BeTrue();
            response.Data.Should().BeOfType<List<Organisation>>();
            response.Data.Should().HaveCount(2);
            response.Data.Should().BeEquivalentTo(new List<Organisation>()
            {
                    new Organisation()
                    {
                        Code = "RPA",
                        Description =  "First Organisation"
                    },
                    new Organisation()
                    {
                        Code = "RPA",
                        Description =  "Second Organisation"
                    }
             });
        }

        [Fact]
        public void GetOrganisationsAsync_API_Returns_NoContent()
        {
            _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var response = _referenceDataApi.GetOrganisationsAsync(_accountType).Result;

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            response.IsSuccess.Should().BeFalse();
            response.Data.Should().BeNull();
        }

        [Fact]
        public void GetOrganisationsAsync_Deserialise_Fail()
        {
            _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("123")
            });

            var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>(), new HttpContentDeserializer());

            var response = service.GetOrganisationsAsync(_accountType).Result;

            response.IsSuccess.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            response.Data.Should().BeEmpty();
            response.Errors.Should().ContainKey("deserializing");
        }

        [Fact]
        public void GetOrganisationAsync_API_Returns_NotFound()
        {
            _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

            var response = _referenceDataApi.GetOrganisationsAsync(_accountType).Result;

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.IsSuccess.Should().BeFalse();
            response.Data.Should().BeNull();
        }

        [Fact]
        public void GetOrganisationAsync_API_Returns_BadRequest()
        {
            _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

            var response = _referenceDataApi.GetOrganisationsAsync(_accountType).Result;

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.IsSuccess.Should().BeFalse();
            response.Data.Should().BeNull();
            response.Errors.Should().ContainKey($"{HttpStatusCode.BadRequest}");
        }

        [Fact]
        public void GetOrganisationAsync_API_Returns_Unexpected()
        {
            _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage((HttpStatusCode)418));

            var response = _referenceDataApi.GetOrganisationsAsync(_accountType).Result;

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            response.IsSuccess.Should().BeFalse();
            response.Data.Should().BeNull();
            response.Errors.Should().ContainKey($"{HttpStatusCode.InternalServerError}");
        }

        [Fact]
        public async Task GetOrganisationAsync_ResponseDataTaskIsFaulted_LogsErrorAndHandlesException()
        {
            // Arrange
            var mockRepository = new Mock<IReferenceDataRepository>();
            var mockLogger = new Mock<ILogger<ReferenceDataApi>>();

            var responseData = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("BAD DATA", Encoding.UTF8, "application/json")

            };

            mockRepository.Setup(x => x.GetOrganisationsListAsync(It.IsAny<string>()))
                .ReturnsAsync(responseData);

            var service = new ReferenceDataApi(mockRepository.Object, mockLogger.Object, new FaultedHttpContentDeserializer());

            // Act
            var result = await service.GetOrganisationsAsync(_accountType);

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
        public async Task GetOrganisationAsync_ResponseDataIsNull_ReturnsNotFound()
        {
            // Arrange
            var mockRepository = new Mock<IReferenceDataRepository>();
            var mockLogger = new Mock<ILogger<ReferenceDataApi>>();

            var responseData = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[]", Encoding.UTF8, "application/json") // Empty array simulates no data
            };

            mockRepository.Setup(x => x.GetOrganisationsListAsync(It.IsAny<string>()))
                .ReturnsAsync(responseData);

            var service = new ReferenceDataApi(mockRepository.Object, mockLogger.Object, new HttpContentDeserializer());

            // Act
            var result = await service.GetOrganisationsAsync(_accountType);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
    }
}

