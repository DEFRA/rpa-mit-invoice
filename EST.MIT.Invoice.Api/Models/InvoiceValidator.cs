using FluentValidation;

namespace Invoices.Api.Models;

public class InvoiceValidator : AbstractValidator<Invoice>
{
    public InvoiceValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Scheme).NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
    }
}