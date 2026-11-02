using FluentValidation;
using LoadlyAggregator.Domain.Company.DTOs.RoleHasPermissions;

namespace LoadlyAggregator.API.Requests.Company.RoleHasPermissions;

public class SyncCompanyRoleHasPermissionRequest
{
    public const string Permission = "manage_permissions";

    public List<long> PermissionIds { get; set; } = new();

    public SyncCompanyRoleHasPermissionDto FromRequest(long roleId, long companyId) => new()
    {
        RoleId = roleId,
        CompanyId = companyId,
        PermissionIds = PermissionIds
    };
}

public class SyncCompanyRoleHasPermissionRequestValidator : AbstractValidator<SyncCompanyRoleHasPermissionRequest>
{
    public SyncCompanyRoleHasPermissionRequestValidator()
    {
        RuleFor(x => x.PermissionIds)
            .NotNull()
            .Must(ids => ids != null && ids.Any())
            .WithMessage("permissions.at_least_one_required");

        RuleForEach(x => x.PermissionIds)
            .GreaterThan(0)
            .WithMessage("validation.must_be_greater_than_zero");
    }
}
