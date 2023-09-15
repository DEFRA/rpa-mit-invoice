using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using FluentValidation;
using Invoices.Api.Models;
using Invoices.Api.Services;

namespace Invoices.Api.Endpoints;

public static class InvoicePutEndpoints
{
    [ExcludeFromCodeCoverage]
    public static IEndpointRouteBuilder MapInvoicePutEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPut("/paymentRequestsBatch/{invoiceId}", UpdateInvoice)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("UpdateInvoice");

        return app;
    }

    public static async Task<IResult> UpdateInvoice(string invoiceId, PaymentRequestsBatch paymentRequestsBatch, ICosmosService cosmosService, IQueueService queueService, IValidator<PaymentRequestsBatch> validator, IEventQueueService eventQueueService)
    {
        var validationResult = await validator.ValidateAsync(paymentRequestsBatch);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var invoiceUpdated = await cosmosService.Update(paymentRequestsBatch);

        if (invoiceUpdated is null)
        {
            await eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "paymentRequestsBatch-update-falied", "PaymentRequestsBatch update failed", paymentRequestsBatch);
            return Results.BadRequest();
        }

        await eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "paymentRequestsBatch-update", "PaymentRequestsBatch updated", paymentRequestsBatch);

        if (paymentRequestsBatch.Status == InvoiceStatuses.Approved)
        {
            var message = JsonSerializer.Serialize(new InvoiceGenerator { Id = paymentRequestsBatch.Id, Scheme = paymentRequestsBatch.SchemeType });
            await queueService.CreateMessage(message);
            await eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "paymentRequestsBatch-payment-request-sent", "PaymentRequestsBatch payment request sent");
        }

        return Results.Ok(paymentRequestsBatch);
    }
}
