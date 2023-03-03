namespace Invoices.Api.Models;

public class InvoiceHeader
{
    public string Id { get; init; } = default!;
    public string ClaimReference { get; init; } = default!;
    public string ClaimReferenceNumber { get; init; } = default!;
    public string FRN { get; init; } = default!;
    public string AgreementNumber { get; init; } = default!;
    public string Currency { get; init; } = default!;
    public string Description { get; init; } = default!;
}