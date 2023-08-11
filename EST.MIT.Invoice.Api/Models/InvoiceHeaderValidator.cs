using System.Globalization;
using System.Text.RegularExpressions;
using FluentValidation;
using Invoices.Api.Util;

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
        RuleFor(x => x.Value)
            .NotEqual(0)
            .WithMessage("Invoice value must be non-zero")
            .Must(HaveNoMoreThanTwoDecimalPlaces)
            .WithMessage("Invoice value cannot be more than 2dp");
        RuleForEach(x => x.InvoiceLines).SetValidator(new InvoiceLineValidator());

        RuleFor(model => model)
            .Must(HaveSameCurrencyTypes)
            .WithMessage("Cannot mix currencies in an invoice")
            .When(model => model.InvoiceLines != null && model.InvoiceLines.Any());
    }

    private bool HaveSameCurrencyTypes(InvoiceHeader invoiceHeader)
    {
        // get all the currency types from the invoice lines
        var allCurrencyTypes = invoiceHeader.InvoiceLines.Select(x => x.Currency).Distinct();

        // check that all invoice lines have the same currency
        return allCurrencyTypes.Count() <= 1;
    }

    private bool HaveNoMoreThanTwoDecimalPlaces(decimal value)
    {
        return Regex.IsMatch(value.ToString(CultureInfo.InvariantCulture), RegexConstants.TwoDecimalPlaces);
    }
}


