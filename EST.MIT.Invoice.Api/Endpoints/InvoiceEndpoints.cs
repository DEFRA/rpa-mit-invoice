using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using FluentValidation;
using Invoices.Api.Models;
using Invoices.Api.Services;

namespace Invoices.Api.Endpoints;

public static class InvoiceEndpoints
{
    [ExcludeFromCodeCoverage]
    public static IEndpointRouteBuilder MapInvoiceEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/invoice/{scheme}/{invoiceId}", GetInvoice)
            .Produces<Invoice>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetInvoice");

        app.MapPost("/invoice", CreateInvoice)
            .Produces<Invoice>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("CreateInvoice");

        app.MapPut("/invoice/{invoiceId}", UpdateInvoice)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("UpdateInvoice");

        app.MapDelete("/invoice/{scheme}/{invoiceId}", DeleteInvoice)
            .Produces(StatusCodes.Status200OK)
            .WithName("DeleteInvoice");

        return app;
    }

    public static async Task<IResult> GetInvoice(string scheme, string invoiceId, ICosmosService cosmosService)
    {
        var invoiceResponse = await cosmosService.Get($"SELECT * FROM c WHERE c.schemeType = '{scheme}' AND c.id = '{invoiceId}'");

        if (invoiceResponse is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(invoiceResponse.FirstOrDefault());
    }

    public static async Task<IResult> CreateInvoice(Invoice invoice, IValidator<Invoice> validator, ICosmosService cosmosService, IEventQueueService eventQueueService)
    {
        var validationResult = await validator.ValidateAsync(invoice);

        if (!validationResult.IsValid)
        {
            await eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-validation-falied", "Invoice validation failed", invoice);
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var invoiceCreated = await cosmosService.Create(invoice);

        if (invoiceCreated is null)
        {
            await eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-create-falied", "Invoice creation failed", invoice);
            return Results.NotFound();
        }

        await eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-created", "Invoice created", invoice);

        return Results.Created($"/invoice/{invoice.SchemeType}/{invoice.Id}", invoice);
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

    public static async Task<IResult> DeleteInvoice(string id, string scheme, ICosmosService cosmosService, IEventQueueService eventQueueService)
    {
        await cosmosService.Delete(id, scheme);
        await eventQueueService.CreateMessage(id, "deleted", "invoice-deleted", "Invoice updated");
        return Results.Ok();
    }
}
