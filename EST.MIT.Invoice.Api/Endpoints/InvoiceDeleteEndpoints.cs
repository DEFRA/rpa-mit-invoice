using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using FluentValidation;
using Invoices.Api.Models;
using Invoices.Api.Services;

namespace Invoices.Api.Endpoints;

public static class InvoiceDeleteEndpoints
{
    [ExcludeFromCodeCoverage]
    public static IEndpointRouteBuilder MapInvoiceDeleteEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/invoice/{scheme}/{invoiceId}", DeleteInvoice)
            .Produces(StatusCodes.Status200OK)
            .WithName("DeleteInvoice");

        return app;
    }

    public static async Task<IResult> DeleteInvoice(string id, string scheme, ICosmosService cosmosService, IEventQueueService eventQueueService)
    {
        await cosmosService.Delete(id, scheme);
        await eventQueueService.CreateMessage(id, "deleted", "invoice-deleted", "Invoice updated");
        return Results.Ok();
    }
}
