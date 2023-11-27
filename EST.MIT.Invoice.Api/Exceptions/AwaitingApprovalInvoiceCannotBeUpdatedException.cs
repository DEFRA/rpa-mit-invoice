namespace EST.MIT.Invoice.Api.Exceptions;

public class AwaitingApprovalInvoiceCannotBeUpdatedException : Exception
{
    public AwaitingApprovalInvoiceCannotBeUpdatedException()
        : base("Awaiting approval invoice cannot be updated")
    {
    }
}
