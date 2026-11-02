using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.Domain.Company.DTOs.Roles;

public class CompanyRoleDto
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<CompanyRolePermissionDto> Permissions { get; set; } = new();
}

public class CompanyRolePermissionDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string GuardName { get; set; } = string.Empty;
}

public class CreateCompanyRoleDto
{
    public long CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class GetCompanyRoleDto
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
}

public class ListCompanyRolesDto : BasePagedRequest
{
    public long CompanyId { get; set; }
}

public class UpdateCompanyRoleDto
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class DeleteCompanyRoleDto
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
}
