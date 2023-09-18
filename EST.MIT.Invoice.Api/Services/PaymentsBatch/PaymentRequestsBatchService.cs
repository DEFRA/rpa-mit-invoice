using Invoices.Api.Models;
using Invoices.Api.Repositories.Entities;
using Invoices.Api.Repositories.Interfaces;
using Invoices.Api.Util;

namespace Invoices.Api.Services.PaymentsBatch;

public class PaymentRequestsBatchService : IPaymentRequestsBatchService
{
    private readonly IPaymentRequestsBatchRepository _paymentRequestsBatchRepository;

    public PaymentRequestsBatchService(IPaymentRequestsBatchRepository paymentRequestsBatchRepository)
    {
        _paymentRequestsBatchRepository = paymentRequestsBatchRepository;
    }

    public async Task<List<PaymentRequestsBatch>> GetBySchemeAndIdAsync(string scheme, string id)
    {
        var result = await _paymentRequestsBatchRepository.GetBySchemeAndIdAsync(scheme, id);
        return InvoiceMapper.MapToInvoice(result);
    }

    public async Task<PaymentRequestsBatch> CreateAsync(PaymentRequestsBatch invoice)
    {
        var invoiceEntity = InvoiceMapper.MapToInvoiceEntity(invoice);
        await _paymentRequestsBatchRepository.CreateAsync(invoiceEntity);
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

        await _paymentRequestsBatchRepository.CreateBulkAsync(entity);

        return invoices;
    }

    public async Task<PaymentRequestsBatch> UpdateAsync(PaymentRequestsBatch invoice)
    {
        var invoiceEntity = InvoiceMapper.MapToInvoiceEntity(invoice);
        await _paymentRequestsBatchRepository.UpdateAsync(invoiceEntity);
        return invoice;
    }

    public async Task<string> DeleteBySchemeAndIdAsync(string schemeType, string id)
    {
        await _paymentRequestsBatchRepository.DeleteBySchemeAndIdAsync(schemeType, id);
        return id;
    }
}