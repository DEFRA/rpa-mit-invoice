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

        RuleFor(invoiceHeader => invoiceHeader)
            .Must(HaveSameCurrencyTypes)
            .WithMessage("Cannot mix currencies in an invoice")
            .Must(HaveAValueEqualToTheSumOfLinesValue)
            .WithMessage((invoiceHeader) => $"Invoice Value ({invoiceHeader.Value}) does not equal the sum of Line Values ({invoiceHeader.InvoiceLines.Sum(x => x.Value)})")
            .When(invoiceHeader => invoiceHeader.InvoiceLines != null && invoiceHeader.InvoiceLines.Any());

        RuleFor(invoiceHeader => invoiceHeader)
            .Must(invoiceHeader => HaveOnlySBIOrFRNOrVendorId(invoiceHeader.SingleBusinessIdentifier, invoiceHeader.FirmReferenceNumber, invoiceHeader.VendorId))
            .WithMessage("Invoice must only have Single Business Identifier (SBI), Firm Reference Number (FRN) or Vendor Id")
            .Must(invoiceHeader => HaveValidVendorId(invoiceHeader.VendorId))
            .WithMessage("Vendor Id must be between 6 and 10 characters long")
            .When(invoiceHeader => !string.IsNullOrWhiteSpace(invoiceHeader.VendorId) && invoiceHeader is { SingleBusinessIdentifier: 0, FirmReferenceNumber: 0 },
                ApplyConditionTo.CurrentValidator)
            .Must(invoiceHeader => invoiceHeader.FirmReferenceNumber.ToString().Length == 10)
            .WithMessage("Firm Reference Number (FRN) must be between 1000000000 and 9999999999")
            .When(invoiceHeader => string.IsNullOrWhiteSpace(invoiceHeader.VendorId) && invoiceHeader.SingleBusinessIdentifier == 0
                && invoiceHeader.FirmReferenceNumber != 0,
                ApplyConditionTo.CurrentValidator)
            .Must(invoiceHeader => invoiceHeader.SingleBusinessIdentifier.ToString().Length == 9)
            .WithMessage("Single Business Identifier (SBI) must be between 100000000 and 999999999")
            .When(invoiceHeader => string.IsNullOrWhiteSpace(invoiceHeader.VendorId) && invoiceHeader.FirmReferenceNumber == 0
                && invoiceHeader.SingleBusinessIdentifier != 0,
                ApplyConditionTo.CurrentValidator);
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

    private static bool HaveOnlySBIOrFRNOrVendorId(int singleBusinessIdentifier, long firmReferenceNumber, string vendorId)
    {
        // return true if only one of the three values is populated
        return (singleBusinessIdentifier != 0 && firmReferenceNumber == 0 && string.IsNullOrWhiteSpace(vendorId))
            || (singleBusinessIdentifier == 0 && firmReferenceNumber != 0 && string.IsNullOrWhiteSpace(vendorId))
            || (singleBusinessIdentifier == 0 && firmReferenceNumber == 0 && !string.IsNullOrWhiteSpace(vendorId));
    }

    private static bool HaveValidVendorId(string vendorId)
    {
        return vendorId.Length is >= 6 and <= 10;
    }
}