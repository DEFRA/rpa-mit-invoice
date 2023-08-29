
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using FluentValidation;

namespace Invoices.Api.Models;

public class BulkInvoiceValidator : AbstractValidator<BulkInvoices>
{
    public BulkInvoiceValidator(IReferenceDataApi referenceDataApi)
    {
        var _referenceDataApi = referenceDataApi;

        RuleFor(x => x.Reference).NotEmpty();
        RuleFor(x => x.SchemeType).NotEmpty();

        RuleForEach(x => x.Invoices).NotEmpty().SetValidator(new InvoiceValidator(_referenceDataApi));
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



