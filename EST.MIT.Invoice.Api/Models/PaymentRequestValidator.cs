using System.Globalization;
using System.Text.RegularExpressions;
using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using FluentValidation;
using EST.MIT.Invoice.Api.Util;

namespace EST.MIT.Invoice.Api.Models;

public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
{
    public PaymentRequestValidator(IReferenceDataApi referenceDataApi, ICachedReferenceDataApi cachedReferenceDataApi, FieldsRoute route, string status)
    {
        RuleFor(x => x.AgreementNumber).NotEmpty();
        RuleFor(x => x.AppendixReferences).NotEmpty();
        RuleFor(x => x.InvoiceLines).NotEmpty();
        RuleFor(x => x.SourceSystem).NotEmpty();
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
            .Must(x => HaveStatusFieldEqualPendingOrApproval(status))
            .WithMessage("Status Field not equal to pending or approval")
            .NotEqual(0)
            .WithMessage("Invoice value must be non-zero")
            .Must(HaveNoMoreThanTwoDecimalPlaces)
            .WithMessage("Invoice value cannot be more than 2dp")
            .Must(value => HaveAMaximumAbsoluteValueOf(value, 999999999))
            .WithMessage("The ABS invoice value must be less than 1 Billion");

        RuleForEach(x => x.InvoiceLines).SetValidator(new InvoiceLineValidator(referenceDataApi, route, cachedReferenceDataApi));

        RuleFor(invoiceHeader => invoiceHeader)
            .Must(HaveSameCurrencyTypes)
            .WithMessage("Cannot mix currencies in an invoice")
            .Must(x => HaveStatusFieldEqualPendingOrApproval(status))
            .WithMessage("Status Field not equal to pending or approval")
            .Must(HaveAValueEqualToTheSumOfLinesValue)
            .WithMessage((invoiceHeader) => $"Invoice Value ({invoiceHeader.Value}) does not equal the sum of Line Values ({invoiceHeader.InvoiceLines.Sum(x => x.Value)})")
            .When(invoiceHeader => invoiceHeader.InvoiceLines != null && invoiceHeader.InvoiceLines.Any())
            .Must(invoiceHeader => HaveOnlySBIOrFRNOrVendor(invoiceHeader.SBI, invoiceHeader.FRN, invoiceHeader.Vendor))
            .WithMessage("Invoice must only have SBI, FRN or Vendor");

        RuleFor(invoiceHeader => invoiceHeader.FRN)
            .InclusiveBetween(1000000000, 9999999999)
            .WithMessage("FRN is not in valid range (1000000000 .. 9999999999)")
            .When(invoiceHeader => string.IsNullOrWhiteSpace(invoiceHeader.Vendor) && invoiceHeader.SBI == 0
                    && invoiceHeader.FRN != 0,
                ApplyConditionTo.CurrentValidator);

        RuleFor(invoiceHeader => invoiceHeader.SBI)
            .InclusiveBetween(105000000, 999999999)
            .WithMessage("SBI is not in valid range (105000000 .. 999999999)")
            .When(invoiceHeader => string.IsNullOrWhiteSpace(invoiceHeader.Vendor) && invoiceHeader.FRN == 0
                    && invoiceHeader.SBI != 0,
                ApplyConditionTo.CurrentValidator);

        RuleFor(invoiceHeader => invoiceHeader.Vendor)
            .Length(6)
            .WithMessage("Vendor must be 6 characters")
            .When(invoiceHeader => !string.IsNullOrWhiteSpace(invoiceHeader.Vendor) && invoiceHeader is { SBI: 0, FRN: 0 },
                ApplyConditionTo.CurrentValidator);
    }

    private bool HaveSameCurrencyTypes(PaymentRequest paymentRequest)
    {
        // get all the currency types from the invoice lines
        var allCurrencyTypes = paymentRequest.InvoiceLines.Select(x => x.Currency).Distinct();

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

    private bool HaveAValueEqualToTheSumOfLinesValue(PaymentRequest paymentRequest)
    {
        var invoiceValue = paymentRequest.Value;
        var sumOfLinesValue = paymentRequest.InvoiceLines.Sum(x => x.Value);

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

    private static bool HaveOnlySBIOrFRNOrVendor(int sbi, long frn, string vendor)
    {
        // return true if only one of the three values is populated
        return (sbi != 0 && frn == 0 && string.IsNullOrWhiteSpace(vendor))
            || (sbi == 0 && frn != 0 && string.IsNullOrWhiteSpace(vendor))
            || (sbi == 0 && frn == 0 && !string.IsNullOrWhiteSpace(vendor));
    }

    private bool HaveStatusFieldEqualPendingOrApproval(string status)
    {
        return status.ToLower() == "pendingapproval" || status.ToLower() == "approval";
    }
}