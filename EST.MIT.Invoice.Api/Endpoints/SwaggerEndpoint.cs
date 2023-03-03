using Microsoft.OpenApi.Models;

namespace EST.MIT.Invoice.Api.Endpoints;

public static class SwaggerEndpointDefinition
{
    public static void SwaggerEndpoints(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    public static void SwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "ManualInvoiceTemplatesApi", Version = "v1", Description = "Invoice api for manual invoice templates" }));
    }
}