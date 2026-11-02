using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.Roles;

namespace LoadlyAggregator.API.Requests.Admin.RoleHasPermissions;

public class SyncRoleHasPermissionRequest
{
    public const string Permission = "manage_permissions";

    public List<long> PermissionIds { get; set; } = new();

    public SyncRoleHasPermissionDto FromRequest(long roleId) => new()
    {
        RoleId = roleId,
        PermissionIds = PermissionIds
    };
}

public class SyncRoleHasPermissionRequestValidator : AbstractValidator<SyncRoleHasPermissionRequest>
{
    public SyncRoleHasPermissionRequestValidator()
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
