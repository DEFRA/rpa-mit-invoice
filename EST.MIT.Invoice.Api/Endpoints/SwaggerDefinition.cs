using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;

namespace Invoices.Api.Endpoints;

[ExcludeFromCodeCoverage]
public static class SwaggerDefinition
{
    public static void SwaggerEndpoints(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    public static void AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "ManualInvoiceTemplatesApi", Version = "v1", Description = "Invoice api for manual invoice templates" }));
    }
}