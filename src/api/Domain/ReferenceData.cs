namespace TempTrack.Api.Domain;

public class Department
{
    public Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required string Directorate { get; set; }
}

public class AppUser
{
    public Guid Id { get; set; }
    // Entra object id; null while auth is mocked (ADR-004)
    public string? ExternalId { get; set; }
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
    public Role Role { get; set; }
    public Guid? HomeDepartmentId { get; set; }
    public bool IsSynthetic { get; set; }
}

public class UserDepartmentScope
{
    public Guid UserId { get; set; }
    public Guid DepartmentId { get; set; }
}

public class PayRate
{
    public Guid Id { get; set; }
    public StaffType StaffType { get; set; }
    public int Band { get; set; }
    public decimal HourlyRate { get; set; }
    public string Currency { get; set; } = "GBP";
}

public class ReasonCode
{
    public required string Code { get; set; }
    public required string Label { get; set; }
}

public class ApprovalChain
{
    public Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public List<ApprovalChainStep> Steps { get; set; } = [];
}

public class ApprovalChainStep
{
    public Guid ChainId { get; set; }
    public int StepOrder { get; set; }
    public Role Role { get; set; }
    public required string Label { get; set; }
}

public class RoutingRule
{
    public Guid Id { get; set; }
    // First match by ascending priority wins; the last rule has no conditions
    public int Priority { get; set; }
    public StaffType? MatchStaffType { get; set; }
    public decimal? MatchMinCost { get; set; }
    public required string ChainCode { get; set; }
}
