using Invoices.Api.Models;
using Invoices.Api.Services.Models;
using Newtonsoft.Json;

namespace Invoices.Api.Util;

public static class InvoiceMapper
{
    public static InvoiceEntity MapToInvoiceEntity(PaymentRequestsBatch paymentRequestsBatch)
    {
        return new InvoiceEntity()
        {
            SchemeType = paymentRequestsBatch.SchemeType,
            Id = paymentRequestsBatch.Id,
            Data = JsonConvert.SerializeObject(paymentRequestsBatch),
            Value = paymentRequestsBatch.PaymentRequests.Sum(x => x.Value),
            Status = paymentRequestsBatch.Status,
            CreatedBy = paymentRequestsBatch.CreatedBy,
            UpdatedBy = paymentRequestsBatch.UpdatedBy,
            Created = paymentRequestsBatch.Created.ToLongDateString(),
            Updated = paymentRequestsBatch.Updated?.ToLongDateString() ?? string.Empty
        };
    }

    public static List<PaymentRequestsBatch> MapToInvoice(List<InvoiceEntity> invoiceEntities)
    {
        var invoices = new List<PaymentRequestsBatch>();

        foreach (var invoiceData in invoiceEntities.Select(x => x.Data))
        {
            var invoice = JsonConvert.DeserializeObject<PaymentRequestsBatch>(invoiceData);
            invoices.Add(invoice!);
        }

        return invoices;
    }
}