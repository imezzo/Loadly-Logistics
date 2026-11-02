using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Domain.Common.DTOs.DeviceToken;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Auth;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Admin.Actions.Auth;




public class AdminLoginAction : IAction<AdminLoginDto, object>
{
    private readonly IAdminRepository _adminRepository;
    private readonly AuthService _authService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPermissionService _permissionService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ILogger<AdminLoginAction> _logger;
    private readonly Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? _registerDeviceTokenAction;

    public AdminLoginAction(
        IAdminRepository adminRepository,
        AuthService authService,
        IJwtTokenService jwtTokenService,
        IPermissionService permissionService,
        IRefreshTokenService refreshTokenService,
        ILogger<AdminLoginAction> logger,
        Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? registerDeviceTokenAction = null)
    {
        _adminRepository = adminRepository;
        _authService = authService;
        _jwtTokenService = jwtTokenService;
        _permissionService = permissionService;
        _refreshTokenService = refreshTokenService;
        _logger = logger;
        _registerDeviceTokenAction = registerDeviceTokenAction;
    }

    public async Task<object> ExecuteAsync(AdminLoginDto dto)
    {
        _logger.LogInformation(
            "Admin login attempt started for login: {Login}, IP: {IpAddress}, UserAgent: {UserAgent}",
            dto.Login, dto.IpAddress, dto.UserAgent);
        var admin = await _adminRepository.GetByLoginAsync(dto.Login.Trim().ToLower());
        _authService.ValidateCredentials(admin, dto.Password);
        _logger.LogInformation(
            "Admin credentials validated successfully for AdminId: {AdminId}, Username: {Username}",
            admin!.Id, admin.UserName);

        await _jwtTokenService.ClearUserRevocationAsync(admin.Id, "admin");

        if (admin.MfaChannel != CommunicationChannel.None)
        {
            return await CreateMfaRequiredResponseAsync(admin);
        }

        var loginResult = await CreateLoginResponseAsync(admin, dto.IpAddress, dto.UserAgent, dto.DeviceInfoJson);
        await RegisterFcmTokenIfProvidedAsync(admin.Id, dto);
        return loginResult;
    }

    private async Task<object> CreateMfaRequiredResponseAsync(Domain.Common.Entities.Admin admin)
    {
        var permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(admin);
        _logger.LogInformation(
            "MFA temporary token generated for AdminId: {AdminId}, Token expires in 5 minutes",
            admin.Id);
        return new
        {
            AccessToken = _jwtTokenService.GenerateTemporaryMfaToken(admin.Id, "admin", admin.Email),
            RefreshToken = string.Empty,
            ExpiresIn = 300,
            User = new
            {
                Id = admin.Id,
                FullName = admin.FullName,
                UserName = admin.UserName,
                Email = admin.Email,
                Phone = admin.Phone,
                UserType = "admin",
                MfaChannel = (int)admin.MfaChannel,
                Permissions = permissions
            }
        };
    }

    private async Task<object> CreateLoginResponseAsync(
        Domain.Common.Entities.Admin admin,
        string? ipAddress = null,
        string? userAgent = null,
        string? deviceInfoJson = null)
    {
        var permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(admin);
        var (refreshTokenEntity, plainRefreshToken) = await _refreshTokenService.CreateRefreshTokenAsync(
            admin.Id,
            "Admin",
            deviceInfo: deviceInfoJson,
            ipAddress: ipAddress,
            userAgent: userAgent);
        _logger.LogInformation(
            "Access token generated for AdminId: {AdminId}, Token expires in 1 hour",
            admin.Id);
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
                Permissions = permissions
            }
        };
    }

    private async Task RegisterFcmTokenIfProvidedAsync(long adminId, AdminLoginDto dto)
    {
        await (string.IsNullOrEmpty(dto.FcmToken) || _registerDeviceTokenAction == null
            ? Task.CompletedTask
            : RegisterFcmTokenAsync(adminId, dto));
    }

    private async Task RegisterFcmTokenAsync(long adminId, AdminLoginDto dto)
    {
        try
        {
            await _registerDeviceTokenAction!.ExecuteAsync(new RegisterDeviceTokenDto
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
