
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using FluentValidation;

namespace EST.MIT.Invoice.Api.Models;

public class BulkInvoiceValidator : AbstractValidator<BulkInvoices>
{
    public BulkInvoiceValidator(IReferenceDataApi referenceDataApi, ICachedReferenceDataApi cachedReferenceDataApi)
    {
        var _referenceDataApi = referenceDataApi;

        RuleFor(x => x.Reference).NotEmpty();
        RuleFor(x => x.SchemeType).NotEmpty();

        RuleFor(x => x.Invoices)
            .NotEmpty();

        RuleForEach(x => x.Invoices)
            .SetValidator(new PaymentRequestsBatchValidator(_referenceDataApi, cachedReferenceDataApi));

        RuleFor(model => model)
            .Must(HaveNoDuplicatedPaymentRequestIds)
            .WithMessage("Payment Request Id is duplicated in this batch");
    }

    public bool HaveNoDuplicatedPaymentRequestIds(BulkInvoices bulkInvoice)
    {
        return bulkInvoice.Invoices
            .SelectMany(x => x.PaymentRequests)
            .GroupBy(x => x.PaymentRequestId)
            .All(x => x.Count() == 1);
    }
}



