using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Invoices.Api.Services.PaymentsBatch;
using FluentValidation;
using Invoices.Api.Models;
using Invoices.Api.Services;

namespace Invoices.Api.Endpoints;

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

    public static async Task<IResult> UpdateInvoice(string invoiceId, PaymentRequestsBatch paymentRequestsBatch, IPaymentRequestsBatchService paymentRequestsBatchService, IQueueService queueService, IValidator<PaymentRequestsBatch> validator, IEventQueueService eventQueueService)
    {
        var validationResult = await validator.ValidateAsync(paymentRequestsBatch);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var invoiceUpdated = await paymentRequestsBatchService.UpdateAsync(paymentRequestsBatch);

        if (invoiceUpdated is null)
        {
            await eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "invoice-update-failed", "Invoice update failed", paymentRequestsBatch);
            return Results.BadRequest();
        }

        await eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "invoice-update", "Invoice updated", paymentRequestsBatch);

        if (paymentRequestsBatch.Status == InvoiceStatuses.Approved)
        {
            var message = JsonSerializer.Serialize(new InvoiceGenerator { Id = paymentRequestsBatch.Id, Scheme = paymentRequestsBatch.SchemeType });
            await queueService.CreateMessage(message);
            await eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "invoice-payment-request-sent", "Invoice payment request sent");
        }

        return Results.Ok(paymentRequestsBatch);
    }
}
