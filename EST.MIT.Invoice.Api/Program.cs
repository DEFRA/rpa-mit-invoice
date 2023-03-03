using Azure.Data.Tables;
using EST.MIT.Invoice.Api.Endpoints;
using Invoices.Api.Endpoints;
using Invoices.Api.Services;

var builder = WebApplication.CreateBuilder(args);
var storageConnection = builder.Configuration["Storage:ConnectionString"];

builder.Services.AddInvoiceServices(storageConnection);
builder.Services.SwaggerServices();

var app = builder.Build();

app.SwaggerEndpoints();
app.MapInvoiceEndpoints();

app.Run();
