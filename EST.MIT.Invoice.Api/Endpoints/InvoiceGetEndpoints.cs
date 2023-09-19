using System.Diagnostics.CodeAnalysis;
using EST.MIT.Invoice.Api.Services.PaymentsBatch;
using FluentValidation;
using EST.MIT.Invoice.Api.Models;

namespace EST.MIT.Invoice.Api.Endpoints;

public static class InvoiceGetEndpoints
{
    [ExcludeFromCodeCoverage]
    public static IEndpointRouteBuilder MapInvoiceGetEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/invoice/{scheme}/{invoiceId}", GetInvoice)
            .Produces<PaymentRequestsBatch>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetInvoice");

        return app;
    }

    public static async Task<IResult> GetInvoice(string scheme, string invoiceId, IPaymentRequestsBatchService paymentRequestsBatchService)
    {
        var invoiceResponse = await paymentRequestsBatchService.GetBySchemeAndIdAsync(scheme, invoiceId);

        if (invoiceResponse is null || invoiceResponse.Count == 0)
        {
            return Results.NotFound();
        }

        return Results.Ok(invoiceResponse.FirstOrDefault());
    }
}