namespace EST.MIT.Invoice.Api.Models;

public static class InvoiceStatuses
{
    public static readonly string New = "new";
    public static readonly string AwaitingApproval = "AWAITING_APPROVAL";
    public static readonly string Approved = "APPROVED";
    public static readonly string Rejected = "REJECTED";
}