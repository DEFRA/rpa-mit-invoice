using FluentValidation;

namespace Invoices.Api.Models;

public class InvoiceLineValidator : AbstractValidator<InvoiceLine>
{
    public InvoiceLineValidator()
    {
        RuleFor(x => x.SchemeCode).NotEmpty();
        RuleFor(x => x.Value).NotEmpty();
        RuleFor(x => x.Currency).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.FundCode).NotEmpty();
    }
}

