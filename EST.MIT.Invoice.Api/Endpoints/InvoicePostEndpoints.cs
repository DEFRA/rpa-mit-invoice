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
        app.MapPost("/paymentRequestsBatch", CreateInvoice)
            .Produces<PaymentRequestsBatch>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("CreateInvoice");

        app.MapPost("/invoices", CreateBulkInvoices)
            .Produces<BulkInvoices>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("CreateBulkInvoices");

        return app;
    }

    public static async Task<IResult> CreateInvoice(PaymentRequestsBatch paymentRequestsBatch, IValidator<PaymentRequestsBatch> validator, ICosmosService cosmosService, IEventQueueService eventQueueService)
    {
        var validationResult = await validator.ValidateAsync(paymentRequestsBatch);

        if (!validationResult.IsValid)
        {
            await eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "paymentRequestsBatch-validation-failed", "PaymentRequestsBatch validation failed", paymentRequestsBatch);
            return Results.BadRequest(new HttpValidationProblemDetails(validationResult.ToDictionary()));
        }

        var invoiceCreated = await cosmosService.Create(paymentRequestsBatch);

        if (invoiceCreated is null)
        {
            await eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "paymentRequestsBatch-create-falied", "PaymentRequestsBatch creation failed", paymentRequestsBatch);
            return Results.BadRequest();
        }

        await eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "paymentRequestsBatch-created", "PaymentRequestsBatch created", paymentRequestsBatch);

        return Results.Created($"/paymentRequestsBatch/{paymentRequestsBatch.SchemeType}/{paymentRequestsBatch.Id}", paymentRequestsBatch);
    }

    public static async Task<IResult> CreateBulkInvoices(BulkInvoices invoices, IValidator<BulkInvoices> validator, ICosmosService cosmosService, IEventQueueService eventQueueService)
    {
        var validationResult = await validator.ValidateAsync(invoices);
        var reference = invoices.Reference;

        if (!validationResult.IsValid)
        {
            await eventQueueService.CreateMessage(reference, "invalid", "bulk-paymentRequestsBatch-validation-falied", "Bulk paymentRequestsBatch validation failed");
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var bulkInvoiceCreated = await cosmosService.CreateBulk(invoices);

        if (bulkInvoiceCreated is null)
        {
            await eventQueueService.CreateMessage(reference, "failed", "bulk-paymentRequestsBatch-creation-falied", "Bulk paymentRequestsBatch creation failed");
            return Results.BadRequest();
        }

        return Results.Ok($"{invoices.Invoices.Count()} Bulk invoices created successfully");
    }
}