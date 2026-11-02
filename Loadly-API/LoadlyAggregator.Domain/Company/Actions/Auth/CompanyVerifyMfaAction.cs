using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Company.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;
using MfaCodeAttempt = LoadlyAggregator.Domain.Common.Interfaces.MfaCodeAttempt;

namespace LoadlyAggregator.Domain.Company.Actions.Auth;

public class CompanyVerifyMfaAction : IAction<CompanyVerifyMfaDto, object>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMfaCacheService _mfaCacheService;
    private readonly IPermissionService _permissionService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? _registerDeviceTokenAction;

    public CompanyVerifyMfaAction(
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

    public async Task<object> ExecuteAsync(CompanyVerifyMfaDto dto)
    {
        ValidateRequest(dto);
        _ = dto.UserType == "company"
            ? true
            : throw new UnauthorizedException("auth.invalid_token_for_company");
        var company = await _companyRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(company, "Company");
        _ = await _mfaCacheService.IsRateLimitedAsync(dto.UserId, "company")
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;
        var mfaAttempt = await _mfaCacheService.GetMfaCodeAsync(dto.UserId, "company");
        _ = mfaAttempt != null
            ? true
            : await HandleNullMfaAttemptAsync(dto.UserId);
        _ = mfaAttempt!.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)
            ? true
            : await HandleInvalidMfaCodeAsync(dto.UserId, mfaAttempt);
        await UpdateVerificationStatusAsync(company!, mfaAttempt.Channel);
        await _mfaCacheService.DeleteMfaCodeAsync(dto.UserId, "company");
        await _jwtTokenService.ClearUserRevocationAsync(company!.Id, "company");
        var loginResult = await CreateLoginResponseAsync(company, dto.IpAddress, dto.UserAgent, dto.DeviceInfoJson);
        await RegisterFcmTokenIfProvidedAsync(company.Id, dto);
        return loginResult;
    }

    private async Task<bool> HandleNullMfaAttemptAsync(long userId)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "company");
        throw new ValidationException("auth.invalid_or_expired_mfa_code");
    }

    private async Task<bool> HandleInvalidMfaCodeAsync(long userId, MfaCodeAttempt mfaAttempt)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "company");
        await (mfaAttempt.FailedAttempts >= 4
            ? _mfaCacheService.DeleteMfaCodeAsync(userId, "company")
            : Task.CompletedTask);
        throw new ValidationException("auth.invalid_or_expired_mfa_code");
    }

    private async Task UpdateVerificationStatusAsync(Domain.Common.Entities.Company company, CommunicationChannel channel)
    {
        company.EmailVerified = channel == CommunicationChannel.Email ? true : company.EmailVerified;
        company.PhoneVerified = channel == CommunicationChannel.Sms ? true : company.PhoneVerified;
        await _companyRepository.UpdateAsync(company);
    }

    private static void ValidateRequest(CompanyVerifyMfaDto dto)
    {
        _ = string.IsNullOrWhiteSpace(dto.Code)
            ? throw new ValidationException("validation.mfa_code_required")
            : true;
        _ = dto.UserId <= 0
            ? throw new ValidationException("validation.user_id_required")
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

    private async Task RegisterFcmTokenIfProvidedAsync(long companyId, CompanyVerifyMfaDto dto)
    {
        await (string.IsNullOrEmpty(dto.FcmToken) || _registerDeviceTokenAction == null
            ? Task.CompletedTask
            : RegisterFcmTokenAsync(companyId, dto));
    }

    private async Task RegisterFcmTokenAsync(long companyId, CompanyVerifyMfaDto dto)
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
