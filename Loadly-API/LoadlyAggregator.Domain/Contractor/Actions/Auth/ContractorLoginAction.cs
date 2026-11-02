using LoadlyAggregator.Domain.Common.DTOs.DeviceToken;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Auth;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Common.Utilities;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Contractor.Actions.Auth;

public class ContractorLoginAction : IAction<ContractorLoginDto, object>
{
    private const int FirstLoginResetCodeExpirationMinutes = 60;

    private readonly IContractorRepository _contractorRepository;
    private readonly AuthService _authService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPermissionService _permissionService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly INotificationTransport _notificationTransport;
    private readonly IPasswordResetCacheService _passwordResetCacheService;
    private readonly Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? _registerDeviceTokenAction;

    public ContractorLoginAction(
        IContractorRepository contractorRepository,
        AuthService authService,
        IJwtTokenService jwtTokenService,
        IPermissionService permissionService,
        IRefreshTokenService refreshTokenService,
        INotificationTransport notificationTransport,
        IPasswordResetCacheService passwordResetCacheService,
        Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? registerDeviceTokenAction = null)
    {
        _contractorRepository = contractorRepository;
        _authService = authService;
        _jwtTokenService = jwtTokenService;
        _permissionService = permissionService;
        _refreshTokenService = refreshTokenService;
        _notificationTransport = notificationTransport;
        _passwordResetCacheService = passwordResetCacheService;
        _registerDeviceTokenAction = registerDeviceTokenAction;
    }

    public async Task<object> ExecuteAsync(ContractorLoginDto dto)
    {
        var contractor = await _contractorRepository.GetByLoginAsync((dto.Login ?? string.Empty).Trim().ToLower());
        UserValidator.ValidateUserExists(contractor, "Contractor");
        UserValidator.ValidateStatus(contractor!.Status);

        return HasNoPassword(contractor)
            ? await HandleFirstTimeLoginNoPasswordAsync(contractor)
            : await HandleLoginWithPasswordAsync(contractor, dto);
    }

    private async Task<object> HandleLoginWithPasswordAsync(
        Domain.Common.Entities.Contractor contractor,
        ContractorLoginDto dto)
    {
        _authService.ValidateCredentials(contractor, dto.Password);

        await _jwtTokenService.ClearUserRevocationAsync(contractor.Id, "contractor");

        if (contractor.MfaChannel != CommunicationChannel.None)
        {
            return await CreateMfaRequiredResponseAsync(contractor);
        }

        var loginResult = await CreateLoginResponseAsync(contractor, dto.IpAddress, dto.UserAgent, dto.DeviceInfoJson);
        await RegisterFcmTokenIfProvidedAsync(contractor.Id, dto);
        return loginResult;
    }

    private static bool HasNoPassword(Domain.Common.Entities.Contractor contractor) =>
        string.IsNullOrWhiteSpace(contractor.Password);

    private async Task<object> HandleFirstTimeLoginNoPasswordAsync(Domain.Common.Entities.Contractor contractor)
    {
        _ = await _passwordResetCacheService.IsRateLimitedAsync("contractor", contractor.Phone)
            ? throw new TooManyRequestsException("auth.too_many_attempts")
            : true;

        var code = CodeGenerator.GenerateNumericCode(6);

        await Task.WhenAll(
            _passwordResetCacheService.StoreResetAttemptAsync(
                "contractor",
                contractor.Phone,
                contractor.Id,
                code,
                CommunicationChannel.Sms,
                FirstLoginResetCodeExpirationMinutes),
            _jwtTokenService.ClearUserRevocationAsync(contractor.Id, "contractor"));

        await _notificationTransport.SendSmsAsync(
            contractor.Phone,
            code,
            NotificationDomain.Contractor,
            contractor.Id,
            "Contractor");

        return new
        {
            Redirect = "forget_password",
            Message = "auth.forget_password_sms_sent"
        };
    }

    private async Task<object> CreateMfaRequiredResponseAsync(Domain.Common.Entities.Contractor contractor) =>
        new
        {
            AccessToken = _jwtTokenService.GenerateTemporaryMfaToken(contractor.Id, "contractor", contractor.Email),
            RefreshToken = string.Empty,
            ExpiresIn = 300,
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
            },
            Redirect = "mfa"
        };

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
            },
            Redirect = "home"
        };
    }

    private async Task RegisterFcmTokenIfProvidedAsync(long contractorId, ContractorLoginDto dto)
    {
        await (string.IsNullOrEmpty(dto.FcmToken) || _registerDeviceTokenAction == null
            ? Task.CompletedTask
            : RegisterFcmTokenAsync(contractorId, dto));
    }

    private async Task RegisterFcmTokenAsync(long contractorId, ContractorLoginDto dto)
    {
        try
        {
            await _registerDeviceTokenAction!.ExecuteAsync(new RegisterDeviceTokenDto
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
