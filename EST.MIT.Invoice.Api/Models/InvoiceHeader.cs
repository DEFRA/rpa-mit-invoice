namespace Invoices.Api.Models;

public class InvoiceHeader
{
    public string PaymentRequestId { get; init; } = default!;
    public int FRN { get; init; }
    public string SourceSystem { get; init; } = default!;
    public int MarketingYear { get; init; }
    public string Ledger { get; init; } = default!;
    public string DeliveryBody { get; init; } = default!;
    public int PaymentRequestNumber { get; init; }
    public string AgreementNumber { get; init; } = default!;
    public string ContractNumber { get; init; } = default!;
    public int Value { get; init; }
    public string DueDate { get; init; } = default!;
    public List<InvoiceLine> InvoiceLines { get; init; } = default!;
    public AppendixReferences AppendixReferences { get; init; } = default!;
}