using FluentAssertions;
using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Repositories.Entities;
using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Services.Api;
using EST.MIT.Invoice.Api.Services.PaymentsBatch;
using EST.MIT.Invoice.Api.Util;
using Moq;
using Newtonsoft.Json;

namespace EST.MIT.Invoice.Api.Test;

public class InvoiceServiceTests
{
    private readonly Mock<IPaymentRequestsBatchRepository> _mockPaymentRequestsBatchRepository;
    private readonly PaymentRequestsBatchService _paymentRequestsBatchService;
    private readonly MockedDataService _mockedDataService;
    private readonly PaymentRequestsBatch invoiceTestData = PaymentRequestsBatchTestData.CreateInvoice(InvoiceStatuses.Approved);

    public InvoiceServiceTests()
    {
        _mockPaymentRequestsBatchRepository = new Mock<IPaymentRequestsBatchRepository>();
        _mockedDataService = new MockedDataService();
        _paymentRequestsBatchService = new PaymentRequestsBatchService(_mockPaymentRequestsBatchRepository.Object);
    }

    [Fact]
    public async Task Get_ReturnsInvoices()
    {
        var data = JsonConvert.SerializeObject(invoiceTestData);
        var invoiceEntities = new List<InvoiceEntity>
        {
            new InvoiceEntity { Id = "1", SchemeType = "Scheme1", Value = 100, Status = "awaiting", Data = data },
            new InvoiceEntity { Id = "2", SchemeType = "Scheme1", Value = 200, Status = "awaiting", Data = data },
        };

        _mockPaymentRequestsBatchRepository.Setup(x => x.GetBySchemeAndIdAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(invoiceEntities);

        var result = await _paymentRequestsBatchService.GetBySchemeAndIdAsync("1", "1");
        result.Should().BeEquivalentTo(InvoiceMapper.MapToInvoice(invoiceEntities));
    }

    [Fact]
    public async Task Create_AddsInvoice()
    {
        var invoice = invoiceTestData;
        var invoiceEntity = InvoiceMapper.MapToInvoiceEntity(invoice);
        var loggedInUser = _mockedDataService.GetLoggedInUser();

		_mockPaymentRequestsBatchRepository.Setup(x => x.CreateAsync(It.IsAny<InvoiceEntity>())).ReturnsAsync(invoiceEntity);

        var result = await _paymentRequestsBatchService.CreateAsync(invoice, loggedInUser);

        result.Should().BeEquivalentTo(invoice);
    }

    [Fact]
    public async Task Update_UpdatesInvoice()
    {
        var invoice = invoiceTestData;
        var invoiceEntity = InvoiceMapper.MapToInvoiceEntity(invoice);
        var loggedInUser = _mockedDataService.GetLoggedInUser();

		_mockPaymentRequestsBatchRepository.Setup(x => x.UpdateAsync(It.IsAny<InvoiceEntity>())).ReturnsAsync(invoiceEntity);

        var result = await _paymentRequestsBatchService.UpdateAsync(invoice, loggedInUser);

        result.Should().BeEquivalentTo(invoice);
    }

    [Fact]
    public async Task Delete_DeletesInvoice()
    {
        var invoice = invoiceTestData;
        var invoiceEntity = InvoiceMapper.MapToInvoiceEntity(invoice);

        _mockPaymentRequestsBatchRepository.Setup(x => x.DeleteBySchemeAndIdAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(invoice.Id);

        var result = await _paymentRequestsBatchService.DeleteBySchemeAndIdAsync(invoice.SchemeType, invoice.Id);

        result.Should().BeEquivalentTo(invoice.Id);
    }
}
