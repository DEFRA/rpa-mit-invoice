using EST.MIT.Invoice.Api.Endpoints;
using EST.MIT.Invoice.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);
var storageConnection = builder.Configuration["Storage:ConnectionString"];
var eventQueueName = builder.Configuration["Storage:EventQueueName"];
var paymentQueueName = builder.Configuration["Storage:PaymentQueueName"];

// Postgres DB
builder.Services.Configure<PgDbSettings>(builder.Configuration.GetSection("PostgresDbSettings"));
builder.Services.AddSingleton<PgDbContext>();

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
