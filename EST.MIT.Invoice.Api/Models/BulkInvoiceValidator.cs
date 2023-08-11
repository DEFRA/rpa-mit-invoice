
using EST.MIT.Invoice.Api.Services.API.Interfaces;
using FluentValidation;

namespace Invoices.Api.Models;

public class BulkInvoiceValidator : AbstractValidator<BulkInvoices>
{
    private readonly IReferenceDataApi _referenceDataApi;

    public BulkInvoiceValidator(IReferenceDataApi referenceDataApi)
    {
        _referenceDataApi = referenceDataApi;

        RuleFor(x => x.Reference).NotEmpty();
        RuleFor(x => x.SchemeType).NotEmpty();

        RuleForEach(x => x.Invoices).NotEmpty().SetValidator(new InvoiceValidator(_referenceDataApi));
        RuleFor(model => model)
            .Must(BeNoDuplicate)
            .WithMessage("Invoice ID is duplicated in this batch");
    }

    public bool BeNoDuplicate(BulkInvoices bulkInvoice)
    {
        var notDuplicate = bulkInvoice.Invoices.GroupBy(x => x.Id).All(x => x.Count() == 1);

        if (!notDuplicate)
        {
            return false;
        }
        return true;
    }
}



