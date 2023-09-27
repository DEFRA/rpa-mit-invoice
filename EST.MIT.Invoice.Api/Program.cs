using Azure.Core.Diagnostics;
using Azure.Identity;
using EST.MIT.Invoice.Api.Endpoints;
using EST.MIT.Invoice.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);
var storageConnection = builder.Configuration["Storage:ConnectionString"];
var eventQueueName = builder.Configuration["Storage:EventQueueName"];
var paymentQueueName = builder.Configuration["Storage:PaymentQueueName"];

// Postgres 
var host = builder.Configuration["POSTGRES_HOST"];
var port = builder.Configuration["POSTGRES_PORT"];
var db = builder.Configuration["POSTGRES_DB"];
var user = builder.Configuration["POSTGRES_USER"];
var pass = builder.Configuration["POSTGRES_PASSWORD"];
var postgresSqlAAD = builder.Configuration["AzureADPostgreSQLResourceID"];

if (builder.Environment.IsProduction())
{
				using AzureEventSourceListener listener = AzureEventSourceListener.CreateConsoleLogger();

				var options = new DefaultAzureCredentialOptions()
				{
								Diagnostics =
								{
												LoggedHeaderNames = { "x-ms-request-id" },
												LoggedQueryParameters = { "api-version" },
												IsLoggingContentEnabled = true,
								}
				};
				options.Retry.NetworkTimeout = TimeSpan.FromSeconds(1000);

				var sqlServerTokenProvider = new DefaultAzureCredential(options);

				pass = (await sqlServerTokenProvider.GetTokenAsync(
								new Azure.Core.TokenRequestContext(scopes: new string[] { postgresSqlAAD! }) { })).Token;
}

var settings = new PgDbSettings()
{
				Database = db,
				Password = pass,
				Server = host,
				Username = user,
				Port = port
};

var dbContext = new PgDbContext(settings);
builder.Services.AddSingleton(dbContext);
builder.Services.AddQueueServices(storageConnection, eventQueueName, paymentQueueName);
builder.Services.AddInvoiceServices();
builder.Services.AddSwaggerServices();
builder.Services.AddApiServices();
builder.Services.AddRepositoryServices();
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient("ReferenceDataApi", clientBuilder =>
{
    clientBuilder.BaseAddress = new Uri(builder.Configuration["ApiEndpoints:ReferenceDataApiBaseUri"]);
});

var app = builder.Build();

// ensure database and tables exist
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<PgDbContext>();
    await context.Init();
}

app.SwaggerEndpoints();
app.MapInvoiceGetEndpoints();
app.MapInvoicePostEndpoints();
app.MapInvoicePutEndpoints();
app.MapInvoiceDeleteEndpoints();

app.Run();
