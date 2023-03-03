using System.Text.Json;
using Azure.Data.Tables;
using Invoices.Api.Models;
using Invoices.Api.Services;

namespace Invoices.Api.Endpoints;

public static class InvoiceEndpoints
{
    public static IEndpointRouteBuilder MapInvoiceEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/invoice/{scheme}/{invoiceId}", GetInvoice)
            .Produces<Invoice>(StatusCodes.Status200OK)
            .WithName("GetInvoice");

        app.MapPost("/invoice", CreateInvoice).WithName("CreateInvoice");

        app.MapPut("/invoice", UpdateInvoice).WithName("UpdateInvoice");

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

    public static async Task<IResult> CreateInvoice(Invoice invoice, ITableService tableService)
    {
        var invoiceCreated = await tableService.CreateInvoice(invoice);

        if (!invoiceCreated)
        {
            return Results.BadRequest();
        }

        return Results.Created($"/invoice/{invoice.Id}", null);
    }

    public static async Task<IResult> UpdateInvoice(Invoice invoice, ITableService tableService)
    {
        var invoiceUpdated = await tableService.UpdateInvoice(invoice);

        if (!invoiceUpdated)
        {
            return Results.BadRequest();
        }

        return Results.NoContent();
    }

    public static IServiceCollection AddInvoiceServices(this IServiceCollection services, string storageConnection)
    {
        services.AddSingleton(_ => new TableServiceClient(storageConnection));
        services.AddScoped<ITableService, TableService>();
        return services;
    }
}
