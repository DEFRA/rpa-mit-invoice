using FluentValidation;

namespace Invoices.Api.Models;

public class InvoiceHeaderValidator : AbstractValidator<InvoiceHeader>
{
    public InvoiceHeaderValidator()
    {
        RuleFor(x => x.AgreementNumber).NotEmpty();
        RuleFor(x => x.AppendixReferences).NotEmpty();
        RuleFor(x => x.FRN).NotEmpty();
        RuleFor(x => x.InvoiceLines).NotEmpty();
        RuleFor(x => x.SourceSystem).NotEmpty();
        RuleFor(x => x.ContractNumber).NotEmpty();
        RuleFor(x => x.DeliveryBody).NotEmpty();
        RuleFor(x => x.DueDate).NotEmpty();
        RuleFor(x => x.MarketingYear).NotEmpty();
        RuleFor(x => x.PaymentRequestId).NotEmpty();
        RuleFor(x => x.PaymentRequestNumber).NotEmpty();
        RuleFor(x => x.Value).NotEmpty();
        RuleForEach(x => x.InvoiceLines).SetValidator(new InvoiceLineValidator());
    }
}


