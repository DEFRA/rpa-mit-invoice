using Invoices.Api.Endpoints;
using Microsoft.Extensions.Azure;

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
builder.Services.AddApiServices();
builder.Services.AddRepositoryServices();
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["storageconnectionstring:blob"], preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["storageconnectionstring:queue"], preferMsi: true);
});

builder.Services.AddHttpClient("ReferenceDataApi", clientBuilder =>
{
    clientBuilder.BaseAddress = new Uri(builder.Configuration["ApiEndpoints:ReferenceDataApiBaseUri"]);
});

var app = builder.Build();

app.SwaggerEndpoints();
app.MapInvoiceGetEndpoints();
app.MapInvoicePostEndpoints();
app.MapInvoicePutEndpoints();
app.MapInvoiceDeleteEndpoints();

app.Run();
