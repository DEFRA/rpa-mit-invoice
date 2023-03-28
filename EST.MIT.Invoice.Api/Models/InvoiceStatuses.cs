namespace Invoices.Api.Models;

public static class InvoiceStatuses
{
    public static readonly string New = "new";
    public static readonly string Awaiting = "awaiting";
    public static readonly string Approved = "approved";
    public static readonly string Rejected = "rejected";
}