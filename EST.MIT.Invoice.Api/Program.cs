using Invoices.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var storageConnection = builder.Configuration["Storage:ConnectionString"];
var queueName = builder.Configuration["Storage:QueueName"];

builder.Services.AddInvoiceServices(storageConnection, queueName);
builder.Services.SwaggerServices();

var app = builder.Build();

app.SwaggerEndpoints();
app.MapInvoiceEndpoints();

app.Run();
