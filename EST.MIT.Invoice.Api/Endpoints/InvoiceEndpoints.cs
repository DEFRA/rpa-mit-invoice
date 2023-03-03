using System.Text.Json;
using Azure.Data.Tables;
using FluentValidation;
using Invoices.Api.Models;
using Invoices.Api.Services;

namespace Invoices.Api.Endpoints;

public static class InvoiceEndpoints
{
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

        app.MapPut("/invoice", UpdateInvoice)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("UpdateInvoice");

        return app;
    }

    public static async Task<IResult> GetInvoice(string scheme, string invoiceId, ITableService tableService)
    {
        var invoiceResponse = await tableService.GetInvoice(scheme, invoiceId);

        if (invoiceResponse is null)
        {
            return Results.NotFound();
        }

        var invoice = JsonSerializer.Deserialize<Invoice>(invoiceResponse.Data);
        return Results.Ok(invoice);
    }

    public static async Task<IResult> CreateInvoice(Invoice invoice, ITableService tableService, IValidator<Invoice> validator)
    {
        var validationResult = await validator.ValidateAsync(invoice);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var invoiceCreated = await tableService.CreateInvoice(invoice);

        if (!invoiceCreated)
        {
            return Results.BadRequest();
        }

        return Results.Created($"/invoice/{invoice.Scheme}/{invoice.Id}", invoice);
    }

    public static async Task<IResult> UpdateInvoice(Invoice invoice, ITableService tableService, IValidator<Invoice> validator)
    {
        var validationResult = await validator.ValidateAsync(invoice);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var invoiceUpdated = await tableService.UpdateInvoice(invoice);

        if (!invoiceUpdated)
        {
            return Results.BadRequest();
        }

        return Results.NoContent();
    }

    public static IServiceCollection AddInvoiceServices(this IServiceCollection services, string storageConnection)
    {
        services.AddScoped<IValidator<Invoice>, InvoiceValidator>();
        services.AddSingleton(_ => new TableServiceClient(storageConnection));
        services.AddScoped<ITableService, TableService>();
        return services;
    }
}
