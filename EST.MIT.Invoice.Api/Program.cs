using EST.MIT.Invoice.Api.Authentication;
using EST.MIT.Invoice.Api.Endpoints;
using EST.MIT.Invoice.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Postgres 
var host = builder.Configuration["POSTGRES_HOST"];
var port = builder.Configuration["POSTGRES_PORT"];
var db = builder.Configuration["POSTGRES_DB"];
var user = builder.Configuration["POSTGRES_USER"];
var pass = builder.Configuration["POSTGRES_PASSWORD"];
var postgresSqlAAD = builder.Configuration["AzureADPostgreSQLResourceID"];

var settings = new PgDbSettings()
{
    Database = db,
    Password = pass,
    Server = host,
    Username = user,
    Port = port,
    PostgresSqlAAD = postgresSqlAAD
};

builder.Services.AddSingleton<IPgDbContext>(new PgDbContext(settings, new TokenGenerator(), builder.Environment.IsProduction()));
builder.Services.AddQueueServices(builder.Configuration);
builder.Services.AddInvoiceServices();
builder.Services.AddSwaggerServices();
builder.Services.AddApiServices();
builder.Services.AddRepositoryServices();
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient("ReferenceDataApi", clientBuilder =>
{
    clientBuilder.BaseAddress = new Uri(builder.Configuration["ReferenceDataAPIBaseURI"]);
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
