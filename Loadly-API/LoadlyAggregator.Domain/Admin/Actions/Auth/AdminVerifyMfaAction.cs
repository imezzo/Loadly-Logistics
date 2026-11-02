using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;
using MfaCodeAttempt = LoadlyAggregator.Domain.Common.Interfaces.MfaCodeAttempt;
namespace LoadlyAggregator.Domain.Admin.Actions.Auth;




public class AdminVerifyMfaAction : IAction<AdminVerifyMfaDto, object>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMfaCacheService _mfaCacheService;
    private readonly IPermissionService _permissionService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? _registerDeviceTokenAction;

    public AdminVerifyMfaAction(
        IAdminRepository adminRepository,
        IJwtTokenService jwtTokenService,
        IMfaCacheService mfaCacheService,
        IPermissionService permissionService,
        IRefreshTokenService refreshTokenService,
        Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? registerDeviceTokenAction = null)
    {
        _adminRepository = adminRepository;
        _jwtTokenService = jwtTokenService;
        _mfaCacheService = mfaCacheService;
        _permissionService = permissionService;
        _refreshTokenService = refreshTokenService;
        _registerDeviceTokenAction = registerDeviceTokenAction;
    }

    public async Task<object> ExecuteAsync(AdminVerifyMfaDto dto)
    {
        ValidateRequest(dto);
        _ = dto.UserType == "admin"
            ? true
            : throw new UnauthorizedException("auth.invalid_token_for_driver");
        var admin = await _adminRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(admin, "Admin");
        _ = await _mfaCacheService.IsRateLimitedAsync(dto.UserId, "admin")
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;
        var mfaAttempt = await _mfaCacheService.GetMfaCodeAsync(dto.UserId, "admin");
        _ = mfaAttempt != null
            ? true
            : await HandleNullMfaAttemptAsync(dto.UserId);
        _ = mfaAttempt!.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)
            ? true
            : await HandleInvalidMfaCodeAsync(dto.UserId, mfaAttempt);
        await UpdateVerificationStatusAsync(admin!, mfaAttempt.Channel);
        await _mfaCacheService.DeleteMfaCodeAsync(dto.UserId, "admin");
        await _jwtTokenService.ClearUserRevocationAsync(admin!.Id, "admin");
        var loginResult = await CreateLoginResponse(admin, dto.IpAddress, dto.UserAgent, dto.DeviceInfoJson);
        await RegisterFcmTokenIfProvidedAsync(admin.Id, dto);
        return loginResult;
    }

    private async Task<bool> HandleNullMfaAttemptAsync(long userId)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "admin");
        throw new ValidationException("auth.invalid_or_expired_mfa_code");
    }

    private async Task<bool> HandleInvalidMfaCodeAsync(long userId, MfaCodeAttempt mfaAttempt)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "admin");
        await (mfaAttempt.FailedAttempts >= 4
            ? _mfaCacheService.DeleteMfaCodeAsync(userId, "admin")
            : Task.CompletedTask);
        throw new ValidationException("auth.invalid_or_expired_mfa_code");
    }

    private async Task UpdateVerificationStatusAsync(Domain.Common.Entities.Admin admin, CommunicationChannel channel)
    {
        admin.EmailVerified = channel == CommunicationChannel.Email ? true : admin.EmailVerified;
        admin.PhoneVerified = channel == CommunicationChannel.Sms ? true : admin.PhoneVerified;
        await _adminRepository.UpdateAsync(admin);
    }

    private static void ValidateRequest(AdminVerifyMfaDto dto)
    {
        _ = string.IsNullOrWhiteSpace(dto.Code)
            ? throw new ValidationException("validation.mfa_code_required")
            : true;
        _ = dto.UserId <= 0
            ? throw new ValidationException("validation.user_id_required")
            : true;
    }

    private async Task<object> CreateLoginResponse(
        Domain.Common.Entities.Admin admin,
        string? ipAddress = null,
        string? userAgent = null,
        string? deviceInfoJson = null)
    {
        var (refreshTokenEntity, plainRefreshToken) = await _refreshTokenService.CreateRefreshTokenAsync(
            admin.Id,
            "Admin",
            deviceInfo: deviceInfoJson,
            ipAddress: ipAddress,
            userAgent: userAgent);
        return new
        {
            AccessToken = _jwtTokenService.GenerateToken(
                admin.Id,
                "admin",
                admin.Email,
                roles: null,
                permissions: null,
                tokenFamilyId: refreshTokenEntity.TokenFamilyId),
            RefreshToken = plainRefreshToken,
            ExpiresIn = 3600,
            User = new
            {
                Id = admin.Id,
                FullName = admin.FullName,
                UserName = admin.UserName,
                Email = admin.Email,
                Phone = admin.Phone,
                UserType = "admin",
                MfaChannel = (int)admin.MfaChannel,
                Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(admin)
            }
        };
    }

    private async Task RegisterFcmTokenIfProvidedAsync(long adminId, AdminVerifyMfaDto dto)
    {
        await (string.IsNullOrEmpty(dto.FcmToken) || _registerDeviceTokenAction == null
            ? Task.CompletedTask
            : RegisterFcmTokenAsync(adminId, dto));
    }

    private async Task RegisterFcmTokenAsync(long adminId, AdminVerifyMfaDto dto)
    {
        try
        {
            await _registerDeviceTokenAction!.ExecuteAsync(new Domain.Common.DTOs.DeviceToken.RegisterDeviceTokenDto
            {
                UserId = adminId,
                UserType = "admin",
                FcmToken = dto.FcmToken!,
                DeviceId = dto.DeviceId,
                DeviceModel = dto.DeviceModel,
                DeviceOs = dto.DeviceOs,
                AppVersion = dto.AppVersion
            });
        }
        catch
        {
            // Silently fail - don't break login flow if FCM token registration fails
        }
    }
}
