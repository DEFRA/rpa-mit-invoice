using System.Diagnostics.CodeAnalysis;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using EST.MIT.Invoice.Api.Services.Api.Models;
using FluentValidation;
using Invoices.Api.Models;

namespace Invoices.Api.Endpoints;

[ExcludeFromCodeCoverage]
public static class InvoiceDefinition
{
    public static IServiceCollection AddInvoiceServices(this IServiceCollection services)
    {
        services.AddScoped<IValidator<PaymentRequestsBatch>, PaymentRequestsBatchValidator>();
        services.AddScoped<IValidator<BulkInvoices>, BulkInvoiceValidator>();

        services.AddScoped<IValidator<InvoiceHeader>, InvoiceHeaderValidator>(
            serviceProvider => new InvoiceHeaderValidator(
                referenceDataApi: serviceProvider.GetRequiredService<IReferenceDataApi>(),
                cachedReferenceDataApi: serviceProvider.GetRequiredService<ICachedReferenceDataApi>(),
                new FieldsRoute())
        );

        services.AddScoped<IValidator<InvoiceLine>, InvoiceLineValidator>(
            serviceProvider => new InvoiceLineValidator(
                referenceDataApi: serviceProvider.GetRequiredService<IReferenceDataApi>(),
                new FieldsRoute(),
                cachedReferenceDataApi: serviceProvider.GetRequiredService<ICachedReferenceDataApi>())
        );
        return services;
    }
}