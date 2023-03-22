namespace Invoices.Api.Models;

public class Invoice
{
    public string Id { get; init; } = default!;
    public string InvoiceType { get; init; } = default!;
    public string AccountType { get; init; } = default!;
    public string Organisation { get; init; } = default!;
    public string SchemeType { get; init; } = default!;
    public List<InvoiceHeader> Headers { get; init; } = default!;
    public string Status { get; init; } = default!;
}