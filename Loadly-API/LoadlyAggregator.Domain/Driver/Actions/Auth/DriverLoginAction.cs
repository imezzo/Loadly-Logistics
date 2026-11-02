using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Auth;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Utilities;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Driver.Actions.Auth;

public class DriverLoginAction : IAction<DriverLoginDto, object>
{
    private const int FirstLoginResetCodeExpirationMinutes = 60;

    private readonly IDriverRepository _driverRepository;
    private readonly IDriverDocumentRepository _driverDocumentRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly AuthService _authService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPermissionService _permissionService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly INotificationTransport _notificationTransport;
    private readonly IPasswordResetCacheService _passwordResetCacheService;
    private readonly IMediaService _mediaService;
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? _registerDeviceTokenAction;

    public DriverLoginAction(
        IDriverRepository driverRepository,
        IDriverDocumentRepository driverDocumentRepository,
        IDocumentRepository documentRepository,
        AuthService authService,
        IJwtTokenService jwtTokenService,
        IPermissionService permissionService,
        IRefreshTokenService refreshTokenService,
        INotificationTransport notificationTransport,
        IPasswordResetCacheService passwordResetCacheService,
        IMediaService mediaService,
        ITripRepository tripRepository,
        IVehicleRepository vehicleRepository,
        Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? registerDeviceTokenAction = null)
    {
        _driverRepository = driverRepository;
        _driverDocumentRepository = driverDocumentRepository;
        _documentRepository = documentRepository;
        _authService = authService;
        _jwtTokenService = jwtTokenService;
        _permissionService = permissionService;
        _refreshTokenService = refreshTokenService;
        _notificationTransport = notificationTransport;
        _passwordResetCacheService = passwordResetCacheService;
        _mediaService = mediaService;
        _tripRepository = tripRepository;
        _vehicleRepository = vehicleRepository;
        _registerDeviceTokenAction = registerDeviceTokenAction;
    }

    public async Task<object> ExecuteAsync(DriverLoginDto dto)
    {
        var driver = await _driverRepository.GetByLoginAsync((dto.Login ?? string.Empty).Trim().ToLower());
        UserValidator.ValidateUserExists(driver, "Driver");
        UserValidator.ValidateStatus(driver!.Status);

        return HasNoPassword(driver)
            ? await HandleFirstTimeLoginNoPasswordAsync(driver, dto.IpAddress, dto.UserAgent, dto.DeviceInfoJson)
            : await HandleLoginWithPasswordAsync(driver, dto);
    }

    private async Task<object> HandleLoginWithPasswordAsync(
        Domain.Common.Entities.Driver driver,
        DriverLoginDto dto)
    {
        _authService.ValidateCredentials(driver, dto.Password);

        await _jwtTokenService.ClearUserRevocationAsync(driver.Id, "driver");

        if (driver.MfaChannel != CommunicationChannel.None)
        {
            return await CreateMfaRequiredResponseAsync(driver);
        }

        var loginResult = await CreateLoginResponseAsync(driver, dto.IpAddress, dto.UserAgent, dto.DeviceInfoJson);
        await RegisterFcmTokenIfProvidedAsync(driver.Id, dto);
        return loginResult;
    }

    private static bool HasNoPassword(Domain.Common.Entities.Driver driver) =>
        string.IsNullOrWhiteSpace(driver.Password);

    private async Task<object> HandleFirstTimeLoginNoPasswordAsync(
        Domain.Common.Entities.Driver driver,
        string? ipAddress,
        string? userAgent,
        string? deviceInfoJson)
    {
        _ = await _passwordResetCacheService.IsRateLimitedAsync("driver", driver.Phone)
            ? throw new TooManyRequestsException("auth.too_many_attempts")
            : true;

        var code = CodeGenerator.GenerateNumericCode(6);

        await Task.WhenAll(
            _passwordResetCacheService.StoreResetAttemptAsync(
                "driver",
                driver.Phone,
                driver.Id,
                code,
                CommunicationChannel.Sms,
                FirstLoginResetCodeExpirationMinutes),
            _jwtTokenService.ClearUserRevocationAsync(driver.Id, "driver"));

        await _notificationTransport.SendSmsAsync(
            driver.Phone,
            code,
            NotificationDomain.Driver,
            driver.Id,
            "Driver");

        return new
        {
            Redirect = "forget_password",
            Message = "auth.forget_password_sms_sent"
        };
    }
    private async Task<object> CreateMfaRequiredResponseAsync(Domain.Common.Entities.Driver driver)
    {
        var vehicles = await _vehicleRepository.GetByDriverIdAsync(driver.Id);
        var documents = await BuildDocumentsAsync(driver.Id);

        return new
        {
            AccessToken = _jwtTokenService.GenerateTemporaryMfaToken(driver.Id, "driver", driver.Email),
            RefreshToken = string.Empty,
            ExpiresIn = 300,
            User = new
            {
                Id = driver.Id,
                FullName = driver.FullName,
                UserName = driver.UserName,
                Email = driver.Email,
                Phone = driver.Phone,
                UserType = "driver",
                MfaChannel = (int)driver.MfaChannel,
                ProfilePicture = !string.IsNullOrWhiteSpace(driver.ProfilePicture)
                    ? _mediaService.Read($"driver/{driver.Id}/profile-picture/{driver.ProfilePicture}")
                    : null,
                DriverRating = (decimal?)null,
                TripsCount = await _tripRepository.CountAsync(t => t.DriverId == driver.Id),
                BiometricFingerprint = driver.BiometricFingerprint,
                IsOnline = (driver.WorkingStatus ?? DriverWorkingStatus.Offline) == DriverWorkingStatus.Available,
                Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(driver),
                Vehicles = vehicles.Select(v => new
                {
                    Id = v.Id,
                    PlateNo = v.PlateNo,
                    PlateLetters = v.PlateLetters,
                    MakeYear = v.MakeYear,
                    VinNo = v.VinNo,
                    VehicleTypeId = v.VehicleTypeId,
                    Status = (int)v.Status,
                    VehicleType = v.VehicleType != null ? new { Id = v.VehicleType.Id, NameEn = v.VehicleType.NameEn, NameAr = v.VehicleType.NameAr } : null,
                    Color = v.Color != null ? new { Id = v.Color.Id, NameEn = v.Color.NameEn, NameAr = v.Color.NameAr } : null
                }).ToList(),
                Documents = documents
            },
            Redirect = "mfa"
        };
    }
    private async Task<object> CreateLoginResponseAsync(
        Domain.Common.Entities.Driver driver,
        string? ipAddress = null,
        string? userAgent = null,
        string? deviceInfoJson = null)
    {
        return await CreateLoginResponseWithRedirectAsync(driver, ipAddress, userAgent, "home", deviceInfoJson);
    }

