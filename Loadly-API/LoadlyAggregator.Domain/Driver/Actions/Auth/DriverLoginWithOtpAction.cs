using System;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;
using MfaCodeAttempt = LoadlyAggregator.Domain.Common.Interfaces.MfaCodeAttempt;

namespace LoadlyAggregator.Domain.Driver.Actions.Auth;



public class DriverLoginWithOtpAction : IAction<DriverLoginWithOtpDto, object>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IDriverDocumentRepository _driverDocumentRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMfaCacheService _mfaCacheService;
    private readonly IPermissionService _permissionService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IMediaService _mediaService;
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? _registerDeviceTokenAction;

    public DriverLoginWithOtpAction(
        IDriverRepository driverRepository,
        IDriverDocumentRepository driverDocumentRepository,
        IDocumentRepository documentRepository,
        IJwtTokenService jwtTokenService,
        IMfaCacheService mfaCacheService,
        IPermissionService permissionService,
        IRefreshTokenService refreshTokenService,
        IMediaService mediaService,
        ITripRepository tripRepository,
        IVehicleRepository vehicleRepository,
        Domain.Common.Actions.DeviceToken.RegisterDeviceTokenAction? registerDeviceTokenAction = null)
    {
        _driverRepository = driverRepository;
        _driverDocumentRepository = driverDocumentRepository;
        _documentRepository = documentRepository;
        _jwtTokenService = jwtTokenService;
        _mfaCacheService = mfaCacheService;
        _permissionService = permissionService;
        _refreshTokenService = refreshTokenService;
        _mediaService = mediaService;
        _tripRepository = tripRepository;
        _vehicleRepository = vehicleRepository;
        _registerDeviceTokenAction = registerDeviceTokenAction;
    }

    public async Task<object> ExecuteAsync(DriverLoginWithOtpDto dto)
    {
        ValidateRequest(dto);
        var driver = await _driverRepository.GetByLoginAsync((dto.Login ?? string.Empty).Trim().ToLower());
        UserValidator.ValidateUserExists(driver, "Driver");
        UserValidator.ValidateStatus(driver!.Status);
        _ = await _mfaCacheService.IsRateLimitedAsync(driver.Id, "driver")
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;

        var storedToken = await _mfaCacheService.GetLoginOtpTokenAsync(driver.Id, "driver");
        if (string.IsNullOrWhiteSpace(storedToken) || storedToken != dto.Token)
        {
            await _mfaCacheService.IncrementFailedAttemptsAsync(driver.Id, "driver");
            throw new ValidationException("auth.invalid_or_expired_verification_token");
        }

        var mfaAttempt = await _mfaCacheService.GetMfaCodeAsync(driver.Id, "driver");
        _ = mfaAttempt != null
            ? true
            : await HandleNullMfaAttemptAsync(driver.Id);
        _ = mfaAttempt!.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)
            ? true
            : await HandleInvalidMfaCodeAsync(driver.Id, mfaAttempt);

        await _mfaCacheService.DeleteMfaCodeAsync(driver.Id, "driver");
        await _mfaCacheService.DeleteLoginOtpTokenAsync(driver.Id, "driver");
        await _jwtTokenService.ClearUserRevocationAsync(driver.Id, "driver");

        if (!driver.PhoneVerified)
        {
            driver.PhoneVerified = true;
            await _driverRepository.UpdateAsync(driver);
            await _driverRepository.SaveChangesAsync();
        }

        var loginResult = await CreateLoginResponse(driver, dto.IpAddress, dto.UserAgent, dto.DeviceInfoJson);
        await RegisterFcmTokenIfProvidedAsync(driver.Id, dto);
        return loginResult;
    }

    private async Task<bool> HandleNullMfaAttemptAsync(long userId)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "driver");
        throw new ValidationException("auth.invalid_or_expired_verification_code");
    }

    private async Task<bool> HandleInvalidMfaCodeAsync(long userId, MfaCodeAttempt mfaAttempt)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "driver");
        await (mfaAttempt.FailedAttempts >= 4
            ? _mfaCacheService.DeleteMfaCodeAsync(userId, "driver")
            : Task.CompletedTask);
        throw new ValidationException("auth.invalid_or_expired_verification_code");
    }

    private static void ValidateRequest(DriverLoginWithOtpDto dto)
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
            }
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

    private async Task RegisterFcmTokenIfProvidedAsync(long driverId, DriverLoginWithOtpDto dto)
    {
        await (string.IsNullOrEmpty(dto.FcmToken) || _registerDeviceTokenAction == null
            ? Task.CompletedTask
            : RegisterFcmTokenAsync(driverId, dto));
    }

    private async Task RegisterFcmTokenAsync(long driverId, DriverLoginWithOtpDto dto)
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
