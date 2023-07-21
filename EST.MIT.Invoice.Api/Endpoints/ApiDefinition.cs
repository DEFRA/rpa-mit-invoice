using System.Diagnostics.CodeAnalysis;
using EST.MIT.Invoice.Api.Services.API.Interfaces;
using EST.MIT.Invoice.Api.Services.Api;

namespace Invoices.Api.Endpoints;

[ExcludeFromCodeCoverage]
public static class ApiDefinition
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddSingleton<IReferenceDataApi, ReferenceDataApi>();
        return services;
    }
}