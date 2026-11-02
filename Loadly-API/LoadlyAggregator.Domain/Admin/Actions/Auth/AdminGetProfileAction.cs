using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Admin.Actions.Auth;



public class AdminGetProfileAction : IAction<long, object>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IPermissionService _permissionService;
    public AdminGetProfileAction(IAdminRepository adminRepository, IPermissionService permissionService)
    {
        _adminRepository = adminRepository;
        _permissionService = permissionService;
    }
    public async Task<object> ExecuteAsync(long userId)
    {
        var admin = await _adminRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("admins.not_found");
        return new
        {
            Id = admin.Id,
            FullName = admin.FullName,
            UserName = admin.UserName,
            Email = admin.Email,
            Phone = admin.Phone,
            UserType = "admin",
            MfaChannel = (int)admin.MfaChannel,
            EmailVerified = admin.EmailVerified,
            PhoneVerified = admin.PhoneVerified,
            Status = (int)admin.Status,
            Type = (int)admin.Type,
            Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(admin),
            CreatedAt = admin.CreatedAt,
            UpdatedAt = admin.UpdatedAt
        };
    }
}
