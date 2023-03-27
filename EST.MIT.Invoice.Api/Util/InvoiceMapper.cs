using Invoices.Api.Models;
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
            Value = invoice.Headers.Sum(x => x.Value),
            Status = invoice.Status,
            CreatedBy = invoice.CreatedBy,
            UpdatedBy = invoice.UpdatedBy,
            Created = invoice.Created.ToLongDateString(),
            Updated = invoice.Updated?.ToLongDateString()
        };
    }

    public static List<Invoice> MapToInvoice(List<InvoiceEntity> invoiceEntites)
    {
        var invoices = new List<Invoice>();

        foreach (var invoiceEntity in invoiceEntites)
        {
            var invoice = JsonConvert.DeserializeObject<Invoice>(invoiceEntity.Data);
            invoices.Add(invoice!);
        }

        return invoices;
    }
}