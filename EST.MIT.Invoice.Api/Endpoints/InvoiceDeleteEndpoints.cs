using System.Diagnostics.CodeAnalysis;
using EST.MIT.Invoice.Api.Services.PaymentsBatch;
using FluentValidation;
using EST.MIT.Invoice.Api.Services;

namespace EST.MIT.Invoice.Api.Endpoints;

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

    public static async Task<IResult> DeleteInvoice(string invoiceId, string scheme, IPaymentRequestsBatchService paymentRequestsBatchService, IEventQueueService eventQueueService)
    {
        await paymentRequestsBatchService.DeleteBySchemeAndIdAsync(scheme, invoiceId);
        await eventQueueService.CreateMessage(invoiceId, "deleted", "invoice-deleted", "Invoice updated");
        return Results.Ok();
    }
}
