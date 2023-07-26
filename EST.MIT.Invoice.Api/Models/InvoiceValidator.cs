using FluentValidation;

namespace Invoices.Api.Models;

public class InvoiceValidator : AbstractValidator<Invoice>
{
    private readonly string[] _validAccountTypes = { "AP", "AR" };

    public InvoiceValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.SchemeType).NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
        RuleFor(x => x.InvoiceType).NotNull();
        RuleFor(x => x.AccountType)
            .NotEmpty()
            .Must(x => this._validAccountTypes.Contains(x.ToUpper()))
            .WithMessage("Account Type is invalid. Should be AP or AR");
        RuleFor(x => x.PaymentRequests).NotEmpty();
        RuleForEach(x => x.PaymentRequests).SetValidator(new InvoiceHeaderValidator()).When(x => x.PaymentRequests != null);
    }
}