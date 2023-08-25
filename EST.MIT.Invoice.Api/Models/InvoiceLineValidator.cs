using System.Globalization;
using System.Text.RegularExpressions;
using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.API.Interfaces;
using FluentValidation;
using Invoices.Api.Util;

namespace Invoices.Api.Models;

public class InvoiceLineValidator : AbstractValidator<InvoiceLine>
{
    private readonly string[] _validCurrencyTypes = { "GBP", "EUR" };
    private readonly IReferenceDataApi _referenceDataApi;
    private readonly FieldsRoute _route;
    public InvoiceLineValidator(IReferenceDataApi referenceDataApi, FieldsRoute route)
    {
        _route = route;
        _referenceDataApi = referenceDataApi;

        RuleFor(x => x.SchemeCode).NotEmpty();
        RuleFor(model => model)
            .MustAsync((x, cancellation) => BeAValidSchemeCodes(x))
            .WithMessage("SchemeCode is invalid")
            .When(model => !string.IsNullOrWhiteSpace(model.SchemeCode));
        RuleFor(x => x.Value)
            .NotEqual(0)
            .WithMessage("Invoice line value must be non-zero")
            .Must(HaveNoMoreThanTwoDecimalPlaces)
            .WithMessage("Invoice line value cannot be more than 2dp");
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.FundCode).NotEmpty();
        RuleFor(model => model)
           .MustAsync((x, cancellation) => BeAValidFundCodes(x))
           .WithMessage("Fund Code is invalid for this route")
           .When(model => !string.IsNullOrWhiteSpace(model.FundCode));
        RuleFor(x => x.Currency)
            .NotEmpty()
            .Must(x => this._validCurrencyTypes.Contains(x.ToUpper()))
            .WithMessage("Currency must be GBP or EUR");
    }

    private bool HaveNoMoreThanTwoDecimalPlaces(decimal value)
    {
        return Regex.IsMatch(value.ToString(CultureInfo.InvariantCulture), RegexConstants.TwoDecimalPlaces);
    }

    private async Task<bool> BeAValidSchemeCodes(InvoiceLine invoice)
    {
        if (string.IsNullOrWhiteSpace(invoice.SchemeCode))
        {
            return false;
        }

        var schemeCodes = await _referenceDataApi.GetSchemeCodesAsync(_route.InvoiceType, _route.Organisation, _route.PaymentType, _route.SchemeType);

        if (!schemeCodes.IsSuccess || !schemeCodes.Data.Any())
        {
            return false;
        }

        return schemeCodes.Data.Any(x => x.Code.ToLower() == invoice.SchemeCode.ToLower());
    }

    private async Task<bool> BeAValidFundCodes(InvoiceLine invoice)
    {
        if (string.IsNullOrWhiteSpace(invoice.FundCode))
        {
            return false;
        }
        var fundCodes = await _referenceDataApi.GetFundCodesAsync(_route.InvoiceType, _route.Organisation, _route.PaymentType, _route.SchemeType);

        if (!fundCodes.IsSuccess || !fundCodes.Data.Any())
        {
            return false;
        }

        return fundCodes.Data.Any(x => x.Code.ToLower() == invoice.FundCode.ToLower());
    }
}

