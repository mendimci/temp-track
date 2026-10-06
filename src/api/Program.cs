using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TempTrack.Api.Infrastructure.Http;
using TempTrack.Api.Infrastructure.Persistence;
using TempTrack.Api.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole(o => o.IncludeScopes = true);

builder.Services.AddProblemDetails(o => o.CustomizeProblemDetails = ctx =>
{
    var correlationId = CorrelationIdMiddleware.Get(ctx.HttpContext);
    if (correlationId is not null)
    {
        ctx.ProblemDetails.Extensions["correlationId"] = correlationId;
    }
});
builder.Services.AddOpenApi();

// Build-time OpenAPI generation loads the app without config or a database
var isOpenApiBuild = Assembly.GetEntryAssembly()?.GetName().Name == "GetDocument.Insider";
var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString = isOpenApiBuild
        ? "Host=localhost;Database=temptrack"
        : throw new InvalidOperationException("ConnectionStrings:Default is not configured");
}

builder.Services.AddDbContext<TempTrackDbContext>(o =>
    o.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

const string CorsPolicy = "Spa";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(o => o.AddPolicy(CorsPolicy, p => p
    .WithOrigins(allowedOrigins)
    .WithMethods("GET", "POST", "PUT")
    .WithHeaders(CorrelationIdMiddleware.HeaderName, "Content-Type")
    .WithExposedHeaders(CorrelationIdMiddleware.HeaderName)));

if (builder.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    builder.Services.AddHostedService<MigrationHostedService>();
}

if (builder.Configuration.GetValue<bool>("Seed:Enabled"))
{
    builder.Services.AddHostedService<SeedHostedService>();
}

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors(CorsPolicy);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/health", async (TempTrackDbContext db, CancellationToken ct) =>
{
    var dbUp = await db.Database.CanConnectAsync(ct);
    var body = new HealthResponse(dbUp ? "Healthy" : "Unhealthy", dbUp ? "Up" : "Down");
    return dbUp ? Results.Ok(body) : Results.Json(body, statusCode: StatusCodes.Status503ServiceUnavailable);
})
.WithName("GetHealth")
.Produces<HealthResponse>()
.Produces<HealthResponse>(StatusCodes.Status503ServiceUnavailable);

app.Run();

public sealed record HealthResponse(string Status, string Database);

public partial class Program;
