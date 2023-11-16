using System.Globalization;
using System.Text.RegularExpressions;
using EST.MIT.Invoice.Api.Services.Api.Models;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using FluentValidation;
using EST.MIT.Invoice.Api.Util;

namespace EST.MIT.Invoice.Api.Models;

public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
{
    private readonly string[] _validCurrencyTypes = { "GBP", "EUR" };
    public PaymentRequestValidator(IReferenceDataApi referenceDataApi, ICachedReferenceDataApi cachedReferenceDataApi, FieldsRoute route, string status)
    {
        RuleFor(paymentRequest => paymentRequest.AgreementNumber).NotEmpty();
        RuleFor(paymentRequest => paymentRequest.InvoiceLines).NotEmpty().When(x => HaveStatusFieldEqualPendingOrApproval(status));
        RuleFor(paymentRequest => paymentRequest.SourceSystem).NotEmpty();
        RuleFor(paymentRequest => paymentRequest.DueDate).NotEmpty();
        RuleFor(paymentRequest => paymentRequest.MarketingYear).NotEmpty();
        RuleFor(paymentRequest => paymentRequest.PaymentRequestId).NotEmpty()
            .WithMessage("PaymentRequestId is missing")
            .MaximumLength(20)
            .WithMessage("PaymentRequestId must not be more than 20 characters")
            .MinimumLength(1)
            .WithMessage("PaymentRequestId must contain at least one character")
            .Must(BeWithoutSpaces)
            .WithMessage("PaymentRequestId cannot contain spaces");
        RuleFor(paymentRequest => paymentRequest.PaymentRequestNumber).NotEmpty();
        RuleFor(paymentRequest => paymentRequest.Value)
            .NotEqual(0).When(x => HaveStatusFieldEqualPendingOrApproval(status))
            .WithMessage("Invoice value must be non-zero")
            .Must(HaveNoMoreThanTwoDecimalPlaces)
            .WithMessage("Invoice value cannot be more than 2dp")
            .Must(value => HaveAMaximumAbsoluteValueOf(value, 999999999))
            .WithMessage("The ABS invoice value must be less than 1 Billion");

        RuleForEach(paymentRequest => paymentRequest.InvoiceLines).SetValidator(new InvoiceLineValidator(referenceDataApi, route, cachedReferenceDataApi));

        RuleFor(paymentRequest => paymentRequest)
            .Must(HaveAValueEqualToTheSumOfLinesValue)
            .WithMessage((paymentRequest) => $"Invoice Value ({paymentRequest.Value}) does not equal the sum of Line Values ({paymentRequest.InvoiceLines.Sum(x => x.Value)})")
            .When(paymentRequest => paymentRequest.InvoiceLines != null && paymentRequest.InvoiceLines.Any())
            .Must(paymentRequest => HaveOnlySBIOrFRNOrVendor(paymentRequest.SBI, paymentRequest.FRN, paymentRequest.Vendor))
            .WithMessage("Invoice must only have SBI, FRN or Vendor");

        RuleFor(paymentRequest => paymentRequest.Currency)
            .NotEmpty()
            .Must(paymentRequest_currency => paymentRequest_currency is not null && _validCurrencyTypes.Contains(paymentRequest_currency.ToUpper()))
            .WithMessage("Currency must be GBP or EUR");

        RuleFor(paymentRequest => paymentRequest.FRN)
            .InclusiveBetween(1000000000, 9999999999)
            .WithMessage("FRN is not in valid range (1000000000 .. 9999999999)")
            .When(paymentRequest => string.IsNullOrWhiteSpace(paymentRequest.Vendor) && paymentRequest.SBI == 0
                    && paymentRequest.FRN != 0,
                ApplyConditionTo.CurrentValidator);

        RuleFor(paymentRequest => paymentRequest.OriginalInvoiceNumber)
            .NotEmpty()
            .WithMessage("Please input Original AP Reference")
            .When(paymentRequest => route.AccountType == "AR");

        RuleFor(paymentRequest => paymentRequest.OriginalSettlementDate)
            .NotEmpty()
            .WithMessage("Please input Original AP Settlement Date")
            .When(paymentRequest => route.AccountType == "AR");

        RuleFor(paymentRequest => paymentRequest.RecoveryDate)
            .NotEmpty()
            .WithMessage("Please input earliest date possible recovery identified")
            .When(paymentRequest => route.AccountType == "AR");

        RuleFor(paymentRequest => paymentRequest.SBI)
            .InclusiveBetween(105000000, 999999999)
            .WithMessage("SBI is not in valid range (105000000 .. 999999999)")
            .When(paymentRequest => string.IsNullOrWhiteSpace(paymentRequest.Vendor) && paymentRequest.FRN == 0
                    && paymentRequest.SBI != 0,
                ApplyConditionTo.CurrentValidator);

        RuleFor(paymentRequest => paymentRequest.Vendor)
            .Length(6)
            .WithMessage("Vendor must be 6 characters")
            .When(paymentRequest => !string.IsNullOrWhiteSpace(paymentRequest.Vendor) && paymentRequest is { SBI: 0, FRN: 0 },
                ApplyConditionTo.CurrentValidator);
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

    public static bool HaveStatusFieldEqualPendingOrApproval(string status)
    {
        return status.ToLower() == "pendingapproval" || status.ToLower() == "approved";
    }
}