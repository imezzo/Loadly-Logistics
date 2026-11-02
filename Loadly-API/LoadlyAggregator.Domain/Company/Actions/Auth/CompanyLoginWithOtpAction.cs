using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Company.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;
using MfaCodeAttempt = LoadlyAggregator.Domain.Common.Interfaces.MfaCodeAttempt;

namespace LoadlyAggregator.Domain.Company.Actions.Auth;

public class CompanyLoginWithOtpAction : IAction<CompanyLoginWithOtpDto, object>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMfaCacheService _mfaCacheService;
    private readonly IPermissionService _permissionService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? _registerDeviceTokenAction;

    public CompanyLoginWithOtpAction(
        ICompanyRepository companyRepository,
        IJwtTokenService jwtTokenService,
        IMfaCacheService mfaCacheService,
        IPermissionService permissionService,
        IRefreshTokenService refreshTokenService,
        Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? registerDeviceTokenAction = null)
    {
        _companyRepository = companyRepository;
        _jwtTokenService = jwtTokenService;
        _mfaCacheService = mfaCacheService;
        _permissionService = permissionService;
        _refreshTokenService = refreshTokenService;
        _registerDeviceTokenAction = registerDeviceTokenAction;
    }

    public async Task<object> ExecuteAsync(CompanyLoginWithOtpDto dto)
    {
        ValidateRequest(dto);
        var company = await _companyRepository.GetByLoginAsync((dto.Login ?? string.Empty).Trim());
        UserValidator.ValidateUserExists(company, "Company");
        UserValidator.ValidateStatus(company!.Status);
        _ = await _mfaCacheService.IsRateLimitedAsync(company.Id, "company")
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;

        var storedToken = await _mfaCacheService.GetLoginOtpTokenAsync(company.Id, "company");
        if (string.IsNullOrWhiteSpace(storedToken) || storedToken != dto.Token)
        {
            await _mfaCacheService.IncrementFailedAttemptsAsync(company.Id, "company");
            throw new ValidationException("auth.invalid_or_expired_verification_token");
        }

        var mfaAttempt = await _mfaCacheService.GetMfaCodeAsync(company.Id, "company");
        _ = mfaAttempt != null
            ? true
            : await HandleNullMfaAttemptAsync(company.Id);
        _ = mfaAttempt!.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)
            ? true
            : await HandleInvalidMfaCodeAsync(company.Id, mfaAttempt);

        await _mfaCacheService.DeleteMfaCodeAsync(company.Id, "company");
        await _mfaCacheService.DeleteLoginOtpTokenAsync(company.Id, "company");
        await _jwtTokenService.ClearUserRevocationAsync(company.Id, "company");

        if (!company.PhoneVerified)
        {
            company.PhoneVerified = true;
            await _companyRepository.UpdateAsync(company);
            await _companyRepository.SaveChangesAsync();
        }

        var loginResult = await CreateLoginResponseAsync(company, dto.IpAddress, dto.UserAgent, dto.DeviceInfoJson);
        await RegisterFcmTokenIfProvidedAsync(company.Id, dto);
        return loginResult;
    }

    private async Task<bool> HandleNullMfaAttemptAsync(long userId)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "company");
        throw new ValidationException("auth.invalid_or_expired_verification_code");
    }

    private async Task<bool> HandleInvalidMfaCodeAsync(long userId, MfaCodeAttempt mfaAttempt)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "company");
        await (mfaAttempt.FailedAttempts >= 4
            ? _mfaCacheService.DeleteMfaCodeAsync(userId, "company")
            : Task.CompletedTask);
        throw new ValidationException("auth.invalid_or_expired_verification_code");
    }

    private static void ValidateRequest(CompanyLoginWithOtpDto dto)
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
        Domain.Common.Entities.Company company,
        string? ipAddress = null,
        string? userAgent = null,
        string? deviceInfoJson = null)
    {
        var (refreshTokenEntity, plainRefreshToken) = await _refreshTokenService.CreateRefreshTokenAsync(
            company.Id,
            "Company",
            deviceInfo: deviceInfoJson,
            ipAddress: ipAddress,
            userAgent: userAgent);

        return new
        {
            AccessToken = _jwtTokenService.GenerateToken(
                company.Id,
                "company",
                company.Email,
                roles: null,
                permissions: null,
                tokenFamilyId: refreshTokenEntity.TokenFamilyId,
                companyId: company.CompanyId ?? company.Id),
            RefreshToken = plainRefreshToken,
            ExpiresIn = 3600,
            User = new
            {
                Id = company.Id,
                FullName = company.FullName,
                UserName = company.UserName,
                Email = company.Email,
                Phone = company.Phone,
                UserType = "company",
                MfaChannel = (int)company.MfaChannel,
                Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(company)
            }
        };
    }

    private async Task RegisterFcmTokenIfProvidedAsync(long companyId, CompanyLoginWithOtpDto dto)
    {
        await (string.IsNullOrEmpty(dto.FcmToken) || _registerDeviceTokenAction == null
            ? Task.CompletedTask
            : RegisterFcmTokenAsync(companyId, dto));
    }

    private async Task RegisterFcmTokenAsync(long companyId, CompanyLoginWithOtpDto dto)
    {
        try
        {
            await _registerDeviceTokenAction!.ExecuteAsync(new Domain.Common.DTOs.DeviceToken.RegisterDeviceTokenDto
            {
                UserId = companyId,
                UserType = "company",
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
