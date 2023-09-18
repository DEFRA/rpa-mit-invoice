using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Invoices.Api.Models;
using Invoices.Api.Repositories;
using Invoices.Api.Repositories.Entities;
using Invoices.Api.Repositories.Interfaces;
using Invoices.Api.Services;
using Invoices.Api.Services.Models;
using Invoices.Api.Util;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace Invoices.Api.Test;

public class InvoiceServiceTests
{
    private readonly Mock<IInvoiceRepository> _mockInvoiceRepository;
    private readonly InvoiceService _invoiceService;
    private readonly Invoice invoiceTestData = InvoiceTestData.CreateInvoice();

    public InvoiceServiceTests()
    {
        _mockInvoiceRepository = new Mock<IInvoiceRepository>();
        _invoiceService = new InvoiceService(_mockInvoiceRepository.Object);
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

        _mockInvoiceRepository.Setup(x => x.GetBySchemeAndIdAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(invoiceEntities);

        var result = await _invoiceService.GetBySchemeAndIdAsync("1","1");
        result.Should().BeEquivalentTo(InvoiceMapper.MapToInvoice(invoiceEntities));
    }

    [Fact]
    public async Task Create_AddsInvoice()
    {
        var invoice = invoiceTestData;
        var invoiceEntity = InvoiceMapper.MapToInvoiceEntity(invoice);

        _mockInvoiceRepository.Setup(x => x.CreateAsync(It.IsAny<InvoiceEntity>())).ReturnsAsync(invoiceEntity);

        var result = await _invoiceService.CreateAsync(invoice);

        result.Should().BeEquivalentTo(invoice);
    }

    [Fact]
    public async Task Update_UpdatesInvoice()
    {
        var invoice = invoiceTestData;
        var invoiceEntity = InvoiceMapper.MapToInvoiceEntity(invoice);

        _mockInvoiceRepository.Setup(x => x.UpdateAsync(It.IsAny<InvoiceEntity>())).ReturnsAsync(invoiceEntity);

        var result = await _invoiceService.UpdateAsync(invoice);

        result.Should().BeEquivalentTo(invoice);
    }

    [Fact]
    public async Task Delete_DeletesInvoice()
    {
        var invoice = invoiceTestData;
        var invoiceEntity = InvoiceMapper.MapToInvoiceEntity(invoice);

        _mockInvoiceRepository.Setup(x => x.DeleteBySchemeAndIdAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(invoice.Id);

        var result = await _invoiceService.DeleteBySchemeAndIdAsync(invoice.SchemeType, invoice.Id);

        result.Should().BeEquivalentTo(invoice.Id);
    }
}
