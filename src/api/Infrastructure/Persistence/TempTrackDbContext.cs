using Microsoft.EntityFrameworkCore;
using TempTrack.Api.Domain;

namespace TempTrack.Api.Infrastructure.Persistence;

public class TempTrackDbContext(DbContextOptions<TempTrackDbContext> options) : DbContext(options)
{
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<UserDepartmentScope> UserDepartmentScopes => Set<UserDepartmentScope>();
    public DbSet<PayRate> PayRates => Set<PayRate>();
    public DbSet<ReasonCode> ReasonCodes => Set<ReasonCode>();
    public DbSet<ApprovalChain> ApprovalChains => Set<ApprovalChain>();
    public DbSet<ApprovalChainStep> ApprovalChainSteps => Set<ApprovalChainStep>();
    public DbSet<RoutingRule> RoutingRules => Set<RoutingRule>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Department>(e =>
        {
            e.ToTable("department");
            e.HasIndex(x => x.Code).IsUnique();
            e.Property(x => x.Code).HasMaxLength(32);
            e.Property(x => x.Name).HasMaxLength(100);
            e.Property(x => x.Directorate).HasMaxLength(100);
        });

        b.Entity<AppUser>(e =>
        {
            e.ToTable("app_user");
            e.HasIndex(x => x.Email).IsUnique();
            e.HasIndex(x => x.ExternalId).IsUnique();
            e.Property(x => x.Role).HasConversion<string>().HasMaxLength(32);
            e.Property(x => x.DisplayName).HasMaxLength(200);
            e.Property(x => x.Email).HasMaxLength(254);
            e.HasOne<Department>().WithMany().HasForeignKey(x => x.HomeDepartmentId);
        });

        b.Entity<UserDepartmentScope>(e =>
        {
            e.ToTable("user_department_scope");
            e.HasKey(x => new { x.UserId, x.DepartmentId });
            e.HasOne<AppUser>().WithMany().HasForeignKey(x => x.UserId);
            e.HasOne<Department>().WithMany().HasForeignKey(x => x.DepartmentId);
        });

        b.Entity<PayRate>(e =>
        {
            e.ToTable("pay_rate");
            e.HasIndex(x => new { x.StaffType, x.Band }).IsUnique();
            e.Property(x => x.StaffType).HasConversion<string>().HasMaxLength(16);
            e.Property(x => x.HourlyRate).HasColumnType("numeric(10,2)");
            e.Property(x => x.Currency).HasMaxLength(3);
        });

        b.Entity<ReasonCode>(e =>
        {
            e.ToTable("reason_code");
            e.HasKey(x => x.Code);
            e.Property(x => x.Code).HasMaxLength(32);
            e.Property(x => x.Label).HasMaxLength(100);
        });

        b.Entity<ApprovalChain>(e =>
        {
            e.ToTable("approval_chain");
            e.HasAlternateKey(x => x.Code);
            e.Property(x => x.Code).HasMaxLength(32);
            e.Property(x => x.Name).HasMaxLength(100);
            e.HasMany(x => x.Steps).WithOne().HasForeignKey(x => x.ChainId);
        });

        b.Entity<ApprovalChainStep>(e =>
        {
            e.ToTable("approval_chain_step");
            e.HasKey(x => new { x.ChainId, x.StepOrder });
            e.Property(x => x.Role).HasConversion<string>().HasMaxLength(32);
            e.Property(x => x.Label).HasMaxLength(100);
        });

        b.Entity<RoutingRule>(e =>
        {
            e.ToTable("routing_rule");
            e.HasIndex(x => x.Priority).IsUnique();
            e.Property(x => x.MatchStaffType).HasConversion<string>().HasMaxLength(16);
            e.Property(x => x.MatchMinCost).HasColumnType("numeric(12,2)");
            e.Property(x => x.ChainCode).HasMaxLength(32);
            e.HasOne<ApprovalChain>().WithMany().HasForeignKey(x => x.ChainCode).HasPrincipalKey(c => c.Code).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
