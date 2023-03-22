namespace Invoices.Api.Models;

public class InvoiceLine
{
    public int Value { get; set; }
    public string Currency { get; set; } = null!;
    public string SchemeCode { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string FundCode { get; set; } = null!;
}