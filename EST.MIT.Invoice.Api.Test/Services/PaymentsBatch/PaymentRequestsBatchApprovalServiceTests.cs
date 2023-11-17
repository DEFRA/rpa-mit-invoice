using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Repositories.Entities;
using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Services.PaymentsBatch;
using Moq;
using Newtonsoft.Json;

namespace EST.MIT.Invoice.Api.Test.Services.PaymentsBatch;

public class PaymentRequestsBatchApprovalServiceTests
{
    private readonly PaymentRequestsBatch invoiceTestData = PaymentRequestsBatchTestData.CreateInvoice();

    [Fact]
    public async Task GetInvoicesByUserIdAsync_WithValidUserId_ReturnsInvoices()
    {
        var mockRepository = new Mock<IPaymentRequestsBatchRepository>();
        var service = new PaymentRequestsBatchApprovalService(mockRepository.Object);

        var userId = "validUserId";
        var data = JsonConvert.SerializeObject(invoiceTestData);
        var expectedInvoices = new List<InvoiceEntity>
        {
            new InvoiceEntity { Id = "1", SchemeType = "Scheme1", Value = 100, Status = "awaiting", Data = data },
            new InvoiceEntity { Id = "2", SchemeType = "Scheme1", Value = 200, Status = "awaiting", Data = data },
        };
        mockRepository.Setup(repo => repo.GetInvoicesByUserIdAsync(userId))
                      .ReturnsAsync(expectedInvoices);

        var result = await service.GetInvoicesByUserIdAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(expectedInvoices.Count, result.Count);
    }
}
