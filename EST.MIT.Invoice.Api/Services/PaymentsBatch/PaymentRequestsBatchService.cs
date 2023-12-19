using EST.MIT.Invoice.Api.Exceptions;
using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Repositories.Entities;
using EST.MIT.Invoice.Api.Repositories.Interfaces;
using EST.MIT.Invoice.Api.Util;

namespace EST.MIT.Invoice.Api.Services.PaymentsBatch;

public class PaymentRequestsBatchService : IPaymentRequestsBatchService
{
    private readonly IPaymentRequestsBatchRepository _paymentRequestsBatchRepository;

    public PaymentRequestsBatchService(IPaymentRequestsBatchRepository paymentRequestsBatchRepository)
    {
        _paymentRequestsBatchRepository = paymentRequestsBatchRepository;
    }

    public async Task<List<PaymentRequestsBatch>> GetByIdAsync(string id)
    {
        var result = await _paymentRequestsBatchRepository.GetByIdAsync(id);
        return InvoiceMapper.MapToInvoice(result);
    }

    public async Task<List<PaymentRequestsBatch>> GetByPaymentRequestIdAsync(string paymentRequestId)
    {
        var result = await _paymentRequestsBatchRepository.GetByPaymentRequestIdAsync(paymentRequestId);
        return InvoiceMapper.MapToInvoice(result);
    }

    public async Task<List<PaymentRequestsBatch>> GetBySchemeAndIdAsync(string scheme, string id)
    {
        var result = await _paymentRequestsBatchRepository.GetBySchemeAndIdAsync(scheme, id);
        return InvoiceMapper.MapToInvoice(result);
    }

    public async Task<List<PaymentRequestsBatch>> GetInvoicesByUserIdAsync(string userId)
    {
	    var result = await _paymentRequestsBatchRepository.GetInvoicesByUserIdAsync(userId);
        try
        {
            List<PaymentRequestsBatch> resultList = InvoiceMapper.MapToInvoice(result);
            return resultList;
        } catch (Exception ex)
        {
            return new List<PaymentRequestsBatch>();
        }
    }

	public async Task<PaymentRequestsBatch> CreateAsync(PaymentRequestsBatch invoice, LoggedInUser loggedInUser)
	{
		try
		{
			invoice.CreatedBy = loggedInUser.UserId;
			var invoiceEntity = InvoiceMapper.MapToInvoiceEntity(invoice);
			await _paymentRequestsBatchRepository.CreateAsync(invoiceEntity);
			return invoice;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

    public async Task<BulkInvoices?> CreateBulkAsync(BulkInvoices invoices, LoggedInUser loggedInUser)
    {
	    foreach (var invoice in invoices.Invoices)
	    {
		    invoice.CreatedBy = loggedInUser.UserId;
	    }

        var entity = new BulkInvoicesEntity
        {
            SchemeType = invoices.SchemeType,
            Reference = invoices.Reference,
            Invoices = InvoiceMapper.BulkMapToInvoiceEntity(invoices.Invoices)
        };

        await _paymentRequestsBatchRepository.CreateBulkAsync(entity);

        return invoices;
    }

    public async Task<PaymentRequestsBatch> UpdateAsync(PaymentRequestsBatch invoice, LoggedInUser loggedInUser)
    {
        // first get the existing entity
        var existingEntity = (await _paymentRequestsBatchRepository.GetBySchemeAndIdAsync(invoice.SchemeType, invoice.Id)).FirstOrDefault();

        if (existingEntity == null)
        {
            throw new InvoiceNotFoundException();
        }

        if (ExistingInvoicePendingApprovalIsNotBeingApprovedOrRejected(existingEntity, invoice))
        {
            throw new AwaitingApprovalInvoiceCannotBeUpdatedException();
        }

        if (ExistingInvoiceIsApprovedOrRejected(existingEntity))
        {
            throw new ApprovedOrRejectedInvoiceCannotBeUpdatedException();
        }

        if (InvoiceIsBeingApprovedOrRejected(existingEntity, invoice))
        {
            invoice.Approved = DateTime.Now;
            invoice.ApprovedBy = loggedInUser.UserId; // not sure what we should be storing here, the id the email or something else
        }

        invoice.UpdatedBy = loggedInUser.UserId;

        var updatedEntity = InvoiceMapper.MapToInvoiceEntity(invoice);      // Would theoretically allow data changes at same time as approved/rejected - Fix in User Story 280536
        await _paymentRequestsBatchRepository.UpdateAsync(updatedEntity);
        return invoice;
    }

    private bool ExistingInvoicePendingApprovalIsNotBeingApprovedOrRejected(InvoiceEntity existingInvoice, PaymentRequestsBatch newInvoice)
    {
        return (existingInvoice.Status.ToLower() == InvoiceStatuses.AwaitingApproval.ToLower())
            && newInvoice.Status.ToLower() != InvoiceStatuses.Approved.ToLower() && newInvoice.Status.ToLower() != InvoiceStatuses.Rejected.ToLower();
    }

    private bool ExistingInvoiceIsApprovedOrRejected(InvoiceEntity existingInvoice)
    {
        return existingInvoice.Status.ToLower() == InvoiceStatuses.Approved.ToLower() || existingInvoice.Status.ToLower() == InvoiceStatuses.Rejected.ToLower();
    }

    private bool InvoiceIsBeingApprovedOrRejected(InvoiceEntity existingInvoice, PaymentRequestsBatch newInvoice)
    {
        return existingInvoice.Status.ToLower() == InvoiceStatuses.AwaitingApproval.ToLower()
            && (newInvoice.Status.ToLower() == InvoiceStatuses.Approved.ToLower() || newInvoice.Status.ToLower() == InvoiceStatuses.Rejected.ToLower());
    }

    public async Task<string> DeleteBySchemeAndIdAsync(string schemeType, string id)
    {
        await _paymentRequestsBatchRepository.DeleteBySchemeAndIdAsync(schemeType, id);
        return id;
    }
}