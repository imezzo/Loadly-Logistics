using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;
using MfaCodeAttempt = LoadlyAggregator.Domain.Common.Interfaces.MfaCodeAttempt;
namespace LoadlyAggregator.Domain.Driver.Actions.Auth;




public class DriverVerifyMfaAction : IAction<DriverVerifyMfaDto, object>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMfaCacheService _mfaCacheService;
    private readonly IPermissionService _permissionService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? _registerDeviceTokenAction;

    public DriverVerifyMfaAction(
        IDriverRepository driverRepository,
        IJwtTokenService jwtTokenService,
        IMfaCacheService mfaCacheService,
        IPermissionService permissionService,
        IRefreshTokenService refreshTokenService,
        Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? registerDeviceTokenAction = null)
    {
        _driverRepository = driverRepository;
        _jwtTokenService = jwtTokenService;
        _mfaCacheService = mfaCacheService;
        _permissionService = permissionService;
        _refreshTokenService = refreshTokenService;
        _registerDeviceTokenAction = registerDeviceTokenAction;
    }
    public async Task<object> ExecuteAsync(DriverVerifyMfaDto dto)
    {
        ValidateRequest(dto);
        _ = dto.UserType == "driver"
            ? true
            : throw new UnauthorizedException("auth.invalid_token_for_driver");
        var driver = await _driverRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(driver, "Driver");
        _ = await _mfaCacheService.IsRateLimitedAsync(dto.UserId, "driver")
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;
        var mfaAttempt = await _mfaCacheService.GetMfaCodeAsync(dto.UserId, "driver");
        _ = mfaAttempt != null
            ? true
            : await HandleNullMfaAttemptAsync(dto.UserId);
        _ = mfaAttempt!.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)
            ? true
            : await HandleInvalidMfaCodeAsync(dto.UserId, mfaAttempt);
        await UpdateVerificationStatusAsync(driver!, mfaAttempt.Channel);
        await _mfaCacheService.DeleteMfaCodeAsync(dto.UserId, "driver");
        await _jwtTokenService.ClearUserRevocationAsync(driver!.Id, "driver");
        var loginResult = await CreateLoginResponse(driver, dto.IpAddress, dto.UserAgent, dto.DeviceInfoJson);
        await RegisterFcmTokenIfProvidedAsync(driver.Id, dto);
        return loginResult;
    }

    private async Task<bool> HandleNullMfaAttemptAsync(long userId)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "driver");
        throw new ValidationException("auth.invalid_or_expired_mfa_code");
    }

    private async Task<bool> HandleInvalidMfaCodeAsync(long userId, MfaCodeAttempt mfaAttempt)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "driver");
        await (mfaAttempt.FailedAttempts >= 4
            ? _mfaCacheService.DeleteMfaCodeAsync(userId, "driver")
            : Task.CompletedTask);
        throw new ValidationException("auth.invalid_or_expired_mfa_code");
    }

    private async Task UpdateVerificationStatusAsync(Domain.Common.Entities.Driver driver, CommunicationChannel channel)
    {
        driver.EmailVerified = channel == CommunicationChannel.Email ? true : driver.EmailVerified;
        driver.PhoneVerified = channel == CommunicationChannel.Sms ? true : driver.PhoneVerified;
        await _driverRepository.UpdateAsync(driver);
    }
    private static void ValidateRequest(DriverVerifyMfaDto dto)
    {
        _ = string.IsNullOrWhiteSpace(dto.Code)
            ? throw new ValidationException("validation.mfa_code_required")
            : true;
        _ = dto.UserId <= 0
            ? throw new ValidationException("validation.user_id_required")
            : true;
    }
    private async Task<object> CreateLoginResponse(
        Domain.Common.Entities.Driver driver,
        string? ipAddress = null,
        string? userAgent = null,
        string? deviceInfoJson = null)
    {
        var (refreshTokenEntity, plainRefreshToken) = await _refreshTokenService.CreateRefreshTokenAsync(
            driver.Id,
            "Driver",
            deviceInfo: deviceInfoJson,
            ipAddress: ipAddress,
            userAgent: userAgent);

        return new
        {
            AccessToken = _jwtTokenService.GenerateToken(
                driver.Id,
                "driver",
                driver.Email,
                roles: null,
                permissions: null,
                tokenFamilyId: refreshTokenEntity.TokenFamilyId),
            RefreshToken = plainRefreshToken,
            ExpiresIn = 3600,
            User = new
            {
                Id = driver.Id,
                FullName = driver.FullName,
                UserName = driver.UserName,
                Email = driver.Email,
                Phone = driver.Phone,
                UserType = "driver",
                MfaChannel = (int)driver.MfaChannel,
                IsOnline = (driver.WorkingStatus ?? DriverWorkingStatus.Offline) == DriverWorkingStatus.Available,
                Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(driver)
            }
        };
    }

    private async Task RegisterFcmTokenIfProvidedAsync(long driverId, DriverVerifyMfaDto dto)
    {
        await (string.IsNullOrEmpty(dto.FcmToken) || _registerDeviceTokenAction == null
            ? Task.CompletedTask
            : RegisterFcmTokenAsync(driverId, dto));
    }

    private async Task RegisterFcmTokenAsync(long driverId, DriverVerifyMfaDto dto)
    {
        try
        {
            await _registerDeviceTokenAction!.ExecuteAsync(new Domain.Common.DTOs.DeviceToken.RegisterDeviceTokenDto
            {
                UserId = driverId,
                UserType = "driver",
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
