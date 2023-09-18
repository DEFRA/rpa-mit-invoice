using Invoices.Api.Models;
using Invoices.Api.Repositories.Entities;
using Invoices.Api.Repositories.Interfaces;
using Invoices.Api.Util;

namespace Invoices.Api.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceService(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<List<Invoice>> GetBySchemeAndIdAsync(string scheme, string id)
    {
        var result = await _invoiceRepository.GetBySchemeAndIdAsync(scheme, id);
        return InvoiceMapper.MapToInvoice(result);
    }

    public async Task<Invoice> CreateAsync(Invoice invoice)
    {
        var invoiceEntity = InvoiceMapper.MapToInvoiceEntity(invoice);
        await _invoiceRepository.CreateAsync(invoiceEntity);
        return invoice;
    }

    public async Task<BulkInvoices?> CreateBulkAsync(BulkInvoices invoices)
    {
        var entity = new BulkInvoicesEntity
        {
            SchemeType = invoices.SchemeType,
            Reference = invoices.Reference,
            Invoices = InvoiceMapper.BulkMapToInvoiceEntity(invoices.Invoices)
        };

        await _invoiceRepository.CreateBulkAsync(entity);

        return invoices;
    }

    public async Task<Invoice> UpdateAsync(Invoice invoice)
    {
        var invoiceEntity = InvoiceMapper.MapToInvoiceEntity(invoice);
        await _invoiceRepository.UpdateAsync(invoiceEntity);
        return invoice;
    }

    public async Task<string> DeleteBySchemeAndIdAsync(string schemeType, string id)
    {
        await _invoiceRepository.DeleteBySchemeAndIdAsync(schemeType, id);
        return id;
    }
}