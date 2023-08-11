using EST.MIT.Invoice.Api.Repositories;
using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Services.Api;
using EST.MIT.Invoice.Api.Services.API.Interfaces;
using EST.MIT.Invoice.Api.Util;
using FluentValidation;
using System.Net.Http;

namespace Invoices.Api.Models;

public class BulkInvoiceValidator : AbstractValidator<BulkInvoices>
{
    private readonly IReferenceDataRepository _referenceDataRepository;
    private readonly IHttpClientFactory _httpClientFactory = null;
    private readonly IReferenceDataApi _referenceDataApi;
    private readonly ILogger<ReferenceDataApi> _logger;
    private readonly IHttpContentDeserializer _httpContentDeserializer;

    public BulkInvoiceValidator()
    {
       

        _httpContentDeserializer = new HttpContentDeserializer();
        _logger = new LoggerFactory().CreateLogger<ReferenceDataApi>();

        _referenceDataRepository = new ReferenceDataRepository(_httpClientFactory);
        _referenceDataApi = new ReferenceDataApi(_referenceDataRepository, _logger, _httpContentDeserializer);

        RuleFor(x => x.Reference).NotEmpty();
        RuleFor(x => x.SchemeType).NotEmpty();

        RuleForEach(x => x.Invoices).NotEmpty().SetValidator(new InvoiceValidator(_referenceDataApi));
        RuleFor(model => model)
            .Must(BeNoDuplicate)
            .WithMessage("Invoice ID is duplicated in this batch");
    }

    public bool BeNoDuplicate(BulkInvoices bulkInvoice)
    {
        var notDuplicate = bulkInvoice.Invoices.GroupBy(x => x.Id).All(x=>x.Count()== 1);

        if (!notDuplicate)
        {
            return false;
        }

        return true;
    }
}
 


