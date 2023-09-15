using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using FluentValidation;

namespace Invoices.Api.Models;

public class PaymentRequestsBatchValidator : AbstractValidator<PaymentRequestsBatch>
{
    private readonly IReferenceDataApi _referenceDataApi;
    private readonly ICachedReferenceDataApi _cachedReferenceDataApi;
    private readonly string[] _validAccountTypes = { "AP", "AR" };

    private readonly FieldsRoute _route;

    public PaymentRequestsBatchValidator(IReferenceDataApi referenceDataApi, ICachedReferenceDataApi cachedReferenceDataApi)
    {
        _referenceDataApi = referenceDataApi;
        _cachedReferenceDataApi = cachedReferenceDataApi;

        _route = new FieldsRoute()
        {
            InvoiceType = RuleFor(x => x.InvoiceType).NotNull().ToString(),
            Organisation = RuleFor(x => x.Organisation).NotEmpty().ToString(),
            PaymentType = RuleFor(x => x.PaymentType).NotEmpty().ToString(),
            SchemeType = RuleFor(x => x.SchemeType).NotEmpty().ToString(),
        };

        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
        RuleFor(x => x.AccountType)
            .NotEmpty()
            .Must(x => this._validAccountTypes.Contains(x.ToUpper()))
            .WithMessage("Account Type is invalid. Should be AP or AR");
        RuleFor(x => x.PaymentRequests)
            .NotEmpty();
        RuleForEach(x => x.PaymentRequests).SetValidator(x => new InvoiceHeaderValidator(_referenceDataApi, _cachedReferenceDataApi, _route)).When(x => x.PaymentRequests != null);

        RuleFor(model => model)
            .MustAsync((x, cancellation) => BeAValidSchemeType(x))
            .WithMessage("Scheme Type is invalid")
            .When(model => !string.IsNullOrWhiteSpace(model.InvoiceType) && !string.IsNullOrWhiteSpace(model.Organisation));

        RuleFor(model => model)
            .MustAsync((x, cancellation) => BeAValidPaymentType(x))
            .WithMessage("Payment Type is invalid")
            .When(model => !string.IsNullOrWhiteSpace(model.InvoiceType) && !string.IsNullOrWhiteSpace(model.Organisation) && !string.IsNullOrWhiteSpace(model.SchemeType));

        RuleFor(model => model)
            .MustAsync((x, CancellationToken) => BeAValidOrganisationCode(x))
            .WithMessage("Organisation is Invalid")
            .When(model => !string.IsNullOrWhiteSpace(model.Organisation) && !string.IsNullOrWhiteSpace(model.InvoiceType));
    }

    private async Task<bool> BeAValidSchemeType(PaymentRequestsBatch paymentRequestsBatch)
    {
        if (string.IsNullOrWhiteSpace(paymentRequestsBatch.SchemeType))
        {
            return false;
        }

        var schemeTypes = await _referenceDataApi.GetSchemeTypesAsync(paymentRequestsBatch.InvoiceType, paymentRequestsBatch.Organisation);

        if (!schemeTypes.IsSuccess || !schemeTypes.Data.Any())
        {
            return false;
        }

        return schemeTypes.Data.Any(x => x.Code.ToLower() == paymentRequestsBatch.SchemeType.ToLower());
    }

    private async Task<bool> BeAValidPaymentType(PaymentRequestsBatch paymentRequestsBatch)
    {
        if (string.IsNullOrWhiteSpace(paymentRequestsBatch.PaymentType))
        {
            return false;
        }

        var paymentTypes = await _referenceDataApi.GetPaymentTypesAsync(paymentRequestsBatch.InvoiceType, paymentRequestsBatch.Organisation, paymentRequestsBatch.SchemeType);

        if (!paymentTypes.IsSuccess || !paymentTypes.Data.Any())
        {
            return false;
        }

        return paymentTypes.Data.Any(x => x.Code.ToLower() == paymentRequestsBatch.PaymentType.ToLower());
    }

    private async Task<bool> BeAValidOrganisationCode(PaymentRequestsBatch paymentRequestsBatch)
    {
        var organisationCodes = await _referenceDataApi.GetOrganisationsAsync(paymentRequestsBatch.InvoiceType);

        if (!organisationCodes.IsSuccess || !organisationCodes.Data.Any())
        {
            return false;
        }

        return organisationCodes.Data.Any(x => x.Code.ToLower() == paymentRequestsBatch.Organisation.ToLower());
    }
}