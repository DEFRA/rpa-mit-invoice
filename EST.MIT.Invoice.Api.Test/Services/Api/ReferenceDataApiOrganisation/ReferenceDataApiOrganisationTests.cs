using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Services.Api;
using EST.MIT.Invoice.Api.Services.API.Models;
using FluentAssertions;
using Invoices.Api.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EST.MIT.Invoice.Api.Test.Services.Api.ReferenceDataApiOrganisation
{
    public class ReferenceDataApiOrganisationTests
    {
        private readonly Mock<IReferenceDataRepository> _mockReferenceDataRepository;
        private string _invoiceType = "RPA";

        private readonly ReferenceDataApi _referenceDataApi;
        public ReferenceDataApiOrganisationTests()
        {
            _mockReferenceDataRepository = new Mock<IReferenceDataRepository>();
            _referenceDataApi = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());
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
                        Code = "ORG1",
                        Description =  "First Organisation"
                    },
                    new Organisation()
                    {
                        Code = "ORG2",
                        Description =  "Second Organisation"
                    }
                    }))
                });

            //Act
            var response = _referenceDataApi.GetOrganisationsAsync(_invoiceType).Result;

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.IsSuccess.Should().BeTrue();
            response.Data.Should().BeOfType<List<Organisation>>();
            response.Data.Should().HaveCount(2);
            response.Data.Should().BeEquivalentTo(new List<Organisation>()
            {
                    new Organisation()
                    {
                        Code = "ORG1",
                        Description =  "First Organisation"
                    },
                    new Organisation()
                    {
                        Code = "ORG2",
                        Description =  "Second Organisation"
                    }
             });
        }

        [Fact]
        public void GetOrganisationsAsync_API_Returns_NoContent()
        {
            _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var response = _referenceDataApi.GetOrganisationsAsync(_invoiceType).Result;

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

            var response = _referenceDataApi.GetOrganisationsAsync(_invoiceType).Result;

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

            var response = _referenceDataApi.GetOrganisationsAsync(_invoiceType).Result;

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.IsSuccess.Should().BeFalse();
            response.Data.Should().BeNull();
        }

        [Fact]
        public void GetOrganisationAsync_API_Returns_BadRequest()
        {
            _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)); 

            var response = _referenceDataApi.GetOrganisationsAsync(_invoiceType).Result;

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
 
            var response = _referenceDataApi.GetOrganisationsAsync(_invoiceType).Result;

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            response.IsSuccess.Should().BeFalse();
            response.Data.Should().BeNull();
            response.Errors.Should().ContainKey($"{HttpStatusCode.InternalServerError}");
        }
    }
}
