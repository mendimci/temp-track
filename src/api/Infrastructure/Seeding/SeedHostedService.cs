namespace TempTrack.Api.Infrastructure.Seeding;

// Registered after MigrationHostedService so the schema exists when it runs
public sealed class SeedHostedService(IServiceProvider services, IConfiguration config) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = services.CreateAsyncScope();
        var dir = config["Seed:Directory"] ?? Path.Combine(AppContext.BaseDirectory, "seed");
        var loader = ActivatorUtilities.CreateInstance<SeedLoader>(scope.ServiceProvider, dir);
        await loader.LoadAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
