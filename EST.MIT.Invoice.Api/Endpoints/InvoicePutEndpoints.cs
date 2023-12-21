using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using EST.MIT.Invoice.Api.Services.PaymentsBatch;
using FluentValidation;
using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Services;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using EST.MIT.Invoice.Api.Exceptions;

namespace EST.MIT.Invoice.Api.Endpoints;

public static class InvoicePutEndpoints
{
    [ExcludeFromCodeCoverage]
    public static IEndpointRouteBuilder MapInvoicePutEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPut("/invoice/{invoiceId}", UpdateInvoice)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("UpdateInvoice");

        return app;
    }

    public static async Task<IResult> UpdateInvoice(string invoiceId, PaymentRequestsBatch paymentRequestsBatch, IPaymentRequestsBatchService paymentRequestsBatchService, IPaymentQueueService paymentQueueService, IValidator<PaymentRequestsBatch> validator, IEventQueueService eventQueueService, IMockedDataService mockedDataService)
    {
        var validationResult = await validator.ValidateAsync(paymentRequestsBatch);

        if (!validationResult.IsValid)
        {
            await eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "invoice-validation-failed", "Invoice validation failed", paymentRequestsBatch);
            return Results.BadRequest(new HttpValidationProblemDetails(validationResult.ToDictionary()));
        }

        // TODO: 
        // get the logged in user
        // from the auth token, but for now mock it
        var loggedInUser = mockedDataService.GetLoggedInUser();

        try
        {
            var invoiceUpdated = await paymentRequestsBatchService.UpdateAsync(paymentRequestsBatch, loggedInUser);
            if (invoiceUpdated is null)
            {
                await eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "invoice-update-failed", "Invoice update failed", paymentRequestsBatch);
                var unknownError = new Dictionary<string, string[]>
                {
                    { "", new string[] { "Invoice could not be saved" } }
                };
                return Results.BadRequest(new HttpValidationProblemDetails(unknownError));
            }
        }
        catch (InvoiceNotFoundException)
        {
            return Results.NotFound();
        }
        catch (AwaitingApprovalInvoiceCannotBeUpdatedException)
        {
            var cannotBeUpdatedError = new Dictionary<string, string[]>
            {
                { "", new string[] { "Invoices waiting approval cannot be updated" } }
            };
            return Results.BadRequest(new HttpValidationProblemDetails(cannotBeUpdatedError));
        }
        catch (ApprovedOrRejectedInvoiceCannotBeUpdatedException)
        {
            var cannotBeUpdatedError = new Dictionary<string, string[]>
            {
                { "", new string[] { "Approved or Rejected invoices cannot be updated" } }
            };
            return Results.BadRequest(new HttpValidationProblemDetails(cannotBeUpdatedError));
        }
        catch (Exception ex) {
            var unknownError = new Dictionary<string, string[]>
            {
                { "", new string[] { $"Error whilst saving invoice: {ex.Message}" } }
            };
            return Results.BadRequest(new HttpValidationProblemDetails(unknownError));
        }

        await eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "invoice-update", "Invoice updated", paymentRequestsBatch);

        if (paymentRequestsBatch.Status == InvoiceStatuses.Approved)
        {
            var message = JsonSerializer.Serialize(new InvoiceGenerator { Id = paymentRequestsBatch.Id, Scheme = paymentRequestsBatch.SchemeType });
            await paymentQueueService.CreateMessage(message);
            await eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "invoice-payment-request-sent", "Invoice payment request sent");
        }

        return Results.Ok(paymentRequestsBatch);
    }
}
