using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Repositories.Entities;
using Newtonsoft.Json;

namespace EST.MIT.Invoice.Api.Util;

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
            Reference = paymentRequestsBatch.Reference,
            ApproverId = paymentRequestsBatch.ApproverId,
            ApproverEmail = paymentRequestsBatch.ApproverEmail,
            ApprovedBy = paymentRequestsBatch.ApprovedBy,
            Approved = paymentRequestsBatch.Approved,
            CreatedBy = paymentRequestsBatch.CreatedBy,
            UpdatedBy = paymentRequestsBatch.UpdatedBy,
            Created = paymentRequestsBatch.Created,
            Updated = paymentRequestsBatch.Updated
        };
    }

    public static List<PaymentRequestsBatch> MapToInvoice(IEnumerable<InvoiceEntity> invoiceEntities)
    {
        var invoices = new List<PaymentRequestsBatch>();

        foreach (var invoiceData in invoiceEntities.Select(x => x.Data))
        {
            var invoice = JsonConvert.DeserializeObject<PaymentRequestsBatch>(invoiceData);
            invoices.Add(invoice!);
        }

        return invoices;
    }

    public static PaymentRequestsBatch? MapToPaymentRequestsBatch(InvoiceEntity invoiceEntity)
    {
        if (invoiceEntity?.Data == null)
        {
            return null;
        }

        return JsonConvert.DeserializeObject<PaymentRequestsBatch>(invoiceEntity.Data);
    }

    public static List<InvoiceEntity> BulkMapToInvoiceEntity(IEnumerable<PaymentRequestsBatch> batches)
    {
        var invoiceEntities = new List<InvoiceEntity>();

        foreach (var batch in batches)
        {
            var invoiceEntity = MapToInvoiceEntity(batch);
            invoiceEntities.Add(invoiceEntity);
        }

        return invoiceEntities;
    }
}