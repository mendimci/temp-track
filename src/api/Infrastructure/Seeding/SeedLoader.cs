using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TempTrack.Api.Domain;
using TempTrack.Api.Infrastructure.Persistence;

namespace TempTrack.Api.Infrastructure.Seeding;

public sealed class SeedLoader(TempTrackDbContext db, ILogger<SeedLoader> logger, string seedDirectory)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private sealed record SeedFile<T>(bool Synthetic, List<T> Items);
    private sealed record DepartmentSeed(string Code, string Name, string Directorate);
    private sealed record UserSeed(string Email, string DisplayName, Role Role, string? HomeDepartment);
    private sealed record ScopeSeed(string UserEmail, string DepartmentCode);
    private sealed record PayRateSeed(StaffType StaffType, int Band, decimal HourlyRate);
    private sealed record ReasonSeed(string Code, string Label);
    private sealed record StepSeed(int Order, Role Role, string Label);
    private sealed record ChainSeed(string Code, string Name, List<StepSeed> Steps);
    private sealed record RuleSeed(int Priority, StaffType? MatchStaffType, decimal? MatchMinCost, string ChainCode);

    public async Task LoadAsync(CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        var departments = await UpsertDepartments(await Read<DepartmentSeed>("departments", ct), ct);
        var users = await UpsertUsers(await Read<UserSeed>("users", ct), departments, ct);
        await UpsertScopes(await Read<ScopeSeed>("scopes", ct), users, departments, ct);
        await UpsertPayRates(await Read<PayRateSeed>("pay-rates", ct), ct);
        await UpsertReasons(await Read<ReasonSeed>("reasons", ct), ct);
        await UpsertChains(await Read<ChainSeed>("chains", ct), ct);
        await UpsertRules(await Read<RuleSeed>("routing-rules", ct), ct);

        await tx.CommitAsync(ct);
        logger.LogInformation("Reference data seed applied from {SeedDirectory}", seedDirectory);
    }

    private async Task<List<T>> Read<T>(string name, CancellationToken ct)
    {
        var path = Path.Combine(seedDirectory, name + ".json");
        await using var stream = File.OpenRead(path);
        var file = await JsonSerializer.DeserializeAsync<SeedFile<T>>(stream, Json, ct)
            ?? throw new InvalidOperationException($"Seed file {name}.json is empty");
        // Guards against real data being loaded into a non-production environment by mistake
        if (!file.Synthetic)
        {
            throw new InvalidOperationException($"Seed file {name}.json is not marked synthetic");
        }
        return file.Items ?? throw new InvalidOperationException($"Seed file {name}.json has no items");
    }

    private static T Resolve<T>(Dictionary<string, T> map, string key, string file, int index, string what) =>
        map.TryGetValue(key, out var value)
            ? value
            : throw new InvalidOperationException($"Seed file {file}.json item {index}: unknown {what}");

    private async Task<Dictionary<string, Department>> UpsertDepartments(List<DepartmentSeed> items, CancellationToken ct)
    {
        var existing = await db.Departments.ToDictionaryAsync(x => x.Code, ct);
        foreach (var s in items)
        {
            if (!existing.TryGetValue(s.Code, out var e))
            {
                existing[s.Code] = e = new Department { Id = Guid.NewGuid(), Code = s.Code, Name = s.Name, Directorate = s.Directorate };
                db.Departments.Add(e);
            }
            e.Name = s.Name;
            e.Directorate = s.Directorate;
        }
        await db.SaveChangesAsync(ct);
        return existing;
    }

    private async Task<Dictionary<string, AppUser>> UpsertUsers(List<UserSeed> items, Dictionary<string, Department> departments, CancellationToken ct)
    {
        var existing = await db.Users.ToDictionaryAsync(x => x.Email, ct);
        foreach (var (i, s) in items.Index())
        {
            if (!existing.TryGetValue(s.Email, out var e))
            {
                existing[s.Email] = e = new AppUser { Id = Guid.NewGuid(), Email = s.Email, DisplayName = s.DisplayName };
                db.Users.Add(e);
            }
            e.DisplayName = s.DisplayName;
            e.Role = s.Role;
            e.HomeDepartmentId = s.HomeDepartment is null
                ? null
                : Resolve(departments, s.HomeDepartment, "users", i, $"department code '{s.HomeDepartment}'").Id;
            e.IsSynthetic = true;
        }
        await db.SaveChangesAsync(ct);
        return existing;
    }

    private async Task UpsertScopes(List<ScopeSeed> items, Dictionary<string, AppUser> users, Dictionary<string, Department> departments, CancellationToken ct)
    {
        var existing = (await db.UserDepartmentScopes.ToListAsync(ct)).Select(x => (x.UserId, x.DepartmentId)).ToHashSet();
        foreach (var (i, s) in items.Index())
        {
            var key = (
                Resolve(users, s.UserEmail, "scopes", i, "user").Id,
                Resolve(departments, s.DepartmentCode, "scopes", i, $"department code '{s.DepartmentCode}'").Id);
            if (existing.Add(key))
            {
                db.UserDepartmentScopes.Add(new UserDepartmentScope { UserId = key.Item1, DepartmentId = key.Item2 });
            }
        }
        await db.SaveChangesAsync(ct);
    }

    private async Task UpsertPayRates(List<PayRateSeed> items, CancellationToken ct)
    {
        var existing = await db.PayRates.ToDictionaryAsync(x => (x.StaffType, x.Band), ct);
        foreach (var s in items)
        {
            if (!existing.TryGetValue((s.StaffType, s.Band), out var e))
            {
                existing[(s.StaffType, s.Band)] = e = new PayRate { Id = Guid.NewGuid(), StaffType = s.StaffType, Band = s.Band };
                db.PayRates.Add(e);
            }
            e.HourlyRate = s.HourlyRate;
        }
        await db.SaveChangesAsync(ct);
    }

    private async Task UpsertReasons(List<ReasonSeed> items, CancellationToken ct)
    {
        var existing = await db.ReasonCodes.ToDictionaryAsync(x => x.Code, ct);
        foreach (var s in items)
        {
            if (!existing.TryGetValue(s.Code, out var e))
            {
                existing[s.Code] = e = new ReasonCode { Code = s.Code, Label = s.Label };
                db.ReasonCodes.Add(e);
            }
            e.Label = s.Label;
        }
        await db.SaveChangesAsync(ct);
    }

    private async Task UpsertChains(List<ChainSeed> items, CancellationToken ct)
    {
        var existing = await db.ApprovalChains.Include(x => x.Steps).ToDictionaryAsync(x => x.Code, ct);
        foreach (var s in items)
        {
            if (!existing.TryGetValue(s.Code, out var chain))
            {
                existing[s.Code] = chain = new ApprovalChain { Id = Guid.NewGuid(), Code = s.Code, Name = s.Name };
                db.ApprovalChains.Add(chain);
            }
            chain.Name = s.Name;
            // Steps missing from the seed are removed so the chain matches the file exactly
            var stale = chain.Steps.Where(x => s.Steps.All(y => y.Order != x.StepOrder)).ToList();
            db.ApprovalChainSteps.RemoveRange(stale);
            chain.Steps.RemoveAll(stale.Contains);
            foreach (var step in s.Steps)
            {
                var e = chain.Steps.FirstOrDefault(x => x.StepOrder == step.Order);
                if (e is null)
                {
                    chain.Steps.Add(new ApprovalChainStep { StepOrder = step.Order, Role = step.Role, Label = step.Label });
                    continue;
                }
                e.Role = step.Role;
                e.Label = step.Label;
            }
        }
        await db.SaveChangesAsync(ct);
    }

    private async Task UpsertRules(List<RuleSeed> items, CancellationToken ct)
    {
        var existing = await db.RoutingRules.ToDictionaryAsync(x => x.Priority, ct);
        foreach (var s in items)
        {
            if (!existing.TryGetValue(s.Priority, out var e))
            {
                existing[s.Priority] = e = new RoutingRule { Id = Guid.NewGuid(), Priority = s.Priority, ChainCode = s.ChainCode };
                db.RoutingRules.Add(e);
            }
            e.MatchStaffType = s.MatchStaffType;
            e.MatchMinCost = s.MatchMinCost;
            e.ChainCode = s.ChainCode;
        }
        // Rules removed from the seed must stop routing, so rules are mirrored, not just upserted
        var seeded = items.Select(x => x.Priority).ToHashSet();
        db.RoutingRules.RemoveRange(existing.Values.Where(x => !seeded.Contains(x.Priority)));
        await db.SaveChangesAsync(ct);
    }
}
