using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using TempTrack.Api.Infrastructure.Persistence;
using TempTrack.Api.Infrastructure.Seeding;

namespace TempTrack.Api.Tests;

public sealed class SeedLoaderTests(ApiFactory factory) : IClassFixture<ApiFactory>, IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "seed-" + Guid.NewGuid());

    public void Dispose()
    {
        if (Directory.Exists(_dir))
        {
            Directory.Delete(_dir, true);
        }
    }

    private string CopySeed()
    {
        Directory.CreateDirectory(_dir);
        foreach (var f in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "seed")))
        {
            File.Copy(f, Path.Combine(_dir, Path.GetFileName(f)));
        }
        return _dir;
    }

    private void Edit(string file, Action<JsonNode> change)
    {
        var path = Path.Combine(_dir, file + ".json");
        var node = JsonNode.Parse(File.ReadAllText(path))!;
        change(node);
        File.WriteAllText(path, node.ToJsonString());
    }

    private async Task LoadAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TempTrackDbContext>();
        await new SeedLoader(db, NullLogger<SeedLoader>.Instance, _dir).LoadAsync(TestContext.Current.CancellationToken);
    }

    private async Task<T> QueryAsync<T>(Func<TempTrackDbContext, Task<T>> query)
    {
        using var scope = factory.Services.CreateScope();
        return await query(scope.ServiceProvider.GetRequiredService<TempTrackDbContext>());
    }

    [Fact]
    public async Task Rules_and_chain_steps_removed_from_seed_are_deleted()
    {
        CopySeed();
        await LoadAsync();
        Edit("routing-rules", n => n["items"]!.AsArray().RemoveAt(1));
        Edit("chains", n => n["items"]!.AsArray()[1]!["steps"]!.AsArray().RemoveAt(2));

        await LoadAsync();

        var priorities = await QueryAsync(db => db.RoutingRules.Select(r => r.Priority).OrderBy(p => p).ToListAsync(TestContext.Current.CancellationToken));
        var steps = await QueryAsync(db => db.ApprovalChainSteps.CountAsync(TestContext.Current.CancellationToken));
        Assert.Equal([10, 100], priorities);
        Assert.Equal(4, steps);
    }

    [Fact]
    public async Task Non_synthetic_file_rolls_back_the_whole_load()
    {
        CopySeed();
        Edit("departments", n => n["items"]!.AsArray().Add(JsonNode.Parse("""{"code":"ROLLBACK","name":"X","directorate":"Y"}""")));
        Edit("reasons", n => n["synthetic"] = false);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(LoadAsync);

        Assert.Contains("reasons.json", ex.Message);
        Assert.False(await QueryAsync(db => db.Departments.AnyAsync(d => d.Code == "ROLLBACK", TestContext.Current.CancellationToken)));
    }

    [Fact]
    public async Task Missing_items_names_the_file()
    {
        CopySeed();
        Edit("reasons", n => n.AsObject().Remove("items"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(LoadAsync);

        Assert.Contains("reasons.json", ex.Message);
    }

    [Fact]
    public async Task Unknown_department_reference_names_file_and_index_but_not_the_email()
    {
        CopySeed();
        Edit("scopes", n => n["items"]![0]!["departmentCode"] = "NOPE");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(LoadAsync);

        Assert.Contains("scopes.json item 0", ex.Message);
        Assert.DoesNotContain("@", ex.Message);
    }

    [Fact]
    public async Task Seeding_is_refused_in_production()
    {
        var env = new ProductionEnvironment();
        var service = new SeedHostedService(factory.Services, new Microsoft.Extensions.Configuration.ConfigurationBuilder().Build(), env);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.StartAsync(TestContext.Current.CancellationToken));
    }

    private sealed class ProductionEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ApplicationName { get; set; } = "test";
        public string ContentRootPath { get; set; } = "";
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}
