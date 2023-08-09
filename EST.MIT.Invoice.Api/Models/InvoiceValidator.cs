using EST.MIT.Invoice.Api.Services.API.Interfaces;
using FluentValidation;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Invoices.Api.Models;

public class InvoiceValidator : AbstractValidator<Invoice>
{
    private readonly IReferenceDataApi _referenceDataApi;
    private readonly string[] _validAccountTypes = { "AP", "AR" };

    public InvoiceValidator(IReferenceDataApi referenceDataApi)
    {
        _referenceDataApi = referenceDataApi;
        RuleFor(model => model)
                           .Must((x, Cancellation) => BeNotNullOrEmpty(x))
                           .WithMessage("Invoice ID must not be null or empty")
                           .Must((x, cancellation) => BeMaximumCharacterTwenty(x))
                           .WithMessage("Invoice Id must not be more than 20 characters")
                           .Must((x, cancellation) => BeMinimumCharacterOne(x))
                           .WithMessage("Invoice Id must contain at least one character")
                           .Must((x, cancellation) => BeWithoutSpaces(x))
                           .WithMessage("Invoice ID cannot contain spaces");
        RuleFor(x => x.SchemeType)
            .NotEmpty();
        RuleFor(x => x.Organisation)
            .NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
        RuleFor(x => x.InvoiceType).NotNull();
        RuleFor(x => x.AccountType)
            .NotEmpty()
            .Must(x => this._validAccountTypes.Contains(x.ToUpper()))
            .WithMessage("Account Type is invalid. Should be AP or AR");
        RuleFor(x => x.PaymentType)
            .NotEmpty();
        RuleFor(x => x.PaymentRequests)
            .NotEmpty();
        RuleForEach(x => x.PaymentRequests).SetValidator(new InvoiceHeaderValidator()).When(x => x.PaymentRequests != null);

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

    private static bool BeNotNullOrEmpty(Invoice invoice)
    {
        if (string.IsNullOrEmpty(invoice.Id))
        {
            return false;
        }

        return true;
    }

    private static bool BeMaximumCharacterTwenty(Invoice invoice)
    {
        if (invoice.Id.Length > 20)
        {
            return false;
        }
        return true;
    }

    private static bool BeMinimumCharacterOne(Invoice invoice)
    {
        if (invoice.Id.Length < 1)
        {
            return false;
        }
        return true;
    }

    private static bool BeWithoutSpaces(Invoice invoice)
    {
        if (Regex.IsMatch(invoice.Id, @"\s"))
        {
            return false;
        }
        return true;
    }

    private async Task<bool> BeAValidSchemeType(Invoice invoice)
    {
        if (string.IsNullOrWhiteSpace(invoice.SchemeType))
        {
            return false;
        }

        var schemeTypes = await _referenceDataApi.GetSchemeTypesAsync(invoice.InvoiceType, invoice.Organisation);

        if (!schemeTypes.IsSuccess || !schemeTypes.Data.Any())
        {
            return false;
        }

        return schemeTypes.Data.Any(x => x.Code.ToLower() == invoice.SchemeType.ToLower());
    }

    private async Task<bool> BeAValidPaymentType(Invoice invoice)
    {
        if (string.IsNullOrWhiteSpace(invoice.PaymentType))
        {
            return false;
        }

        var paymentTypes = await _referenceDataApi.GetPaymentTypesAsync(invoice.InvoiceType, invoice.Organisation, invoice.SchemeType);

        if (!paymentTypes.IsSuccess || !paymentTypes.Data.Any())
        {
            return false;
        }

        return paymentTypes.Data.Any(x => x.Code.ToLower() == invoice.PaymentType.ToLower());
    }

    private async Task<bool> BeAValidOrganisationCode(Invoice invoice)
    {
        var organisationCodes = await _referenceDataApi.GetOrganisationsAsync(invoice.InvoiceType);

        if (!organisationCodes.IsSuccess || !organisationCodes.Data.Any())
        {
            return false;
        }

        return organisationCodes.Data.Any(x => x.Code.ToLower() == invoice.Organisation.ToLower());
    }
}