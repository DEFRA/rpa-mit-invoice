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
        app.MapPut("/invoice/{invoiceId}", UpdateInvoice)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("UpdateInvoice");

        return app;
    }

    public static async Task<IResult> UpdateInvoice(string invoiceId, Invoice invoice, ICosmosService cosmosService, IQueueService queueService, IValidator<Invoice> validator, IEventQueueService eventQueueService)
    {
        var validationResult = await validator.ValidateAsync(invoice);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var invoiceUpdated = await cosmosService.Update(invoice);

        if (invoiceUpdated is null)
        {
            await eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-update-falied", "Invoice update failed", invoice);
            return Results.BadRequest();
        }

        await eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-update", "Invoice updated", invoice);

        if (invoice.Status == InvoiceStatuses.Approved)
        {
            var message = JsonSerializer.Serialize(new InvoiceGenerator { Id = invoice.Id, Scheme = invoice.SchemeType });
            await queueService.CreateMessage(message);
            await eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-payment-request-sent", "Invoice payment request sent");
        }

        return Results.Ok(invoice);
    }
}
