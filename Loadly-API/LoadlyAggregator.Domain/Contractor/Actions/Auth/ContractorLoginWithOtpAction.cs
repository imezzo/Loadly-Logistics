using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;
using MfaCodeAttempt = LoadlyAggregator.Domain.Common.Interfaces.MfaCodeAttempt;

namespace LoadlyAggregator.Domain.Contractor.Actions.Auth;

public class ContractorLoginWithOtpAction : IAction<ContractorLoginWithOtpDto, object>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMfaCacheService _mfaCacheService;
    private readonly IPermissionService _permissionService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? _registerDeviceTokenAction;

    public ContractorLoginWithOtpAction(
        IContractorRepository contractorRepository,
        IJwtTokenService jwtTokenService,
        IMfaCacheService mfaCacheService,
        IPermissionService permissionService,
        IRefreshTokenService refreshTokenService,
        Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? registerDeviceTokenAction = null)
    {
        _contractorRepository = contractorRepository;
        _jwtTokenService = jwtTokenService;
        _mfaCacheService = mfaCacheService;
        _permissionService = permissionService;
        _refreshTokenService = refreshTokenService;
        _registerDeviceTokenAction = registerDeviceTokenAction;
    }

    public async Task<object> ExecuteAsync(ContractorLoginWithOtpDto dto)
    {
        ValidateRequest(dto);
        var contractor = await _contractorRepository.GetByLoginAsync((dto.Login ?? string.Empty).Trim());
        UserValidator.ValidateUserExists(contractor, "Contractor");
        UserValidator.ValidateStatus(contractor!.Status);
        _ = await _mfaCacheService.IsRateLimitedAsync(contractor.Id, "contractor")
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;

        var storedToken = await _mfaCacheService.GetLoginOtpTokenAsync(contractor.Id, "contractor");
        if (string.IsNullOrWhiteSpace(storedToken) || storedToken != dto.Token)
        {
            await _mfaCacheService.IncrementFailedAttemptsAsync(contractor.Id, "contractor");
            throw new ValidationException("auth.invalid_or_expired_verification_token");
        }

        var mfaAttempt = await _mfaCacheService.GetMfaCodeAsync(contractor.Id, "contractor");
        _ = mfaAttempt != null
            ? true
            : await HandleNullMfaAttemptAsync(contractor.Id);
        _ = mfaAttempt!.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)
            ? true
            : await HandleInvalidMfaCodeAsync(contractor.Id, mfaAttempt);

        await _mfaCacheService.DeleteMfaCodeAsync(contractor.Id, "contractor");
        await _mfaCacheService.DeleteLoginOtpTokenAsync(contractor.Id, "contractor");
        await _jwtTokenService.ClearUserRevocationAsync(contractor.Id, "contractor");

        if (!contractor.PhoneVerified)
        {
            contractor.PhoneVerified = true;
            await _contractorRepository.UpdateAsync(contractor);
            await _contractorRepository.SaveChangesAsync();
        }

        var loginResult = await CreateLoginResponseAsync(contractor, dto.IpAddress, dto.UserAgent, dto.DeviceInfoJson);
        await RegisterFcmTokenIfProvidedAsync(contractor.Id, dto);
        return loginResult;
    }

    private async Task<bool> HandleNullMfaAttemptAsync(long userId)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "contractor");
        throw new ValidationException("auth.invalid_or_expired_verification_code");
    }

    private async Task<bool> HandleInvalidMfaCodeAsync(long userId, MfaCodeAttempt mfaAttempt)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "contractor");
        await (mfaAttempt.FailedAttempts >= 4
            ? _mfaCacheService.DeleteMfaCodeAsync(userId, "contractor")
            : Task.CompletedTask);
        throw new ValidationException("auth.invalid_or_expired_verification_code");
    }

    private static void ValidateRequest(ContractorLoginWithOtpDto dto)
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

    private async Task<object> CreateLoginResponseAsync(Domain.Common.Entities.Contractor contractor,
        string? ipAddress = null,
        string? userAgent = null,
        string? deviceInfoJson = null)
    {
        var (refreshTokenEntity, plainRefreshToken) = await _refreshTokenService.CreateRefreshTokenAsync(contractor.Id, "Contractor",
            deviceInfo: deviceInfoJson,
            ipAddress: ipAddress,
            userAgent: userAgent);

        return new
        {
            AccessToken = _jwtTokenService.GenerateToken(
                contractor.Id,
                "contractor",
                contractor.Email,
                roles: null,
                permissions: null,
                tokenFamilyId: refreshTokenEntity.TokenFamilyId),
            RefreshToken = plainRefreshToken,
            ExpiresIn = 3600,
            User = new
            {
                Id = contractor.Id,
                FullName = contractor.FullName,
                UserName = contractor.UserName,
                Email = contractor.Email,
                Phone = contractor.Phone,
                UserType = "contractor",
                MfaChannel = (int)contractor.MfaChannel,
                Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(contractor)
            }
        };
    }

    private async Task RegisterFcmTokenIfProvidedAsync(long contractorId, ContractorLoginWithOtpDto dto)
    {
        await (string.IsNullOrEmpty(dto.FcmToken) || _registerDeviceTokenAction == null
            ? Task.CompletedTask
            : RegisterFcmTokenAsync(contractorId, dto));
    }

    private async Task RegisterFcmTokenAsync(long contractorId, ContractorLoginWithOtpDto dto)
    {
        try
        {
            await _registerDeviceTokenAction!.ExecuteAsync(new Domain.Common.DTOs.DeviceToken.RegisterDeviceTokenDto
            {
                UserId = contractorId,
                UserType = "contractor",
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
