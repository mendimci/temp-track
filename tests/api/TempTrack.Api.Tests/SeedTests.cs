using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TempTrack.Api.Domain;
using TempTrack.Api.Infrastructure.Persistence;
using TempTrack.Api.Infrastructure.Seeding;

namespace TempTrack.Api.Tests;

public sealed class SeedTests(SeedTests.SeededFactory factory) : IClassFixture<SeedTests.SeededFactory>
{
    public sealed class SeededFactory : ApiFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseSetting("Seed:Enabled", "true");
        }
    }

    private async Task<TempTrackDbContext> RunSeedAgainAsync()
    {
        var hosted = factory.Services.GetServices<IHostedService>().OfType<SeedHostedService>().Single();
        await hosted.StartAsync(TestContext.Current.CancellationToken);
        // Scope intentionally left to the test host's lifetime: the returned context is used by the caller
        return factory.Services.CreateScope().ServiceProvider.GetRequiredService<TempTrackDbContext>();
    }

    [Fact]
    public async Task Seed_runs_twice_without_duplicates()
    {
        // Startup already seeded once; this is the second run
        var db = await RunSeedAgainAsync();
        var ct = TestContext.Current.CancellationToken;

        Assert.Equal(4, await db.Departments.CountAsync(ct));
        Assert.Equal(2, await db.Departments.Select(d => d.Directorate).Distinct().CountAsync(ct));
        Assert.Equal(6, await db.Users.CountAsync(ct));
        Assert.Equal(18, await db.PayRates.CountAsync(ct));
        Assert.Equal(5, await db.ReasonCodes.CountAsync(ct));
        Assert.Equal(2, await db.ApprovalChains.CountAsync(ct));
        Assert.Equal(5, await db.ApprovalChainSteps.CountAsync(ct));
        Assert.Equal(3, await db.RoutingRules.CountAsync(ct));
        Assert.Equal(2, await db.UserDepartmentScopes.CountAsync(ct));
    }

    [Fact]
    public async Task Seed_covers_every_role_and_marks_users_synthetic()
    {
        var db = await RunSeedAgainAsync();
        var ct = TestContext.Current.CancellationToken;

        var users = await db.Users.ToListAsync(ct);

        Assert.Equal(Enum.GetValues<Role>().Length, users.Select(u => u.Role).Distinct().Count());
        Assert.Equal(2, users.Count(u => u.Role == Role.Manager && u.HomeDepartmentId is not null));
        Assert.All(users, u =>
        {
            Assert.True(u.IsSynthetic);
            Assert.EndsWith("@example.invalid", u.Email);
        });
    }

    [Fact]
    public async Task Seed_routes_agency_and_high_cost_to_extended_chain()
    {
        var db = await RunSeedAgainAsync();
        var ct = TestContext.Current.CancellationToken;

        var rules = await db.RoutingRules.OrderBy(r => r.Priority).ToListAsync(ct);
        var extended = await db.ApprovalChains.Include(c => c.Steps).SingleAsync(c => c.Code == "extended", ct);

        Assert.Equal(StaffType.Agency, rules[0].MatchStaffType);
        Assert.Equal(1000.00m, rules[1].MatchMinCost);
        Assert.Equal("standard", rules[^1].ChainCode);
        Assert.Equal("Agency approval", extended.Steps.Single(s => s.Role == Role.Finance).Label);
    }

    [Fact]
    public async Task Money_columns_are_numeric()
    {
        var db = await RunSeedAgainAsync();
        var ct = TestContext.Current.CancellationToken;

        var types = await db.Database.SqlQueryRaw<string>(
            "select data_type as \"Value\" from information_schema.columns " +
            "where (table_name = 'pay_rate' and column_name = 'hourly_rate') " +
            "or (table_name = 'routing_rule' and column_name = 'match_min_cost')").ToListAsync(ct);

        Assert.Equal(["numeric", "numeric"], types);
    }
}
