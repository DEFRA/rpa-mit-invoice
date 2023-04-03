using Invoices.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var storageConnection = builder.Configuration["Storage:ConnectionString"];
var eventQueueName = builder.Configuration["Storage:EventQueueName"];
var paymentQueueName = builder.Configuration["Storage:PaymentQueueName"];
var cosmosUrl = builder.Configuration["AzureCosmosDbSettings:Url"];
var cosmosPrimaryKey = builder.Configuration["AzureCosmosDbSettings:PrimaryKey"];
var cosmosDatabaseName = builder.Configuration["AzureCosmosDbSettings:DatabaseName"];
var cosmosContainerName = builder.Configuration["AzureCosmosDbSettings:ContainerName"];

builder.Services.AddCosmosServices(cosmosUrl, cosmosPrimaryKey, cosmosDatabaseName, cosmosContainerName);
builder.Services.AddQueueServices(storageConnection, eventQueueName, paymentQueueName);
builder.Services.AddInvoiceServices();
builder.Services.AddSwaggerServices();

var app = builder.Build();

app.SwaggerEndpoints();
app.MapInvoiceGetEndpoints();
app.MapInvoicePostEndpoints();
app.MapInvoicePutEndpoints();
app.MapInvoiceDeleteEndpoints();

app.Run();
