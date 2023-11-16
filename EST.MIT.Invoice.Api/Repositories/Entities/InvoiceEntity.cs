namespace EST.MIT.Invoice.Api.Repositories.Entities;

public class InvoiceEntity
{
    public string SchemeType { get; set; } = default!;
    public string Id { get; set; } = default!;
    public string Data { get; set; } = default!;
    public string Reference { get; set; } = default!;
    public decimal Value { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string ApproverId { get; set; } = default!;
    public string ApproverEmail { get; set; } = default!;
    public string ApprovedBy { get; set; } = default!;
    public DateTime? Approved { get; set; } = default!;
    public string CreatedBy { get; set; } = default!;
    public string UpdatedBy { get; set; } = default!;
    public DateTime Created { get; set; } = default!;
    public DateTime? Updated { get; set; } = default!;
}