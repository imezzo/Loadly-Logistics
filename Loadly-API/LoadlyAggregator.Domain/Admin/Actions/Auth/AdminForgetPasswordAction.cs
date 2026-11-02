using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Utilities;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Auth;

public class AdminForgetPasswordAction : IAction<AdminForgetPasswordDto, bool>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly IPasswordResetCacheService _passwordResetCacheService;
    private readonly INotificationTransport _notificationTransport;

    public AdminForgetPasswordAction(
        IAdminRepository adminRepository,
        IPasswordResetTokenService passwordResetTokenService,
        IPasswordResetCacheService passwordResetCacheService,
        INotificationTransport notificationTransport)
    {
        _adminRepository = adminRepository;
        _passwordResetTokenService = passwordResetTokenService;
        _passwordResetCacheService = passwordResetCacheService;
        _notificationTransport = notificationTransport;
    }

    public async Task<bool> ExecuteAsync(AdminForgetPasswordDto dto)
    {
        _ = dto.Channel == CommunicationChannel.Sms
            ? true
            : throw new ValidationException("validation.admin_channel_must_be_sms");
        _ = await _passwordResetCacheService.IsRateLimitedAsync("admin", dto.Identifier)
            ? throw new TooManyRequestsException("auth.too_many_password_reset_requests")
            : true;
        var admin = await _adminRepository.GetByPhoneAsync(dto.Identifier);
        return admin == null
            ? true
            : await SendPasswordResetAsync(admin, dto);
    }

    private async Task<bool> SendPasswordResetAsync(Domain.Common.Entities.Admin admin, AdminForgetPasswordDto dto)
    {
        var resetCode = CodeGenerator.GenerateAlphanumericCode(6);
        _ = _passwordResetTokenService.GenerateResetToken(admin.Id, "admin", channel: dto.Channel);
        await _passwordResetCacheService.StoreResetAttemptAsync(
            userType: "admin",
            identifier: dto.Identifier,
            userId: admin.Id,
            code: resetCode,
            channel: dto.Channel);
        await _notificationTransport.SendSmsAsync(
            admin.Phone,
            $"Your verification code is {resetCode}",
            NotificationDomain.Admin,
            admin.Id,
            "Admin");
        return true;
    }
}
