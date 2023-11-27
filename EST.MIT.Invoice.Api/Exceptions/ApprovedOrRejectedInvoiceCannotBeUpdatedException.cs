namespace EST.MIT.Invoice.Api.Exceptions;

public class ApprovedOrRejectedInvoiceCannotBeUpdatedException : Exception
{
    public ApprovedOrRejectedInvoiceCannotBeUpdatedException()
        : base("Approved or rejected invoice cannot be updated")
    {
    }
}
