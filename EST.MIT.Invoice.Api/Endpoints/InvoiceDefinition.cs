using System.Diagnostics.CodeAnalysis;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using EST.MIT.Invoice.Api.Services.Api.Models;
using FluentValidation;
using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Services;
using EST.MIT.Invoice.Api.Services.PaymentsBatch;

namespace EST.MIT.Invoice.Api.Endpoints;

[ExcludeFromCodeCoverage]
public static class InvoiceDefinition
{
    public static IServiceCollection AddInvoiceServices(this IServiceCollection services)
    {
        services.AddScoped<IValidator<PaymentRequestsBatch>, PaymentRequestsBatchValidator>();

        services.AddScoped<IValidator<BulkInvoices>, BulkInvoiceValidator>();

        services.AddScoped<IValidator<PaymentRequest>, PaymentRequestValidator>(
            serviceProvider => new PaymentRequestValidator(
                referenceDataApi: serviceProvider.GetRequiredService<IReferenceDataApi>(),
                cachedReferenceDataApi: serviceProvider.GetRequiredService<ICachedReferenceDataApi>(),
                new FieldsRoute(), "status")
        );

        services.AddScoped<IValidator<InvoiceLine>, InvoiceLineValidator>(
            serviceProvider => new InvoiceLineValidator(
                referenceDataApi: serviceProvider.GetRequiredService<IReferenceDataApi>(),
                new FieldsRoute(),
                cachedReferenceDataApi: serviceProvider.GetRequiredService<ICachedReferenceDataApi>())
        );

        services.AddScoped<IPaymentRequestsBatchService, PaymentRequestsBatchService>();

        return services;
    }
}