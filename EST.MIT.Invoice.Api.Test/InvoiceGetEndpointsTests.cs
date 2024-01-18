using EST.MIT.Invoice.Api.Endpoints;
using FluentAssertions;
using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Services.Api;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using EST.MIT.Invoice.Api.Services.PaymentsBatch;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EST.MIT.Invoice.Api.Test;

public class InvoiceGetEndpointTests
{
    private readonly IPaymentRequestsBatchService _paymentRequestsBatchService =
        Substitute.For<IPaymentRequestsBatchService>();

    private readonly IMockedDataService _mockedDataService = new MockedDataService();

    private readonly PaymentRequestsBatch _paymentRequestsBatchTestData = PaymentRequestsBatchTestData.CreateInvoice(InvoiceStatuses.Approved);

    [Fact]
    public async Task GetInvoicebyInvoiceId_WhenInvoiceExists()
    {
        const string invoiceId = "123456789";

        var invoice = _paymentRequestsBatchTestData;

        _paymentRequestsBatchService.GetByIdAsync(invoiceId).Returns(new List<PaymentRequestsBatch> { invoice });

        var result = await InvoiceGetEndpoints.GetInvoiceById(invoiceId, _paymentRequestsBatchService);

        result.GetOkObjectResultValue<PaymentRequestsBatch>().Should().BeEquivalentTo(invoice);
        result.GetOkObjectResultStatusCode().Should().Be(200);
    }

    [Fact]
    public async Task GetInvoicebyInvoiceId_WhenInvoiceDoesNotExists()
    {
        const string invoiceId = "123456789";

        _paymentRequestsBatchService.GetByIdAsync(invoiceId)
            .ReturnsNull();

        var result = await InvoiceGetEndpoints.GetInvoiceById(invoiceId, _paymentRequestsBatchService);

        result.GetNotFoundResultStatusCode().Should().Be(404);
    }

    [Fact]
    public async Task GetInvoicebyPaymentRequestId_WhenInvoiceExists()
    {
        const string paymentRequestId = "abcd_123456789";

        var invoice = _paymentRequestsBatchTestData;

        _paymentRequestsBatchService.GetByPaymentRequestIdAsync(paymentRequestId).Returns(new List<PaymentRequestsBatch> { invoice });

        var result = await InvoiceGetEndpoints.GetInvoiceByPaymentRequestId(paymentRequestId, _paymentRequestsBatchService);

        result.GetOkObjectResultValue<PaymentRequestsBatch>().Should().BeEquivalentTo(invoice);
        result.GetOkObjectResultStatusCode().Should().Be(200);
    }

    [Fact]
    public async Task GetInvoicebyPaymentRequestId_WhenInvoiceDoesNotExists()
    {
        const string paymentRequestId = "abcd_123456789";

        _paymentRequestsBatchService.GetByPaymentRequestIdAsync(paymentRequestId).ReturnsNull();

        var result = await InvoiceGetEndpoints.GetInvoiceByPaymentRequestId(paymentRequestId, _paymentRequestsBatchService);

        result.GetNotFoundResultStatusCode().Should().Be(404);
    }

    [Fact]
    public async Task GetInvoicebySchemeAndInvoiceId_WhenInvoiceExists()
    {
        const string scheme = "bps";
        const string invoiceId = "123456789";

        var invoice = _paymentRequestsBatchTestData;

        _paymentRequestsBatchService.GetBySchemeAndIdAsync(scheme, invoiceId).Returns(new List<PaymentRequestsBatch> { invoice });

        var result = await InvoiceGetEndpoints.GetInvoiceBySchemeAndId(scheme, invoiceId, _paymentRequestsBatchService);

        result.GetOkObjectResultValue<PaymentRequestsBatch>().Should().BeEquivalentTo(invoice);
        result.GetOkObjectResultStatusCode().Should().Be(200);
    }

    [Fact]
    public async Task GetInvoicebySchemeAndInvoiceId_WhenInvoiceDoesNotExists()
    {
        const string scheme = "bps";
        const string invoiceId = "123456789";

        _paymentRequestsBatchService.GetBySchemeAndIdAsync(scheme, invoiceId)
            .ReturnsNull();

        var result = await InvoiceGetEndpoints.GetInvoiceBySchemeAndId(scheme, invoiceId, _paymentRequestsBatchService);

        result.GetNotFoundResultStatusCode().Should().Be(404);
    }

    [Fact]
    public async Task GetInvoicesById_ReturnsOkResult_WhenInvoicesExist()
    {
        var mockService = new Mock<IPaymentRequestsBatchService>();
        var userId = "testuser@defra.gov.uk";
        var expectedInvoices = new List<PaymentRequestsBatch>
        {
            new PaymentRequestsBatch()
                {
                    Id = "1",
                    AccountType = "AD",
                    Created = DateTime.Now,
                    CreatedBy = "henry",
                    Organisation = "FGH",
                    PaymentRequests = new List<PaymentRequest>()
                    {
                        new PaymentRequest()
                        {
                            AgreementNumber = "12345",
                            DueDate = "string",
                            FRN = 56789043,
                            InvoiceLines = new List<InvoiceLine>()
                            {
                                new InvoiceLine()
                                {
                                    DeliveryBody = "DeliveryBody",
                                    MainAccount = "AccountA",
                                    MarketingYear = 2022,
                                    Description = "This is a description",
                                    FundCode = "2ADC",
                                    SchemeCode = "D4ERT",
                                    Value = 3
                                }
                            },
                            MarketingYear = 2023,
                            PaymentRequestId = "2",
                            PaymentRequestNumber = 34567,
                            SourceSystem = "sourceSystem",
                            Value = 2
                        }
                    }

                }
        };
        mockService.Setup(service => service.GetInvoicesByUserIdAsync(userId))
                   .ReturnsAsync(expectedInvoices);

		  var result = await InvoiceGetEndpoints.GetInvoicesById(GetMockHttpContext(), mockService.Object, _mockedDataService);

        result.GetOkObjectResultValue<List<PaymentRequestsBatch>>().Should().BeEquivalentTo(expectedInvoices);
        result.GetOkObjectResultStatusCode().Should().Be(200);
    }

    [Fact]
    public async Task GetInvoicesById_ReturnsNotFound_WhenNoInvoicesExist()
    {
        var mockService = new Mock<IPaymentRequestsBatchService>();
        var userId = "1";
        var nullInvoices = new Mock<List<PaymentRequestsBatch>>();

        mockService.Setup(service => service.GetInvoicesByUserIdAsync(userId))
                   .ReturnsAsync(nullInvoices.Object);

        var result = await InvoiceGetEndpoints.GetInvoicesById(GetMockHttpContext(), mockService.Object, _mockedDataService);

        result.GetNotFoundResultStatusCode().Should().Be(404);
    }

    private HttpContext GetMockHttpContext()
    {
        var mockContext = new Mock<HttpContext>();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.Email, "testuser@defra.gov.uk")
        }));

        mockContext.SetupGet(c => c.User).Returns(user);
        return mockContext.Object;
    }
}