using EST.MIT.Invoice.Api.Services.API.Models;
using System.Net;

namespace EST.MIT.Invoice.Api.Test.Services.Api.Models
{
    public class ApiResponseTest
    {
        public class ApiResponseTests
        {
            [Fact]
            public void ApiResponse_ConstructorWithoutErrors_CreatesObject()
            {
                // Arrange
                var success = true;
                var statusCode = HttpStatusCode.OK;

                // Act
                var apiResponse = new ApiResponse(success, statusCode);

                // Assert
                Assert.True(apiResponse.IsSuccess);
                Assert.Equal(statusCode, apiResponse.StatusCode);
                Assert.Empty(apiResponse.Errors);
            }

            [Fact]
            public void ApiResponse_ConstructorWithErrors_CreatesObject()
            {
                // Arrange
                var success = false;
                var statusCode = HttpStatusCode.BadRequest;
                var errors = new Dictionary<string, string> { { "Error", "Bad request" } };

                // Act
                var apiResponse = new ApiResponse(success, statusCode, errors);

                // Assert
                Assert.False(apiResponse.IsSuccess);
                Assert.Equal(statusCode, apiResponse.StatusCode);
                Assert.Equal(errors, apiResponse.Errors);
            }

            [Fact]
            public void ApiResponse_DataProperty_SetAndGet()
            {
                // Arrange
                var success = true;
                var statusCode = HttpStatusCode.OK;
                var data = "Test Data";
                var apiResponse = new ApiResponse(success, statusCode);

                // Act
                apiResponse.Data = data;

                // Assert
                Assert.Equal(data, apiResponse.Data);
            }

            [Fact]
            public void ApiResponse_ErrorsProperty_SetAndGet()
            {
                // Arrange
                var success = true;
                var statusCode = HttpStatusCode.OK;
                var errors = new Dictionary<string, string> { { "Error", "New error" } };
                var apiResponse = new ApiResponse(success, statusCode);

                // Act
                apiResponse.Errors = errors;

                // Assert
                Assert.Equal(errors, apiResponse.Errors);
            }
        }


        [Fact]
        public void ApiResponse_DefaultConstructor_ShouldInitializeCorrectly()
        {
            var apiResponse = new ApiResponse<TestData>(false);

            Assert.False(apiResponse.IsSuccess);
            Assert.Equal(default, apiResponse.StatusCode);
            Assert.Null(apiResponse.Data);
            Assert.NotNull(apiResponse.Errors);
            Assert.Empty(apiResponse.Errors);
        }

        [Fact]
        public void ApiResponse_SuccessAndDataConstructor_ShouldInitializeCorrectly()
        {
            var testData = new TestData { Id = 1, Name = "Test" };

            var apiResponse = new ApiResponse<TestData>(true)
            {
                Data = testData
            };

            Assert.True(apiResponse.IsSuccess);
            Assert.Equal(default, apiResponse.StatusCode);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal(testData, apiResponse.Data);
            Assert.NotNull(apiResponse.Errors);
            Assert.Empty(apiResponse.Errors);
        }

        [Fact]
        public void ApiResponse_SuccessStatusCodeAndErrorsConstructor_ShouldInitializeCorrectly()
        {
            var errors = new Dictionary<string, List<string>>
            {
                { "field1", new List<string> { "Error 1", "Error 2" } },
                { "field2", new List<string> { "Error 3" } }
            };

            var apiResponse = new ApiResponse<TestData>(HttpStatusCode.BadRequest, errors);

            Assert.False(apiResponse.IsSuccess);
            Assert.Equal(HttpStatusCode.BadRequest, apiResponse.StatusCode);
            Assert.Null(apiResponse.Data);
            Assert.NotNull(apiResponse.Errors);
            Assert.Equal(2, apiResponse.Errors.Count);
            Assert.True(apiResponse.Errors.ContainsKey("field1"));
            Assert.True(apiResponse.Errors.ContainsKey("field2"));
            Assert.Equal(2, apiResponse.Errors["field1"].Count);
            Assert.Single(apiResponse.Errors["field2"]);
        }

        [Fact]
        public void ApiResponse_SuccessAndStatusCodeConstructor_ShouldInitializeCorrectly()
        {
            var apiResponse = new ApiResponse<TestData>(HttpStatusCode.OK);

            Assert.True(apiResponse.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
            Assert.Null(apiResponse.Data);
            Assert.NotNull(apiResponse.Errors);
            Assert.Empty(apiResponse.Errors);
        }

        [Fact]
        public void ApiResponse_SetData_ShouldSetDataProperty()
        {
            var apiResponse = new ApiResponse<TestData>(HttpStatusCode.BadRequest, new Dictionary<string, List<string>>());
            var testData = new TestData { Id = 1, Name = "Test" };

            apiResponse.Data = testData;

            Assert.NotNull(apiResponse.Data);
            Assert.Equal(testData, apiResponse.Data);
        }
    }

    public class TestData
    {
        public int Id { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
