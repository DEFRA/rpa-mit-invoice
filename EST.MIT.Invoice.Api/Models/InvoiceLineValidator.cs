using System.Globalization;
using System.Text.RegularExpressions;
using FluentValidation;
using Invoices.Api.Util;

namespace Invoices.Api.Models;

public class InvoiceLineValidator : AbstractValidator<InvoiceLine>
{
    private readonly string[] _validCurrencyTypes = { "GBP", "EUR" };

    public InvoiceLineValidator()
    {
        RuleFor(x => x.SchemeCode).NotEmpty();
        RuleFor(x => x.Value)
            .NotEqual(0)
            .WithMessage("Invoice line value must be non-zero")
            .Must(HaveNoMoreThanTwoDecimalPlaces)
            .WithMessage("Invoice line value cannot be more than 2dp");

        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.FundCode).NotEmpty();

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Must(x => this._validCurrencyTypes.Contains(x.ToUpper()))
            .WithMessage("Currency must be GBP or EUR");
    }

    private bool HaveNoMoreThanTwoDecimalPlaces(decimal value)
    {
        var valueAsString = value.ToString(CultureInfo.InvariantCulture);
        var twoDecimalPlacesRegex = new Regex(RegexConstants.TwoDecimalPlaces);

        return twoDecimalPlacesRegex.IsMatch(valueAsString);
    }
}

