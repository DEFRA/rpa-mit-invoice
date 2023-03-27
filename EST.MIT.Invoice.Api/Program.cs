using Invoices.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var storageConnection = builder.Configuration["Storage:ConnectionString"];
var queueName = builder.Configuration["Storage:QueueName"];
var cosmosUrl = builder.Configuration["AzureCosmosDbSettings:Url"];
var cosmosPrimaryKey = builder.Configuration["AzureCosmosDbSettings:PrimaryKey"];
var cosmosDatabaseName = builder.Configuration["AzureCosmosDbSettings:DatabaseName"];
var cosmosContainerName = builder.Configuration["AzureCosmosDbSettings:ContainerName"];

builder.Services.AddCosmosServices(cosmosUrl, cosmosPrimaryKey, cosmosDatabaseName, cosmosContainerName);
builder.Services.AddInvoiceServices(storageConnection, queueName);
builder.Services.AddSwaggerServices();

var app = builder.Build();

app.SwaggerEndpoints();
app.MapInvoiceEndpoints();

app.Run();
