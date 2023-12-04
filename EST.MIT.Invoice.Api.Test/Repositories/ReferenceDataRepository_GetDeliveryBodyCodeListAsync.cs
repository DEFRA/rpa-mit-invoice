using EST.MIT.Invoice.Api.Repositories;
using FluentAssertions;
using Moq;
using Moq.Contrib.HttpClient;
using System.Net;

namespace EST.MIT.Invoice.Api.Test.Repositories
{
    public class ReferenceDataRepository_GetDeliveryBodyCodeListAsync
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private string _accountType = "RPA";
        private string _organisation = "EST";
        private string _paymentType = "AP";
        private string _schemeType = "BPS";
        public ReferenceDataRepository_GetDeliveryBodyCodeListAsync()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        }

        [Fact]
        public async Task GetDeliveryBodyCodesListAsync_Returns_200()
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

            var response = await repo.GetDeliveryBodyCodesListAsync(_accountType, _organisation, _paymentType, _schemeType);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("", "", "", "")]
        [InlineData("RPA", "", "", "")]
        [InlineData("", "EST", "", "")]
        [InlineData("", "", "AP", "")]
        [InlineData("", "", "", "BPS")]
        [InlineData("RPA", "EST", "", "")]
        [InlineData("RPA", "", "", "BPS")]
        [InlineData("", "EST", "PA", "")]
        [InlineData("", "EST", "", "BPS")]
        [InlineData("RPA", "EST", "PA", "")]
        [InlineData("", "EST", "AP", "BPS")]
        [InlineData("RPA", "EST", "", "BPS")]
        [InlineData("RPA", "EST", "AP", "BPS")]
        public async Task GetDeliveryBodyCodesListAsync_Returns_200_When_AccountType_Or_Organisation_Or_SchemeType_Or_PaymentType_Different_Combos(string accountType, string organisation, string paymentType, string schemeType)
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

            var response = await repo.GetDeliveryBodyCodesListAsync(accountType, organisation, paymentType, schemeType);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task HandleHttpResponseError_Handed_FailCode()
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

            var response = await repo.GetDeliveryBodyCodesListAsync(_accountType, _organisation, _paymentType, _schemeType);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.Content.ReadAsStringAsync().Result.Should().Be("Test BadRequest");
        }
    }
}

