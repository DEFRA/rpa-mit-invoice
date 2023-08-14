using EST.MIT.Invoice.Api.Services.API.Interfaces;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Invoices.Api.Models;

public class InvoiceValidator : AbstractValidator<Invoice>
{
    private readonly IReferenceDataApi _referenceDataApi;
    private readonly string[] _validAccountTypes = { "AP", "AR" };

    public InvoiceValidator(IReferenceDataApi referenceDataApi)
    {
        _referenceDataApi = referenceDataApi;
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Invoice Id is missing")
            .MaximumLength(20)
            .WithMessage("Invoice Id must not be more than 20 characters")
            .MinimumLength(1)
            .WithMessage("Invoice Id must contain at least one character")
            .Must(BeWithoutSpaces)
            .WithMessage("Invoice Id cannot contain spaces");
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
            .Must(BeNoDuplicate)
            .WithMessage("Payment request ID is duplicated in this batch");

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

    private bool BeWithoutSpaces(string id)
    {
        if (Regex.IsMatch(id, @"\s"))
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

    public bool BeNoDuplicate(Invoice invoice)
    {
        var notDuplicate = invoice.PaymentRequests.GroupBy(x => x.PaymentRequestId).All(x => x.Count() ==1);

        if (!notDuplicate)
        {
            return false;
        }
        return true;
    }
}