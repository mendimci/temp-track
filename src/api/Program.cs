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

// Placeholder default keeps startup and build-time OpenAPI generation working without config
builder.Services.AddDbContext<TempTrackDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default") ?? "Host=localhost;Database=temptrack")
     .UseSnakeCaseNamingConvention());

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

app.MapOpenApi();

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
