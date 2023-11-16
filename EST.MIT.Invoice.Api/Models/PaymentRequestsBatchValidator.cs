using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using FluentValidation;

namespace EST.MIT.Invoice.Api.Models;

public class PaymentRequestsBatchValidator : AbstractValidator<PaymentRequestsBatch>
{
    private readonly IReferenceDataApi _referenceDataApi;
    private readonly ICachedReferenceDataApi _cachedReferenceDataApi;
    private readonly string[] _validAccountTypes = { "AP", "AR" };

    public PaymentRequestsBatchValidator(IReferenceDataApi referenceDataApi, ICachedReferenceDataApi cachedReferenceDataApi)
    {
        _referenceDataApi = referenceDataApi;
        _cachedReferenceDataApi = cachedReferenceDataApi;

        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
        RuleFor(x => x.SchemeType).NotEmpty();
        RuleFor(x => x.AccountType)
            .NotEmpty()
            .Must(x => this._validAccountTypes.Contains(x.ToUpper()))
            .WithMessage("Account Type is invalid. Should be AP or AR");
        RuleForEach(x => x.PaymentRequests)
            .NotEmpty()
            .SetValidator((paymentRequest) => new PaymentRequestValidator(_referenceDataApi, _cachedReferenceDataApi, new FieldsRoute() { AccountType = paymentRequest.AccountType, Organisation = paymentRequest.Organisation, PaymentType = paymentRequest.PaymentType, SchemeType = paymentRequest.SchemeType }, paymentRequest.Status))
            .When(x => x.PaymentRequests != null);

        RuleFor(model => model)
            .MustAsync((x, cancellation) => BeAValidSchemeType(x))
            .WithMessage("Scheme Type is invalid")
            .When(model => !string.IsNullOrWhiteSpace(model.AccountType) && !string.IsNullOrWhiteSpace(model.Organisation));

        RuleFor(model => model)
            .MustAsync((x, cancellation) => BeAValidPaymentType(x))
            .WithMessage("Payment Type is invalid")
            .When(model => !string.IsNullOrWhiteSpace(model.AccountType) && !string.IsNullOrWhiteSpace(model.Organisation) && !string.IsNullOrWhiteSpace(model.SchemeType));

        RuleFor(model => model)
            .MustAsync((x, CancellationToken) => BeAValidOrganisationCode(x))
            .WithMessage("Organisation is Invalid")
            .When(model => !string.IsNullOrWhiteSpace(model.Organisation) && !string.IsNullOrWhiteSpace(model.AccountType));
        RuleFor(model => model.ApproverEmail)
            .NotEmpty()
            .When(model => PaymentRequestValidator.HaveStatusFieldEqualPendingOrApproval(model.Status));
    }

    private async Task<bool> BeAValidSchemeType(PaymentRequestsBatch paymentRequestsBatch)
    {
        if (string.IsNullOrWhiteSpace(paymentRequestsBatch.SchemeType))
        {
            return false;
        }

        var schemeTypes = await _referenceDataApi.GetSchemeTypesAsync(paymentRequestsBatch.AccountType, paymentRequestsBatch.Organisation);

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

        var paymentTypes = await _referenceDataApi.GetPaymentTypesAsync(paymentRequestsBatch.AccountType, paymentRequestsBatch.Organisation, paymentRequestsBatch.SchemeType);

        if (!paymentTypes.IsSuccess || !paymentTypes.Data.Any())
        {
            return false;
        }

        return paymentTypes.Data.Any(x => x.Code.ToLower() == paymentRequestsBatch.PaymentType.ToLower());
    }

    private async Task<bool> BeAValidOrganisationCode(PaymentRequestsBatch paymentRequestsBatch)
    {
        var organisationCodes = await _referenceDataApi.GetOrganisationsAsync(paymentRequestsBatch.AccountType);

        if (!organisationCodes.IsSuccess || !organisationCodes.Data.Any())
        {
            return false;
        }

        return organisationCodes.Data.Any(x => x.Code.ToLower() == paymentRequestsBatch.Organisation.ToLower());
    }
}