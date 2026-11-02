using LoadlyAggregator.Domain.Common.DTOs.DeviceToken;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Auth;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Common.Utilities;
using LoadlyAggregator.Domain.Company.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Auth;

public class CompanyLoginAction : IAction<CompanyLoginDto, object>
{
    private const int FirstLoginResetCodeExpirationMinutes = 60;

    private readonly ICompanyRepository _companyRepository;
    private readonly AuthService _authService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPermissionService _permissionService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly INotificationTransport _notificationTransport;
    private readonly IPasswordResetCacheService _passwordResetCacheService;
    private readonly Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? _registerDeviceTokenAction;

    public CompanyLoginAction(
        ICompanyRepository companyRepository,
        AuthService authService,
        IJwtTokenService jwtTokenService,
        IPermissionService permissionService,
        IRefreshTokenService refreshTokenService,
        INotificationTransport notificationTransport,
        IPasswordResetCacheService passwordResetCacheService,
        Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? registerDeviceTokenAction = null)
    {
        _companyRepository = companyRepository;
        _authService = authService;
        _jwtTokenService = jwtTokenService;
        _permissionService = permissionService;
        _refreshTokenService = refreshTokenService;
        _notificationTransport = notificationTransport;
        _passwordResetCacheService = passwordResetCacheService;
        _registerDeviceTokenAction = registerDeviceTokenAction;
    }

    public async Task<object> ExecuteAsync(CompanyLoginDto dto)
    {
        var company = await _companyRepository.GetByLoginAsync((dto.Login ?? string.Empty).Trim().ToLower());
        UserValidator.ValidateUserExists(company, "Company");
        UserValidator.ValidateStatus(company!.Status);

        return HasNoPassword(company)
            ? await HandleFirstTimeLoginNoPasswordAsync(company)
            : await HandleLoginWithPasswordAsync(company, dto);
    }

    private async Task<object> HandleLoginWithPasswordAsync(
        Domain.Common.Entities.Company company,
        CompanyLoginDto dto)
    {
        _authService.ValidateCredentials(company, dto.Password);

        await _jwtTokenService.ClearUserRevocationAsync(company.Id, "company");

        if (company.MfaChannel != CommunicationChannel.None)
        {
            return await CreateMfaRequiredResponseAsync(company);
        }

        var loginResult = await CreateLoginResponseAsync(company, dto.IpAddress, dto.UserAgent, dto.DeviceInfoJson);
        await RegisterFcmTokenIfProvidedAsync(company.Id, dto);
        return loginResult;
    }

    private static bool HasNoPassword(Domain.Common.Entities.Company company) =>
        string.IsNullOrWhiteSpace(company.Password);

    private async Task<object> HandleFirstTimeLoginNoPasswordAsync(Domain.Common.Entities.Company company)
    {
        _ = await _passwordResetCacheService.IsRateLimitedAsync("company", company.Phone)
            ? throw new TooManyRequestsException("auth.too_many_attempts")
            : true;

        var code = CodeGenerator.GenerateNumericCode(6);

        await Task.WhenAll(
            _passwordResetCacheService.StoreResetAttemptAsync(
                "company",
                company.Phone,
                company.Id,
                code,
                CommunicationChannel.Sms,
                FirstLoginResetCodeExpirationMinutes),
            _jwtTokenService.ClearUserRevocationAsync(company.Id, "company"));

        await _notificationTransport.SendSmsAsync(
            company.Phone,
            code,
            NotificationDomain.Company,
            company.Id,
            "Company");

        return new
        {
            Redirect = "forget_password",
            Message = "auth.forget_password_sms_sent"
        };
    }

    private async Task<object> CreateMfaRequiredResponseAsync(Domain.Common.Entities.Company company) =>
        new
        {
            AccessToken = _jwtTokenService.GenerateTemporaryMfaToken(company.Id, "company", company.Email),
            RefreshToken = string.Empty,
            ExpiresIn = 300,
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
            },
            Redirect = "mfa"
        };

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
            },
            Redirect = "home"
        };
    }

    private async Task RegisterFcmTokenIfProvidedAsync(long companyId, CompanyLoginDto dto)
    {
        await (string.IsNullOrEmpty(dto.FcmToken) || _registerDeviceTokenAction == null
            ? Task.CompletedTask
            : RegisterFcmTokenAsync(companyId, dto));
    }

    private async Task RegisterFcmTokenAsync(long companyId, CompanyLoginDto dto)
    {
        try
        {
            await _registerDeviceTokenAction!.ExecuteAsync(new RegisterDeviceTokenDto
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
