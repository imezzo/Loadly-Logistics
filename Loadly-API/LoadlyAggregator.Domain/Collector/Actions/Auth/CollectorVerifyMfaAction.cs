using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;
using MfaCodeAttempt = LoadlyAggregator.Domain.Common.Interfaces.MfaCodeAttempt;
namespace LoadlyAggregator.Domain.Collector.Actions.Auth;




public class CollectorVerifyMfaAction : IAction<CollectorVerifyMfaDto, object>
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMfaCacheService _mfaCacheService;
    private readonly IPermissionService _permissionService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? _registerDeviceTokenAction;

    public CollectorVerifyMfaAction(
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

    public async Task<object> ExecuteAsync(CollectorVerifyMfaDto dto)
    {
        ValidateRequest(dto);
        _ = dto.UserType == "collector"
            ? true
            : throw new UnauthorizedException("auth.invalid_token_for_driver");
        var collector = await _collectorRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(collector, "Collector");
        _ = await _mfaCacheService.IsRateLimitedAsync(dto.UserId, "collector")
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;
        var mfaAttempt = await _mfaCacheService.GetMfaCodeAsync(dto.UserId, "collector");
        _ = mfaAttempt != null
            ? true
            : await HandleNullMfaAttemptAsync(dto.UserId);
        _ = mfaAttempt!.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)
            ? true
            : await HandleInvalidMfaCodeAsync(dto.UserId, mfaAttempt);
        await UpdateVerificationStatusAsync(collector!, mfaAttempt.Channel);
        await _mfaCacheService.DeleteMfaCodeAsync(dto.UserId, "collector");
        await _jwtTokenService.ClearUserRevocationAsync(collector!.Id, "collector");
        var loginResult = await CreateLoginResponse(collector, dto.IpAddress, dto.UserAgent, dto.DeviceInfoJson);
        await RegisterFcmTokenIfProvidedAsync(collector.Id, dto);
        return loginResult;
    }

    private async Task<bool> HandleNullMfaAttemptAsync(long userId)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "collector");
        throw new ValidationException("auth.invalid_or_expired_mfa_code");
    }

    private async Task<bool> HandleInvalidMfaCodeAsync(long userId, MfaCodeAttempt mfaAttempt)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "collector");
        await (mfaAttempt.FailedAttempts >= 4
            ? _mfaCacheService.DeleteMfaCodeAsync(userId, "collector")
            : Task.CompletedTask);
        throw new ValidationException("auth.invalid_or_expired_mfa_code");
    }

    private async Task UpdateVerificationStatusAsync(Domain.Common.Entities.Collector collector, CommunicationChannel channel)
    {
        collector.EmailVerified = channel == CommunicationChannel.Email ? true : collector.EmailVerified;
        collector.PhoneVerified = channel == CommunicationChannel.Sms ? true : collector.PhoneVerified;
        await _collectorRepository.UpdateAsync(collector);
    }

    private static void ValidateRequest(CollectorVerifyMfaDto dto)
    {
        _ = string.IsNullOrWhiteSpace(dto.Code)
            ? throw new ValidationException("validation.mfa_code_required")
            : true;
        _ = dto.UserId <= 0
            ? throw new ValidationException("validation.user_id_required")
            : true;
    }

    private async Task<object> CreateLoginResponse(
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

    private async Task RegisterFcmTokenIfProvidedAsync(long collectorId, CollectorVerifyMfaDto dto)
    {
        await (string.IsNullOrEmpty(dto.FcmToken) || _registerDeviceTokenAction == null
            ? Task.CompletedTask
            : RegisterFcmTokenAsync(collectorId, dto));
    }

    private async Task RegisterFcmTokenAsync(long collectorId, CollectorVerifyMfaDto dto)
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
