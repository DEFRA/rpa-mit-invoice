using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using EST.MIT.Invoice.Api.Services.API.Models;
using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.API.Interfaces;
using FluentValidation;
using Invoices.Api.Util;

namespace Invoices.Api.Models;

public class InvoiceHeaderValidator : AbstractValidator<InvoiceHeader>
{
    public InvoiceHeaderValidator(IReferenceDataApi referenceDataApi, FieldsRoute route)
    {
        _organisation = organisation;
        var _referenceDataApi = referenceDataApi;

        RuleFor(x => x.AgreementNumber).NotEmpty();
        RuleFor(x => x.AppendixReferences).NotEmpty();
        RuleFor(x => x.FRN).NotEmpty();
        RuleFor(x => x.InvoiceLines).NotEmpty();
        RuleFor(x => x.SourceSystem).NotEmpty();
        RuleFor(x => x.ContractNumber).NotEmpty();
        RuleFor(x => x.DeliveryBody).NotEmpty();
        RuleFor(x => x.DueDate).NotEmpty();
        RuleFor(x => x.MarketingYear).NotEmpty();
        RuleFor(x => x.PaymentRequestId).NotEmpty()
            .WithMessage("PaymentRequestId is missing")
            .MaximumLength(20)
            .WithMessage("PaymentRequestId must not be more than 20 characters")
            .MinimumLength(1)
            .WithMessage("PaymentRequestId must contain at least one character")
            .Must(BeWithoutSpaces)
            .WithMessage("PaymentRequestId cannot contain spaces");
        RuleFor(x => x.PaymentRequestNumber).NotEmpty();
        RuleFor(x => x.Value)
            .NotEqual(0)
            .WithMessage("Invoice value must be non-zero")
            .Must(HaveNoMoreThanTwoDecimalPlaces)
            .WithMessage("Invoice value cannot be more than 2dp")
            .Must(value => HaveAMaximumAbsoluteValueOf(value, 999999999))
            .WithMessage("The ABS invoice value must be less than 1 Billion");
        RuleForEach(x => x.InvoiceLines).SetValidator(new InvoiceLineValidator(_referenceDataApi, route));

        RuleFor(model => model)
            .Must(HaveSameCurrencyTypes)
            .WithMessage("Cannot mix currencies in an invoice")
            .Must(HaveAValueEqualToTheSumOfLinesValue)
            .WithMessage((model) => $"Invoice Value ({model.Value}) does not equal the sum of Line Values ({model.InvoiceLines.Sum(x => x.Value)})")
            .When(model => model.InvoiceLines != null && model.InvoiceLines.Any());

        RuleFor(model => model)
            .Must(model => HaveAnAllowedCustomerId(model.DeliveryBody, model.SingleBusinessIdentifier, model.FirmReferenceNumber, model.VendorId))
            .WithMessage("The customer id is not valid for this delivery body and organisation combination");
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

    private static bool HaveAMaximumAbsoluteValueOf(decimal value, decimal absoluteValue)
    {
        return Math.Abs(value) <= absoluteValue;
    }

    private bool HaveAValueEqualToTheSumOfLinesValue(InvoiceHeader invoiceHeader)
    {
        var invoiceValue = invoiceHeader.Value;
        var sumOfLinesValue = invoiceHeader.InvoiceLines.Sum(x => x.Value);

        return invoiceValue == sumOfLinesValue;
    }

    private bool BeWithoutSpaces(string id)
    {
        if (Regex.IsMatch(id, @"\s"))
        {
            return false;
        }
        return true;
    }

    private bool HaveAnAllowedCustomerId(string deliveryBody, string singleBusinessIdentifier, string firmReferenceNumber, string vendorId)
    {
        var allowedDeliveryBodies = new List<string>(){"DE", "DF"};
        var organisationCodeForSBIorFRN = "RDT";
        var organisationCodeForVendorIdorFRN = "NE";

        if (allowedDeliveryBodies.Contains(deliveryBody) || this._organisation == organisationCodeForSBIorFRN)
        {
            var customerId = !string.IsNullOrWhiteSpace(singleBusinessIdentifier) ? singleBusinessIdentifier : "";
            if (string.IsNullOrWhiteSpace(customerId))
            {
                customerId = !string.IsNullOrWhiteSpace(firmReferenceNumber) ? firmReferenceNumber : "";
            }

            var isNumeric = long.TryParse(customerId, out _);
            if (isNumeric && (customerId.Length is > 8 and < 11))
            {
                return true;
            }

            return false;
        }
        else if (this._organisation == organisationCodeForVendorIdorFRN)
        {
            var customerId = !string.IsNullOrWhiteSpace(vendorId) ? vendorId : "";
            if (string.IsNullOrWhiteSpace(customerId))
            {
                customerId = !string.IsNullOrWhiteSpace(firmReferenceNumber) ? firmReferenceNumber : "";
            }

            var isNumeric = long.TryParse(customerId, out _);
            if (isNumeric && (customerId.Length is > 5 and < 11))
            {
                return true;
            }

            return false;
        }
        else
        {
            var customerId = !string.IsNullOrWhiteSpace(firmReferenceNumber) ? firmReferenceNumber : "";

            var isNumeric = long.TryParse(customerId, out _);
            if (isNumeric && (customerId.Length == 10))
            {
                return true;
            }

            return false;
        }
    }
}

