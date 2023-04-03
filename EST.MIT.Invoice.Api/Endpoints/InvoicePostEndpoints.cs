using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using FluentValidation;
using Invoices.Api.Models;
using Invoices.Api.Services;

namespace Invoices.Api.Endpoints;

public static class InvoicePostEndpoints
{
    [ExcludeFromCodeCoverage]
    public static IEndpointRouteBuilder MapInvoicePostEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/invoice", CreateInvoice)
            .Produces<Invoice>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("CreateInvoice");

        app.MapPost("/invoices", CreateBulkInvoices)
            .Produces<BulkInvoices>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("CreateBulkInvoices");

        return app;
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
            return Results.BadRequest();
        }

        await eventQueueService.CreateMessage(invoice.Id, invoice.Status, "invoice-created", "Invoice created", invoice);

        return Results.Created($"/invoice/{invoice.SchemeType}/{invoice.Id}", invoice);
    }

    public static async Task<IResult> CreateBulkInvoices(BulkInvoices invoices, IValidator<BulkInvoices> validator, ICosmosService cosmosService, IEventQueueService eventQueueService)
    {
        var validationResult = await validator.ValidateAsync(invoices);
        var reference = invoices.Reference;

        if (!validationResult.IsValid)
        {
            await eventQueueService.CreateMessage(reference, "invalid", "bulk-invoice-validation-falied", "Bulk invoice validation failed");
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var bulkInvoiceCreated = await cosmosService.CreateBulk(invoices);

        if (bulkInvoiceCreated is null)
        {
            await eventQueueService.CreateMessage(reference, "failed", "bulk-invoice-creation-falied", "Bulk invoice creation failed");
            return Results.BadRequest();
        }

        return Results.Ok($"{invoices.Invoices.Count()} Bulk invoices created successfully");
    }
}