using System.Diagnostics.CodeAnalysis;
using EST.MIT.Invoice.Api.Repositories;
using EST.MIT.Invoice.Api.Repositories.Interfaces;
namespace Invoices.Api.Endpoints;

[ExcludeFromCodeCoverage]
public static class RepositoryDefinition
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddSingleton<IReferenceDataRepository, ReferenceDataRepository>();
        return services;
    }
}