using Microsoft.EntityFrameworkCore;

namespace TempTrack.Api.Infrastructure.Persistence;

// A hosted service (not code in Program.cs) so build-time OpenAPI generation never needs a database
public sealed class MigrationHostedService(IServiceProvider services) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TempTrackDbContext>();
        await db.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
