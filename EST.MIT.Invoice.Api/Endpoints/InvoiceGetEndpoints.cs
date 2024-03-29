using System.Diagnostics.CodeAnalysis;
using EST.MIT.Invoice.Api.Services.PaymentsBatch;
using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;

namespace EST.MIT.Invoice.Api.Endpoints;

public static class InvoiceGetEndpoints
{
    [ExcludeFromCodeCoverage]
    public static IEndpointRouteBuilder MapInvoiceGetEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/invoice/{scheme}/{invoiceId}", GetInvoiceBySchemeAndId)
            .Produces<PaymentRequestsBatch>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetInvoiceBySchemeAndId");

        app.MapGet("/invoice/{invoiceId}", GetInvoiceById)
            .Produces<PaymentRequestsBatch>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetInvoiceById");

        app.MapGet("/invoice/paymentrequest/{paymentRequestId}", GetInvoiceByPaymentRequestId)
            .Produces<PaymentRequestsBatch>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetInvoiceByPaymentRequestId");

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

    public static async Task<IResult> GetInvoiceById(string invoiceId, IPaymentRequestsBatchService paymentRequestsBatchService)
    {
        var invoiceResponse = await paymentRequestsBatchService.GetByIdAsync(invoiceId);

        if (invoiceResponse is null || invoiceResponse.Count == 0)
        {
            return Results.NotFound();
        }

        return Results.Ok(invoiceResponse.FirstOrDefault());
    }

    public static async Task<IResult> GetInvoiceByPaymentRequestId(string paymentRequestId, IPaymentRequestsBatchService paymentRequestsBatchService)
    {
        var invoiceResponse = await paymentRequestsBatchService.GetByPaymentRequestIdAsync(paymentRequestId);

        if (invoiceResponse is null || invoiceResponse.Count == 0)
        {
            return Results.NotFound();
        }

        return Results.Ok(invoiceResponse.FirstOrDefault());
    }

    public static async Task<IResult> GetInvoiceBySchemeAndId(string scheme, string invoiceId, IPaymentRequestsBatchService paymentRequestsBatchService)
    {
        var invoiceResponse = await paymentRequestsBatchService.GetBySchemeAndIdAsync(scheme, invoiceId);

        if (invoiceResponse is null || invoiceResponse.Count == 0)
        {
            return Results.NotFound();
        }

        return Results.Ok(invoiceResponse.FirstOrDefault());
    }

    public static async Task<IResult> GetAllApprovals(IPaymentRequestsBatchApprovalService paymentRequestsBatchApprovalService, IMockedDataService mockedDataService)
    {
	    // TODO: 
	    // get the logged in user
	    // from the auth token, but for now mock it
	    var loggedInUser = mockedDataService.GetLoggedInUser();

		var invoiceResponse = await paymentRequestsBatchApprovalService.GetAllInvoicesForApprovalByUserIdAsync(loggedInUser.UserId);

        if (invoiceResponse is null || invoiceResponse.Count == 0)
        {
            return Results.NotFound();
        }

        return Results.Ok(invoiceResponse);
    }

    public static async Task<IResult> GetApprovalById(string invoiceId, IPaymentRequestsBatchApprovalService paymentRequestsBatchApprovalService, IMockedDataService mockedDataService)
    {
		// TODO: 
		// get the logged in user
		// from the auth token, but for now mock it
		var loggedInUser = mockedDataService.GetLoggedInUser();
		var invoiceResponse = await paymentRequestsBatchApprovalService.GetInvoiceForApprovalByUserIdAndInvoiceIdAsync(loggedInUser.UserId, invoiceId);

        if (invoiceResponse is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(invoiceResponse);
    }

    public static async Task<IResult> GetInvoicesById(IPaymentRequestsBatchService paymentRequestsBatchService, IMockedDataService mockedDataService)
    {
		// TODO: 
		// get the logged in user
		// from the auth token, but for now mock it
		var loggedInUser = mockedDataService.GetLoggedInUser();
		var invoiceResponse = await paymentRequestsBatchService.GetInvoicesByUserIdAsync(loggedInUser.UserId);

        if (invoiceResponse is null || invoiceResponse.Count == 0)
        {
            return Results.NotFound();
        }

        return Results.Ok(invoiceResponse);
    }
}