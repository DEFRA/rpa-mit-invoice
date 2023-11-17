namespace EST.MIT.Invoice.Api.Repositories.Entities
{
    public class BulkInvoicesEntity
    {
        public IEnumerable<InvoiceEntity> Invoices { get; set; } = default!;
        public string Reference { get; set; } = default!;
        public string SchemeType { get; set; } = default!;
    }
}

