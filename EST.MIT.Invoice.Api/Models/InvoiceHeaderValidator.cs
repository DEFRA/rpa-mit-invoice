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
            .Must(invoiceHeader => HaveOnlySBIOrFRN(invoiceHeader.SingleBusinessIdentifier, invoiceHeader.FirmReferenceNumber))
            .WithMessage("Invoice must only have an SBI or FRN, not both")
            .Must(invoiceHeader => HaveValidSBI(invoiceHeader.SingleBusinessIdentifier))
            .WithMessage("SBI must be 9 characters long")
            .When(invoiceHeader => !string.IsNullOrWhiteSpace(invoiceHeader.SingleBusinessIdentifier)
                && string.IsNullOrWhiteSpace(invoiceHeader.FirmReferenceNumber),
                ApplyConditionTo.CurrentValidator)
            .Must(invoiceHeader => HaveValidFRN(invoiceHeader.FirmReferenceNumber))
            .WithMessage("FRN must be 10 characters long")
            .When(invoiceHeader => string.IsNullOrWhiteSpace(invoiceHeader.SingleBusinessIdentifier)
                && !string.IsNullOrWhiteSpace(invoiceHeader.FirmReferenceNumber),
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

    private static bool HaveOnlySBIOrFRN(string singleBusinessIdentifier, string firmReferenceNumber)
    {
        return string.IsNullOrWhiteSpace(singleBusinessIdentifier) || string.IsNullOrWhiteSpace(firmReferenceNumber);
    }

    private static bool HaveValidSBI(string singleBusinessIdentifier)
    {
        return singleBusinessIdentifier.Length == 9;
    }

    private static bool HaveValidFRN(string firmReferenceNumber)
    {
        return firmReferenceNumber.Length == 10;
    }
}

