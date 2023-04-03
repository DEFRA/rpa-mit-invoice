using FluentValidation;

namespace Invoices.Api.Models;

public class BulkInvoiceValidator : AbstractValidator<BulkInvoices>
{
    public BulkInvoiceValidator()
    {
        RuleFor(x => x.Reference).NotEmpty();
        RuleFor(x => x.SchemeType).NotEmpty();
        RuleFor(x => x.Invoices).NotEmpty();
    }
}