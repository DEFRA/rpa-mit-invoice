using FluentValidation;

namespace Invoices.Api.Models;

public class InvoiceValidator : AbstractValidator<Invoice>
{
    public InvoiceValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.SchemeType).NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
        RuleFor(x => x.InvoiceType).NotNull();
        RuleFor(x => x.PaymentRequests).NotEmpty();
        RuleForEach(x => x.PaymentRequests).SetValidator(new InvoiceHeaderValidator()).When(x => x.PaymentRequests != null);
    }
}