using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;
using MfaCodeAttempt = LoadlyAggregator.Domain.Common.Interfaces.MfaCodeAttempt;

namespace LoadlyAggregator.Domain.Collector.Actions.Auth;

public class CollectorLoginWithOtpAction : IAction<CollectorLoginWithOtpDto, object>
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMfaCacheService _mfaCacheService;
    private readonly IPermissionService _permissionService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? _registerDeviceTokenAction;

    public CollectorLoginWithOtpAction(
        ICollectorRepository collectorRepository,
        IJwtTokenService jwtTokenService,
        IMfaCacheService mfaCacheService,
        IPermissionService permissionService,
        IRefreshTokenService refreshTokenService,
        Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? registerDeviceTokenAction = null)
    {
        _collectorRepository = collectorRepository;
        _jwtTokenService = jwtTokenService;
        _mfaCacheService = mfaCacheService;
        _permissionService = permissionService;
        _refreshTokenService = refreshTokenService;
        _registerDeviceTokenAction = registerDeviceTokenAction;
    }

    public async Task<object> ExecuteAsync(CollectorLoginWithOtpDto dto)
    {
        ValidateRequest(dto);
        var collector = await _collectorRepository.GetByLoginAsync(dto.Login.Trim());
        UserValidator.ValidateUserExists(collector, "Collector");
        UserValidator.ValidateStatus(collector!.Status);
        _ = await _mfaCacheService.IsRateLimitedAsync(collector.Id, "collector")
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;

        var storedToken = await _mfaCacheService.GetLoginOtpTokenAsync(collector.Id, "collector");
        if (string.IsNullOrWhiteSpace(storedToken) || storedToken != dto.Token)
        {
            await _mfaCacheService.IncrementFailedAttemptsAsync(collector.Id, "collector");
            throw new ValidationException("auth.invalid_or_expired_verification_token");
        }

        var mfaAttempt = await _mfaCacheService.GetMfaCodeAsync(collector.Id, "collector");
        _ = mfaAttempt != null
            ? true
            : await HandleNullMfaAttemptAsync(collector.Id);
        _ = mfaAttempt!.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)
            ? true
            : await HandleInvalidMfaCodeAsync(collector.Id, mfaAttempt);

        await _mfaCacheService.DeleteMfaCodeAsync(collector.Id, "collector");
        await _mfaCacheService.DeleteLoginOtpTokenAsync(collector.Id, "collector");
        await _jwtTokenService.ClearUserRevocationAsync(collector.Id, "collector");

        if (!collector.PhoneVerified)
        {
            collector.PhoneVerified = true;
            await _collectorRepository.UpdateAsync(collector);
            await _collectorRepository.SaveChangesAsync();
        }

        var loginResult = await CreateLoginResponseAsync(collector, dto.IpAddress, dto.UserAgent, dto.DeviceInfoJson);
        await RegisterFcmTokenIfProvidedAsync(collector.Id, dto);
        return loginResult;
    }

    private async Task<bool> HandleNullMfaAttemptAsync(long userId)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "collector");
        throw new ValidationException("auth.invalid_or_expired_verification_code");
    }

    private async Task<bool> HandleInvalidMfaCodeAsync(long userId, MfaCodeAttempt mfaAttempt)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "collector");
        await (mfaAttempt.FailedAttempts >= 4
            ? _mfaCacheService.DeleteMfaCodeAsync(userId, "collector")
            : Task.CompletedTask);
        throw new ValidationException("auth.invalid_or_expired_verification_code");
    }

    private static void ValidateRequest(CollectorLoginWithOtpDto dto)
    {
        _ = string.IsNullOrWhiteSpace(dto.Code)
            ? throw new ValidationException("validation.otp_code_required")
            : true;
        _ = string.IsNullOrWhiteSpace(dto.Login)
            ? throw new ValidationException("validation.login_required")
            : true;
        _ = string.IsNullOrWhiteSpace(dto.Token)
            ? throw new ValidationException("validation.token_required")
            : true;
    }

    private async Task<object> CreateLoginResponseAsync(
        Domain.Common.Entities.Collector collector,
        string? ipAddress = null,
        string? userAgent = null,
        string? deviceInfoJson = null)
    {
        var (refreshTokenEntity, plainRefreshToken) = await _refreshTokenService.CreateRefreshTokenAsync(
            collector.Id,
            "Collector",
            deviceInfo: deviceInfoJson,
            ipAddress: ipAddress,
            userAgent: userAgent);

        return new
        {
            AccessToken = _jwtTokenService.GenerateToken(
                collector.Id,
                "collector",
                collector.Email,
                roles: null,
                permissions: null,
                tokenFamilyId: refreshTokenEntity.TokenFamilyId),
            RefreshToken = plainRefreshToken,
            ExpiresIn = 3600,
            User = new
            {
                Id = collector.Id,
                FullName = collector.FullName,
                UserName = collector.UserName,
                Email = collector.Email,
                Phone = collector.Phone,
                UserType = "collector",
                MfaChannel = (int)collector.MfaChannel,
                Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(collector)
            }
        };
    }

    private async Task RegisterFcmTokenIfProvidedAsync(long collectorId, CollectorLoginWithOtpDto dto)
    {
        await (string.IsNullOrEmpty(dto.FcmToken) || _registerDeviceTokenAction == null
            ? Task.CompletedTask
            : RegisterFcmTokenAsync(collectorId, dto));
    }

    private async Task RegisterFcmTokenAsync(long collectorId, CollectorLoginWithOtpDto dto)
    {
        try
        {
            await _registerDeviceTokenAction!.ExecuteAsync(new Domain.Common.DTOs.DeviceToken.RegisterDeviceTokenDto
            {
                UserId = collectorId,
                UserType = "collector",
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
