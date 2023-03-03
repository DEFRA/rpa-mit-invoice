namespace Invoices.Api.Models;

public class InvoiceLine
{
    public string Description { get; init; } = default!;
    public Decimal Value { get; init; } = default!;
}
