using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using FluentValidation;
using Invoices.Api.Models;
using Invoices.Api.Services;

namespace Invoices.Api.Endpoints;

[ExcludeFromCodeCoverage]
public static class InvoiceDefinition
{
    public static IServiceCollection AddInvoiceServices(this IServiceCollection services, string storageConnection, string queueName)
    {
        services.AddScoped<IValidator<Invoice>, InvoiceValidator>();
        services.AddSingleton(_ => new QueueClient(storageConnection, queueName));
        services.AddScoped<IQueueService, QueueService>();
        return services;
    }
}