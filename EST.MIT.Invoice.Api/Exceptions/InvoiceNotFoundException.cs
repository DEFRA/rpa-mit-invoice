namespace EST.MIT.Invoice.Api.Exceptions;

public class InvoiceNotFoundException : Exception
{
    public InvoiceNotFoundException()
        : base("The requested invoice could not be found")
    {
    }
}