    private async Task<object> CreateLoginResponseWithRedirectAsync(
        Domain.Common.Entities.Driver driver,
        string? ipAddress,
        string? userAgent,
        string redirect,
        string? deviceInfoJson = null)
    {
        var (refreshTokenEntity, plainRefreshToken) = await _refreshTokenService.CreateRefreshTokenAsync(
            driver.Id,
            "Driver",
            deviceInfo: deviceInfoJson,
            ipAddress: ipAddress,
            userAgent: userAgent);

        var vehicles = await _vehicleRepository.GetByDriverIdAsync(driver.Id);
        var documents = await BuildDocumentsAsync(driver.Id);

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
                ProfilePicture = !string.IsNullOrWhiteSpace(driver.ProfilePicture)
                    ? _mediaService.Read($"driver/{driver.Id}/profile-picture/{driver.ProfilePicture}")
                    : null,
                DriverRating = (decimal?)null,
                TripsCount = await _tripRepository.CountAsync(t => t.DriverId == driver.Id),
                BiometricFingerprint = driver.BiometricFingerprint,
                IsOnline = (driver.WorkingStatus ?? DriverWorkingStatus.Offline) == DriverWorkingStatus.Available,
                Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(driver),
                Vehicles = vehicles.Select(v => new
                {
                    Id = v.Id,
                    PlateNo = v.PlateNo,
                    PlateLetters = v.PlateLetters,
                    MakeYear = v.MakeYear,
                    VinNo = v.VinNo,
                    VehicleTypeId = v.VehicleTypeId,
                    Status = (int)v.Status,
                    VehicleType = v.VehicleType != null ? new { Id = v.VehicleType.Id, NameEn = v.VehicleType.NameEn, NameAr = v.VehicleType.NameAr } : null,
                    Color = v.Color != null ? new { Id = v.Color.Id, NameEn = v.Color.NameEn, NameAr = v.Color.NameAr } : null
                }).ToList(),
                Documents = documents
            },
            Redirect = redirect
        };
    }

    private async Task<List<object>> BuildDocumentsAsync(long driverId)
    {
        var (documentsList, _) = await _driverDocumentRepository.ListAsync(driverId: driverId, pageSize: 100);
        var documents = documentsList.ToList();
        var result = new List<object>();

        foreach (var doc in documents)
        {
            var document = await _documentRepository.GetByIdAsync(doc.DocumentId);
            var fileUrl = (string?)null;
            if (document != null && !string.IsNullOrWhiteSpace(doc.Path))
            {
                var documentNameEn = document.NameEn.ToLower().Replace(" ", "-");
                fileUrl = _mediaService.Read($"driver/{driverId}/{documentNameEn}/{doc.Path}");
            }

            result.Add(new
            {
                Id = doc.Id,
                DocumentId = doc.DocumentId,
                FileUrl = fileUrl,
                ExpiryDate = doc.ExpiryDate,
                Status = (int)doc.Status,
                Document = document != null ? new { Id = document.Id, NameEn = document.NameEn, NameAr = document.NameAr } : null
            });
        }

        return result;
    }

    private async Task RegisterFcmTokenIfProvidedAsync(long driverId, DriverLoginDto dto)
    {
        await (string.IsNullOrEmpty(dto.FcmToken) || _registerDeviceTokenAction == null
            ? Task.CompletedTask
            : RegisterFcmTokenAsync(driverId, dto));
    }

    private async Task RegisterFcmTokenAsync(long driverId, DriverLoginDto dto)
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
