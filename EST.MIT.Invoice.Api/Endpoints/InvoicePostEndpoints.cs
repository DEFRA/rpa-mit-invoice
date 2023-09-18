using System.Diagnostics.CodeAnalysis;
using Invoices.Api.Services.PaymentsBatch;
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
            .Produces<PaymentRequestsBatch>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("CreateInvoice");

        app.MapPost("/invoices", CreateBulkInvoices)
            .Produces<BulkInvoices>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("CreateBulkInvoices");

        return app;
    }

    public static async Task<IResult> CreateInvoice(PaymentRequestsBatch paymentRequestsBatch, IValidator<PaymentRequestsBatch> validator, IPaymentRequestsBatchService paymentRequestsBatchService, IEventQueueService eventQueueService)
    {
        var validationResult = await validator.ValidateAsync(paymentRequestsBatch);

        if (!validationResult.IsValid)
        {
            await eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "invoice-validation-failed", "Invoice validation failed", paymentRequestsBatch);
            return Results.BadRequest(new HttpValidationProblemDetails(validationResult.ToDictionary()));
        }

        var invoiceCreated = await paymentRequestsBatchService.CreateAsync(paymentRequestsBatch);

        if (invoiceCreated is null)
        {
            await eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "invoice-create-failed", "Invoice creation failed", paymentRequestsBatch);
            return Results.BadRequest();
        }

        await eventQueueService.CreateMessage(paymentRequestsBatch.Id, paymentRequestsBatch.Status, "invoice-created", "Invoice created", paymentRequestsBatch);

        return Results.Created($"/invoice/{paymentRequestsBatch.SchemeType}/{paymentRequestsBatch.Id}", paymentRequestsBatch);
    }

    public static async Task<IResult> CreateBulkInvoices(BulkInvoices invoices, IValidator<BulkInvoices> validator, IPaymentRequestsBatchService paymentRequestsBatchService, IEventQueueService eventQueueService)
    {
        var validationResult = await validator.ValidateAsync(invoices);
        var reference = invoices.Reference;

        if (!validationResult.IsValid)
        {
            await eventQueueService.CreateMessage(reference, "invalid", "bulk-invoice-validation-failed", "Bulk Invoice validation failed");
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var bulkInvoiceCreated = await paymentRequestsBatchService.CreateBulkAsync(invoices);

        if (bulkInvoiceCreated is null)
        {
            await eventQueueService.CreateMessage(reference, "failed", "bulk-invoice-creation-failed", "Bulk Invoice creation failed");
            return Results.BadRequest();
        }

        return Results.Ok($"{invoices.Invoices.Count()} Bulk invoices created successfully");
    }
}