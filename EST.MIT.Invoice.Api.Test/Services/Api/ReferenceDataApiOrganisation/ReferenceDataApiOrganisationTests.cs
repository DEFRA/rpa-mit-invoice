using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Services.Api;
using EST.MIT.Invoice.Api.Services.Api.Models;
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
            _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new List<Invoices.Api.Models.Invoice>()
                    {
                        new Invoices.Api.Models.Invoice()
                        {
                            Id = "123456789",
                            InvoiceType = "AP",
                            AccountType = "AP",
                            Organisation = "Test Org",
                            Reference = "123456789",
                            SchemeType = "bps",
                            CreatedBy = "Test User",
                            Status = "status",
                            PaymentRequests = new List<InvoiceHeader> {
                                new InvoiceHeader {
                                    PaymentRequestId = "123456789",
                                    SourceSystem = "Manual",
                                    MarketingYear = 2023,
                                    DeliveryBody = "Test Org",
                                    PaymentRequestNumber = 123456789,
                                    AgreementNumber = "123456789",
                                    ContractNumber = "123456789",
                                    Value = 100,
                                    DueDate = "2023-01-01",
                                    AppendixReferences = new AppendixReferences {
                                        ClaimReferenceNumber = "123456789"
                                    },
                                    InvoiceLines = new List<InvoiceLine> {
                                        new InvoiceLine {
                                            Currency = "GBP",
                                            Description = "Test Description",
                                            Value = 100,
                                            SchemeCode = "123456789",
                                            FundCode = "123456789"
                                        }
                                    }
                                }

                            }

                        }
                    }))
                });

            //var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());
            //Act
            var response = _referenceDataApi.GetOrganisationsAsync().Result;

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.IsSuccess.Should().BeTrue();
            response.Data.Should().BeOfType<List<Invoices.Api.Models.Invoice>>();
            response.Data.Should().HaveCount(1);
            response.Data.Should().BeEquivalentTo(new List<Invoices.Api.Models.Invoice>()
            {
                new Invoices.Api.Models.Invoice()
                {
                    Id = "123456789",
                    InvoiceType = "AP",
                    AccountType = "AP",
                    Organisation = "Test Org",
                    Reference = "123456789",
                    SchemeType = "bps",
                    CreatedBy = "Test User",
                    Status = "status",
                    PaymentRequests = new List<InvoiceHeader> {
                        new InvoiceHeader {
                            PaymentRequestId = "123456789",
                            SourceSystem = "Manual",
                            MarketingYear = 2023,
                            DeliveryBody = "Test Org",
                            PaymentRequestNumber = 123456789,
                            AgreementNumber = "123456789",
                            ContractNumber = "123456789",
                            Value = 100,
                            DueDate = "2023-01-01",
                            AppendixReferences = new AppendixReferences {
                                ClaimReferenceNumber = "123456789"
                            },
                            InvoiceLines = new List<InvoiceLine> {
                                new InvoiceLine {
                                    Currency = "GBP",
                                    Description = "Test Description",
                                    Value = 100,
                                    SchemeCode = "123456789",
                                    FundCode = "123456789"
                                }
                            }
                        }

                    }

                }
             });
        }

        [Fact]
        public void GetOrganisationAsync_Returns_Invalid_Organisation()
        {
           _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new List<Invoices.Api.Models.Invoice>()
                    {
                        new Invoices.Api.Models.Invoice()
                        {
                            Id = "123456789",
                            InvoiceType = "AP",
                            AccountType = "AP",
                           // Organisation = "Test Org",
                            Reference = "123456789",
                            SchemeType = "bps",
                            CreatedBy = "Test User",
                            Status = "status",
                            PaymentRequests = new List<InvoiceHeader> {
                                new InvoiceHeader {
                                    PaymentRequestId = "123456789",
                                    SourceSystem = "Manual",
                                    MarketingYear = 2023,
                                    DeliveryBody = "Test Org",
                                    PaymentRequestNumber = 123456789,
                                    AgreementNumber = "123456789",
                                    ContractNumber = "123456789",
                                    Value = 100,
                                    DueDate = "2023-01-01",
                                    AppendixReferences = new AppendixReferences {
                                        ClaimReferenceNumber = "123456789"
                                    },
                                    InvoiceLines = new List<InvoiceLine> {
                                        new InvoiceLine {
                                            Currency = "GBP",
                                            Description = "Test Description",
                                            Value = 100,
                                            SchemeCode = "123456789",
                                            FundCode = "123456789"
                                        }
                                    }
                                }

                            }

                        }
                    }))
                });

            //Act
            var response = _referenceDataApi.GetOrganisationsAsync().Result;

            //Assert

        }



        [Fact]
        public void GetOrganisationsAsync_API_Returns_NoContent()
        {
            _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());

            var response = _referenceDataApi.GetOrganisationsAsync().Result;

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            response.IsSuccess.Should().BeFalse();
            response.Data.Should().BeNull();
        }

        [Fact]
        public void GetOrganisationsAsync_Deserialise_Fail()
        {
            _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("123")
            });

            //var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());

            var response = _referenceDataApi.GetOrganisationsAsync().Result;

            response.IsSuccess.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            response.Data.Should().BeEmpty();
            response.Errors.Should().ContainKey("deserializing");
        }

        [Fact]
        public void GetOrganisationAsync_API_Returns_NotFound()
        {
            _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

            // var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());

            var response = _referenceDataApi.GetOrganisationsAsync().Result;

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.IsSuccess.Should().BeFalse();
            response.Data.Should().BeNull();
        }

        [Fact]
        public void GetOrganisationAsync_API_Returns_BadRequest()
        {
            _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

            // var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());

            var response = _referenceDataApi.GetOrganisationsAsync().Result;

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.IsSuccess.Should().BeFalse();
            response.Data.Should().BeNull();
            response.Errors.Should().ContainKey($"{HttpStatusCode.BadRequest}");
        }

        [Fact]
        public void GetOrganisationAsync_API_Returns_Unexpected()
        {
            _mockReferenceDataRepository.Setup(x => x.GetOrganisationsListAsync())
            .ReturnsAsync(new HttpResponseMessage((HttpStatusCode)418));

            // var service = new ReferenceDataApi(_mockReferenceDataRepository.Object, Mock.Of<ILogger<ReferenceDataApi>>());

            var response = _referenceDataApi.GetOrganisationsAsync().Result;

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            response.IsSuccess.Should().BeFalse();
            response.Data.Should().BeNull();
            response.Errors.Should().ContainKey($"{HttpStatusCode.InternalServerError}");
        }
    }
}
