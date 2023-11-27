using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using EST.MIT.Invoice.Api.Services.PaymentsBatch;
using FluentValidation;
using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Services;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;

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
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        // TODO: 
        // get the logged in user
        // from the auth token, but for now mock it
        var loggedInUser = mockedDataService.GetLoggedInUser();

        var invoiceUpdated = await paymentRequestsBatchService.UpdateAsync(paymentRequestsBatch, loggedInUser);

        if (invoiceUpdated is null)
        {
            await eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "invoice-update-failed", "Invoice update failed", paymentRequestsBatch);
            return Results.BadRequest();
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
