using Invoices.Api.Models;
using Invoices.Api.Repositories.Entities;
using Invoices.Api.Services.Models;
using Newtonsoft.Json;

namespace Invoices.Api.Util;

public static class InvoiceMapper
{
    public static InvoiceEntity MapToInvoiceEntity(Invoice invoice)
    {
        return new InvoiceEntity()
        {
            SchemeType = invoice.SchemeType,
            Id = invoice.Id,
            Data = JsonConvert.SerializeObject(invoice),
            Value = invoice.PaymentRequests.Sum(x => x.Value),
            Status = invoice.Status,
            Reference = invoice.Reference,
            CreatedBy = invoice.CreatedBy,
            UpdatedBy = invoice.UpdatedBy,
            Created = invoice.Created,
            Updated = invoice.Updated
        };
    }

    public static List<Invoice> MapToInvoice(IEnumerable<InvoiceEntity> invoiceEntites)
    {
        var invoices = new List<Invoice>();

        foreach (var invoiceData in invoiceEntites.Select(x => x.Data))
        {
            var invoice = JsonConvert.DeserializeObject<Invoice>(invoiceData);
            invoices.Add(invoice!);
        }

        return invoices;
    }

    public static List<InvoiceEntity> BulkMapToInvoiceEntity(IEnumerable<Invoice> invoices)
    {
        var invoiceEntities = new List<InvoiceEntity>();

        foreach (var invoice in invoices)
        {
            var invoiceEntity = MapToInvoiceEntity(invoice);
            invoiceEntities.Add(invoiceEntity);
        }

        return invoiceEntities;
    }
}