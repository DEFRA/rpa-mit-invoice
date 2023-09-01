using System.Diagnostics.CodeAnalysis;
using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using EST.MIT.Invoice.Api.Services.Api;
using EST.MIT.Invoice.Api.Util;

namespace Invoices.Api.Endpoints;

[ExcludeFromCodeCoverage]
public static class ApiDefinition
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddSingleton<IReferenceDataApi, ReferenceDataApi>();
        services.AddSingleton<ICachedReferenceDataApi, CachedReferenceDataApi>();
        services.AddSingleton<IHttpContentDeserializer, HttpContentDeserializer>();
        services.AddSingleton<ICacheService, CacheService>();
        return services;
    }
}