using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.API.Interfaces;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Invoices.Api.Models;

public class InvoiceValidator : AbstractValidator<Invoice>
{
    private readonly IReferenceDataApi _referenceDataApi;
    private readonly string[] _validAccountTypes = { "AP", "AR" };

    private readonly SchemeCodeRoute _schemeCodeRoute;

    public InvoiceValidator(IReferenceDataApi referenceDataApi)
    {
        _schemeCodeRoute = new SchemeCodeRoute()
        {
            InvoiceType =  RuleFor(x => x.InvoiceType).NotNull().ToString(),
            Organisation = RuleFor(x => x.Organisation).NotEmpty().ToString(),
            PaymentType = RuleFor(x => x.PaymentType).NotEmpty().ToString(),
            SchemeType = RuleFor(x => x.SchemeType).NotEmpty().ToString(),
        };
        _referenceDataApi = referenceDataApi;

        RuleFor(x => x.Id)
            .NotEmpty();
        //RuleFor(x => x.SchemeType)
        //    .NotEmpty();
        //RuleFor(x => x.Organisation)
        //    .NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
       // RuleFor(x => x.InvoiceType).NotNull();
        RuleFor(x => x.AccountType)
            .NotEmpty()
            .Must(x => this._validAccountTypes.Contains(x.ToUpper()))
            .WithMessage("Account Type is invalid. Should be AP or AR");
        //RuleFor(x => x.PaymentType)
        //    .NotEmpty();
        RuleFor(x => x.PaymentRequests)
            .NotEmpty();
        RuleForEach(x => x.PaymentRequests).SetValidator(x => new InvoiceHeaderValidator(_referenceDataApi, _schemeCodeRoute)).When(x => x.PaymentRequests != null);

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