using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.DTOs.DeviceToken;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Auth;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Collector.Actions.Auth;




public class CollectorLoginAction : IAction<CollectorLoginDto, object>
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly AuthService _authService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPermissionService _permissionService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ILogger<CollectorLoginAction> _logger;
    private readonly Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? _registerDeviceTokenAction;

    public CollectorLoginAction(
        ICollectorRepository collectorRepository,
        AuthService authService,
        IJwtTokenService jwtTokenService,
        IPermissionService permissionService,
        IRefreshTokenService refreshTokenService,
        ILogger<CollectorLoginAction> logger,
        Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? registerDeviceTokenAction = null)
    {
        _collectorRepository = collectorRepository;
        _authService = authService;
        _jwtTokenService = jwtTokenService;
        _permissionService = permissionService;
        _refreshTokenService = refreshTokenService;
        _logger = logger;
        _registerDeviceTokenAction = registerDeviceTokenAction;
    }

    public async Task<object> ExecuteAsync(CollectorLoginDto dto)
    {
        _logger.LogInformation(
            "Collector login attempt started for login: {Login}, IP: {IpAddress}, UserAgent: {UserAgent}",
            dto.Login, dto.IpAddress, dto.UserAgent);
        var collector = await _collectorRepository.GetByLoginAsync(dto.Login.Trim());
        _authService.ValidateCredentials(collector, dto.Password);
        _logger.LogInformation(
            "Collector credentials validated successfully for CollectorId: {CollectorId}, Username: {Username}",
            collector!.Id, collector.UserName);

        await _jwtTokenService.ClearUserRevocationAsync(collector.Id, "collector");

        if (collector.MfaChannel != CommunicationChannel.None)
        {
            return await CreateMfaRequiredResponseAsync(collector);
        }

        var loginResult = await CreateLoginResponseAsync(collector, dto.IpAddress, dto.UserAgent, dto.DeviceInfoJson);
        await RegisterFcmTokenIfProvidedAsync(collector.Id, dto);
        return loginResult;
    }

    private async Task<object> CreateMfaRequiredResponseAsync(Domain.Common.Entities.Collector collector)
    {
        var permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(collector);
        _logger.LogInformation(
            "MFA temporary token generated for CollectorId: {CollectorId}, Token expires in 5 minutes",
            collector.Id);
        return new
        {
            AccessToken = _jwtTokenService.GenerateTemporaryMfaToken(collector.Id, "collector", collector.Email),
            RefreshToken = string.Empty,
            ExpiresIn = 300,
            User = new
            {
                Id = collector.Id,
                FullName = collector.FullName,
                UserName = collector.UserName,
                Email = collector.Email,
                Phone = collector.Phone,
                UserType = "collector",
                MfaChannel = (int)collector.MfaChannel,
                Permissions = permissions
            }
        };
    }

    private async Task<object> CreateLoginResponseAsync(
        Domain.Common.Entities.Collector collector,
        string? ipAddress = null,
        string? userAgent = null,
        string? deviceInfoJson = null)
    {
        var permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(collector);
        var (refreshTokenEntity, plainRefreshToken) = await _refreshTokenService.CreateRefreshTokenAsync(
            collector.Id,
            "Collector",
            deviceInfo: deviceInfoJson,
            ipAddress: ipAddress,
            userAgent: userAgent);
        _logger.LogInformation(
            "Access token generated for CollectorId: {CollectorId}, Token expires in 1 hour",
            collector.Id);
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
                Permissions = permissions
            }
        };
    }

    private async Task RegisterFcmTokenIfProvidedAsync(long collectorId, CollectorLoginDto dto)
    {
        await (string.IsNullOrEmpty(dto.FcmToken) || _registerDeviceTokenAction == null
            ? Task.CompletedTask
            : RegisterFcmTokenAsync(collectorId, dto));
    }

    private async Task RegisterFcmTokenAsync(long collectorId, CollectorLoginDto dto)
    {
        try
        {
            await _registerDeviceTokenAction!.ExecuteAsync(new RegisterDeviceTokenDto
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
