using System.Globalization;
using System.Text.RegularExpressions;
using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using FluentValidation;
using Invoices.Api.Util;

namespace Invoices.Api.Models;

public class InvoiceLineValidator : AbstractValidator<InvoiceLine>
{
    private readonly string[] _validCurrencyTypes = { "GBP", "EUR" };
    private readonly IReferenceDataApi _referenceDataApi;
    private readonly FieldsRoute _route;
    private readonly ICachedReferenceDataApi _cachedReferenceDataApi;
    public InvoiceLineValidator(IReferenceDataApi referenceDataApi, FieldsRoute route, ICachedReferenceDataApi cachedReferenceDataApi)
    {
        _route = route;
        this._cachedReferenceDataApi = cachedReferenceDataApi;
        _referenceDataApi = referenceDataApi;

        RuleFor(x => x.MarketingYear).NotEmpty();
        RuleFor(x => x.SchemeCode).NotEmpty();
        RuleFor(x => x.SchemeCode)
            .MustAsync((x, cancellation) => BeAValidSchemeCodes(x))
            .WithMessage("SchemeCode is invalid")
            .When(model => !string.IsNullOrWhiteSpace(model.SchemeCode));
        RuleFor(x => x.Value)
            .NotEqual(0)
            .WithMessage("PaymentRequestsBatch line value must be non-zero")
            .Must(HaveNoMoreThanTwoDecimalPlaces)
            .WithMessage("PaymentRequestsBatch line value cannot be more than 2dp");
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.FundCode).NotEmpty()
           .MustAsync((x, cancellation) => BeAValidFundCode(x))
           .WithMessage("Fund Code is invalid for this route")
           .When(model => !string.IsNullOrWhiteSpace(model.FundCode));
        RuleFor(x => x.Currency)
            .NotEmpty()
            .Must(x => this._validCurrencyTypes.Contains(x.ToUpper()))
            .WithMessage("Currency must be GBP or EUR");
        RuleFor(x => x.MainAccount).NotEmpty()
            .MustAsync((x, cancellation) => BeAValidMainAccount(x))
            .WithMessage("Account is Invalid for this route")
            .When(model => !string.IsNullOrWhiteSpace(model.MainAccount));
        RuleFor(x => x.DeliveryBody)
            .NotEmpty()
            .MustAsync((deliveryBody, cancellationToken) => BeAValidDeliveryBodyAsync(deliveryBody))
            .WithMessage("Delivery Body is invalid for this route");
        RuleFor(model => model)
            .MustAsync((model, cancellationToken) => BeAllowedCombination(model))
            .WithMessage("Account / Scheme / Delivery Body combination is invalid")
            .When(x => !string.IsNullOrWhiteSpace(x.DeliveryBody) && !string.IsNullOrWhiteSpace(x.SchemeCode) && !string.IsNullOrWhiteSpace(x.MainAccount));
    }

    private bool HaveNoMoreThanTwoDecimalPlaces(decimal value)
    {
        return Regex.IsMatch(value.ToString(CultureInfo.InvariantCulture), RegexConstants.TwoDecimalPlaces);
    }

    private async Task<bool> BeAValidSchemeCodes(string schemeCode)
    {
        var schemeCodes = await _referenceDataApi.GetSchemeCodesAsync(_route.InvoiceType, _route.Organisation, _route.PaymentType, _route.SchemeType);

        if (!schemeCodes.IsSuccess || !schemeCodes.Data.Any())
        {
            return false;
        }

        return schemeCodes.Data.Any(x => x.Code.ToLower() == schemeCode.ToLower());
    }

    private async Task<bool> BeAValidFundCode(string fundCode)
    {
        var fundCodes = await _referenceDataApi.GetFundCodesAsync(_route.InvoiceType, _route.Organisation, _route.PaymentType, _route.SchemeType);

        if (!fundCodes.IsSuccess || !fundCodes.Data.Any())
        {
            return false;
        }

        return fundCodes.Data.Any(x => x.Code.ToLower() == fundCode.ToLower());
    }

    private async Task<IEnumerable<CombinationForRoute>> GetCombinationsListForRouteAsync()
    {
        var invoiceType = _route.InvoiceType ?? "";
        var organisation = _route.Organisation ?? "";
        var paymentType = _route.PaymentType ?? "";
        var schemeType = _route.SchemeType ?? "";

        if (string.IsNullOrWhiteSpace(invoiceType) || string.IsNullOrWhiteSpace(organisation) ||
            string.IsNullOrWhiteSpace(paymentType) || string.IsNullOrWhiteSpace(schemeType))
        {
            return new List<CombinationForRoute>();
        }

        var combinationsForRoute = await _cachedReferenceDataApi.GetCombinationsListForRouteAsync(invoiceType, organisation, paymentType, schemeType);

        if (!combinationsForRoute.IsSuccess || !combinationsForRoute.Data.Any())
        {
            return new List<CombinationForRoute>();
        }

        return combinationsForRoute.Data;
    }

    private async Task<bool> BeAValidMainAccount(string mainAccount)
    {
        return (await GetCombinationsListForRouteAsync()).Any(x => x.AccountCode.ToLower() == mainAccount.ToLower());
    }

    private async Task<bool> BeAValidDeliveryBodyAsync(string deliveryBody)
    {
        return (await GetCombinationsListForRouteAsync()).Any(x => x.DeliveryBodyCode.ToLower() == deliveryBody.ToLower());
    }

    private async Task<bool> BeAllowedCombination(InvoiceLine invoiceLine)
    {
        return (await GetCombinationsListForRouteAsync()).Any(x =>
           string.Equals(x.DeliveryBodyCode, invoiceLine.DeliveryBody, StringComparison.OrdinalIgnoreCase) &&
           string.Equals(x.SchemeCode, invoiceLine.SchemeCode, StringComparison.OrdinalIgnoreCase) &&
           string.Equals(x.AccountCode, invoiceLine.MainAccount, StringComparison.OrdinalIgnoreCase)
       );
    }
}

