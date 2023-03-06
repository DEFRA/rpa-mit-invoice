using Invoices.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var storageConnection = builder.Configuration["Storage:ConnectionString"];

builder.Services.AddInvoiceServices(storageConnection);
builder.Services.SwaggerServices();

var app = builder.Build();

app.SwaggerEndpoints();
app.MapInvoiceEndpoints();

app.Run();
