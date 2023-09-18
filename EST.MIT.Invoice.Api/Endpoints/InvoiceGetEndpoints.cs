using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Invoices.Api.Models;
using Invoices.Api.Services;

namespace Invoices.Api.Endpoints;

public static class InvoiceGetEndpoints
{
    [ExcludeFromCodeCoverage]
    public static IEndpointRouteBuilder MapInvoiceGetEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/invoice/{scheme}/{invoiceId}", GetInvoice)
            .Produces<Invoice>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetInvoice");

        return app;
    }

    public static async Task<IResult> GetInvoice(string scheme, string invoiceId, IInvoiceService invoiceService)
    {
        var invoiceResponse = await invoiceService.GetBySchemeAndIdAsync(scheme, invoiceId);

        if (invoiceResponse is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(invoiceResponse.FirstOrDefault());
    }
}