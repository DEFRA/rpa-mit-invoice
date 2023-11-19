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

        app.MapGet("/invoice/approvals/{invoiceId}", GetApprovalById)
            .Produces<PaymentRequestsBatch>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetApprovalById");

        app.MapGet("/invoice/approvals", GetAllApprovals)
            .Produces<PaymentRequestsBatch>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetAllApprovals");

        app.MapGet("/invoices/user/{userId}", GetInvoicesById)
            .Produces<PaymentRequestsBatch>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetInvoicesById");

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

    public static async Task<IResult> GetAllApprovals(IPaymentRequestsBatchApprovalService paymentRequestsBatchApprovalService)
    {
        var userId = "1"; // TODO: fxs need to get the user id from the token
        var invoiceResponse = await paymentRequestsBatchApprovalService.GetAllInvoicesForApprovalByUserIdAsync(userId);

        if (invoiceResponse is null || invoiceResponse.Count == 0)
        {
            return Results.NotFound();
        }

        return Results.Ok(invoiceResponse);
    }

    public static async Task<IResult> GetApprovalById(string invoiceId, IPaymentRequestsBatchApprovalService paymentRequestsBatchApprovalService)
    {
        var userId = "1"; // TODO: fxs need to get the user id from the token
        var invoiceResponse = await paymentRequestsBatchApprovalService.GetInvoiceForApprovalByUserIdAndInvoiceIdAsync(userId, invoiceId);

        if (invoiceResponse is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(invoiceResponse);
    }

    public static async Task<IResult> GetInvoicesById(IPaymentRequestsBatchApprovalService paymentRequestsBatchApprovalService)
    {
        var userId = "1"; // TODO: fxs need to get the user id from the token
        var invoiceResponse = await paymentRequestsBatchApprovalService.GetInvoicesByUserIdAsync(userId);

        if (invoiceResponse is null || invoiceResponse.Count == 0)
        {
            return Results.NotFound();
        }

        return Results.Ok(invoiceResponse);
    }
}