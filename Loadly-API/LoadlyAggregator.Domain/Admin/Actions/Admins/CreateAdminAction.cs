using LoadlyAggregator.Domain.Admin.DTOs.Admins;
using LoadlyAggregator.Domain.Admin.Services;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Admins;

public class CreateAdminAction
{
    private readonly IAdminRepository _adminRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAdminNotificationService _adminNotificationService;
    private readonly IResetPasswordUrlService _resetPasswordUrlService;
    private readonly IPermissionService _permissionService;

    public CreateAdminAction(
        IAdminRepository adminRepository,
        IUnitOfWork unitOfWork,
        IAdminNotificationService adminNotificationService,
        IResetPasswordUrlService resetPasswordUrlService,
        IPermissionService permissionService)
    {
        _adminRepository = adminRepository;
        _unitOfWork = unitOfWork;
        _adminNotificationService = adminNotificationService;
        _resetPasswordUrlService = resetPasswordUrlService;
        _permissionService = permissionService;
    }

    public async Task ExecuteAsync(CreateAdminDto dto)
    {
        var admin = new Common.Entities.Admin
        {
            FullName = dto.FullName,
            UserName = dto.UserName.ToLower(),
            Email = dto.Email.ToLower(),
            Phone = dto.Phone,
            Type = AdminType.ADMIN,
            Status = (UserStatus)dto.Status,
            Password = string.Empty,
            EmailVerified = false,
            PhoneVerified = false,
            MfaChannel = (Shared.Enums.CommunicationChannel)dto.MfaChannel
        };
        await _adminRepository.AddAsync(admin);
        await _unitOfWork.SaveChangesAsync();

        await _permissionService.AssignRolesIfProvidedAsync(admin, dto.RoleIds);

        var setPasswordUrl = $"{_resetPasswordUrlService.GetBaseUrl("admin")}/auth/forgot-password";
        await _adminNotificationService.SendAdminWelcomeSmsAsync(admin, setPasswordUrl);
    }
}
