using System.Globalization;
using System.Text.RegularExpressions;
using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using FluentValidation;
using EST.MIT.Invoice.Api.Util;
using Microsoft.AspNetCore.Components.Forms;

namespace EST.MIT.Invoice.Api.Models;

public class InvoiceLineValidator : AbstractValidator<InvoiceLine>
{
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
            .WithMessage("Invoice line value must be non-zero")
            .Must(HaveNoMoreThanTwoDecimalPlaces)
            .WithMessage("Invoice line value cannot be more than 2dp");
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.FundCode).NotEmpty()
           .MustAsync((x, cancellation) => BeAValidFundCode(x))
           .WithMessage("Fund Code is invalid for this route")
           .When(model => !string.IsNullOrWhiteSpace(model.FundCode));
        RuleFor(x => x.MainAccount).NotEmpty()
            .MustAsync((x, cancellation) => BeAValidMainAccountCode(x))
            .WithMessage("Account is invalid for this route")
            .When(model => !string.IsNullOrWhiteSpace(model.MainAccount));
        RuleFor(x => x.DeliveryBody)
            .NotEmpty()
            .MustAsync((deliveryBody, cancellationToken) => BeAValidDeliveryBody(deliveryBody))
            .WithMessage("Delivery Body is invalid for this route");
        RuleFor(model => model)
            .MustAsync((model, cancellationToken) => BeAllowedCombination(model))
            .WithMessage("Account / Scheme / Delivery Body combination is invalid")
            .When(x => !string.IsNullOrWhiteSpace(x.DeliveryBody) && !string.IsNullOrWhiteSpace(x.SchemeCode) && !string.IsNullOrWhiteSpace(x.MainAccount));
        RuleFor(model => model)
            .Must((model) => AllRouteValuesMustNotBeEmpty(_route))
            .WithMessage("Account / Organisation / PaymentType / Scheme is required");
    }

    private bool HaveNoMoreThanTwoDecimalPlaces(decimal value)
    {
        return Regex.IsMatch(value.ToString(CultureInfo.InvariantCulture), RegexConstants.TwoDecimalPlaces);
    }
    
    private bool AllRouteValuesMustNotBeEmpty(FieldsRoute route)
    {
        if (string.IsNullOrWhiteSpace(route.AccountType) || string.IsNullOrWhiteSpace(route.Organisation) ||
            string.IsNullOrWhiteSpace(route.PaymentType) || string.IsNullOrWhiteSpace(route.SchemeType))
        {
            return false;
        }

        return true;
    }

    private async Task<bool> BeAValidSchemeCodes(string schemeCode)
    {
        var accountType = _route.AccountType ?? "";
        var organisation = _route.Organisation ?? "";
        var paymentType = _route.PaymentType ?? "";
        var schemeType = _route.SchemeType ?? "";

        if (string.IsNullOrWhiteSpace(accountType) || string.IsNullOrWhiteSpace(organisation) ||
            string.IsNullOrWhiteSpace(paymentType) || string.IsNullOrWhiteSpace(schemeType))
        {
            return false;
        }

        var schemeCodes = await _cachedReferenceDataApi.GetSchemeCodesForRouteAsync(accountType, organisation, paymentType, schemeType);

        if (!schemeCodes.IsSuccess || !schemeCodes.Data.Any())
        {
            return false;
        }

        return schemeCodes.Data.Any(x => x.Code.ToLower() == schemeCode.ToLower());
    }

    private async Task<bool> BeAValidFundCode(string fundCode)
    {
        var accountType = _route.AccountType ?? "";
        var organisation = _route.Organisation ?? "";
        var paymentType = _route.PaymentType ?? "";
        var schemeType = _route.SchemeType ?? "";

        if (string.IsNullOrWhiteSpace(accountType) || string.IsNullOrWhiteSpace(organisation) ||
            string.IsNullOrWhiteSpace(paymentType) || string.IsNullOrWhiteSpace(schemeType))
        {
            return false;
        }

        var fundCodes = await _cachedReferenceDataApi.GetFundCodesForRouteAsync(accountType, organisation, paymentType, schemeType);

        if (!fundCodes.IsSuccess || !fundCodes.Data.Any())
        {
            return false;
        }

        return fundCodes.Data.Any(x => x.Code.ToLower() == fundCode.ToLower());
    }

    private async Task<bool> BeAValidMainAccountCode(string mainAccountCode)
    {
        var accountType = _route.AccountType ?? "";
        var organisation = _route.Organisation ?? "";
        var paymentType = _route.PaymentType ?? "";
        var schemeType = _route.SchemeType ?? "";

        if (string.IsNullOrWhiteSpace(accountType) || string.IsNullOrWhiteSpace(organisation) ||
            string.IsNullOrWhiteSpace(paymentType) || string.IsNullOrWhiteSpace(schemeType))
        {
            return false;
        }

        var mainAccountCodes = await _cachedReferenceDataApi.GetMainAccountCodesForRouteAsync(accountType, organisation, paymentType, schemeType);

        if (!mainAccountCodes.IsSuccess || !mainAccountCodes.Data.Any())
        {
            return false;
        }

        return mainAccountCodes.Data.Any(x => x.Code.ToLower() == mainAccountCode.ToLower());
    }

    private async Task<bool> BeAValidDeliveryBody(string deliveryBodyCode)
    {
        var accountType = _route.AccountType ?? "";
        var organisation = _route.Organisation ?? "";
        var paymentType = _route.PaymentType ?? "";
        var schemeType = _route.SchemeType ?? "";

        if (string.IsNullOrWhiteSpace(accountType) || string.IsNullOrWhiteSpace(organisation) ||
            string.IsNullOrWhiteSpace(paymentType) || string.IsNullOrWhiteSpace(schemeType))
        {
            return false;
        }

        var deliveryBodyCodes = await _cachedReferenceDataApi.GetDeliveryBodyCodesForRouteAsync(accountType, organisation, paymentType, schemeType);

        if (!deliveryBodyCodes.IsSuccess || !deliveryBodyCodes.Data.Any())
        {
            return false;
        }

        return deliveryBodyCodes.Data.Any(x => x.Code.ToLower() == deliveryBodyCode.ToLower());
    }

    private async Task<IEnumerable<CombinationForRoute>?> GetCombinationsListForRouteAsync()
    {
        var accountType = _route.AccountType ?? "";
        var organisation = _route.Organisation ?? "";
        var paymentType = _route.PaymentType ?? "";
        var schemeType = _route.SchemeType ?? "";

        if (string.IsNullOrWhiteSpace(accountType) || string.IsNullOrWhiteSpace(organisation) ||
            string.IsNullOrWhiteSpace(paymentType) || string.IsNullOrWhiteSpace(schemeType))
        {
            return null;
        }

        var combinationsForRoute = await _cachedReferenceDataApi.GetCombinationsListForRouteAsync(accountType, organisation, paymentType, schemeType);

        if (!combinationsForRoute.IsSuccess)
        {
            return null;
        }

        return !combinationsForRoute.Data.Any() ? new List<CombinationForRoute>() : combinationsForRoute.Data;
    }

    private async Task<bool> BeAllowedCombination(InvoiceLine invoiceLine)
    {
        var combinations = await this.GetCombinationsListForRouteAsync();

        if (combinations == null)
        {
            return false;
        }
        
        var combinationForRoutes = combinations.ToList();

        if (!combinationForRoutes.Any())
        {
            return true;
        }

        return combinationForRoutes.Exists(x =>
            string.Equals(x.DeliveryBodyCode, invoiceLine.DeliveryBody, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(x.SchemeCode, invoiceLine.SchemeCode, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(x.AccountCode, invoiceLine.MainAccount, StringComparison.OrdinalIgnoreCase));
    }
}

