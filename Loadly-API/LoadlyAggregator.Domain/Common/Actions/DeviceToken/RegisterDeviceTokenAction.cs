using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.DTOs.DeviceToken;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Time;
using DeviceTokenEntity = LoadlyAggregator.Domain.Common.Entities.DeviceToken;

namespace LoadlyAggregator.Domain.Common.Actions.DeviceToken;

public class RegisterDeviceTokenAction
{
    private readonly IDeviceTokenRepository _deviceTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterDeviceTokenAction> _logger;
    private readonly TimeProvider _timeProvider;

    public RegisterDeviceTokenAction(
        IDeviceTokenRepository deviceTokenRepository,
        IUnitOfWork unitOfWork,
        ILogger<RegisterDeviceTokenAction> logger,
        TimeProvider timeProvider)
    {
        _deviceTokenRepository = deviceTokenRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    public async Task ExecuteAsync(RegisterDeviceTokenDto dto)
    {
        var (existingToken, staleDuplicate) = await ResolveExistingTokensAsync(dto);

        await (existingToken switch
        {
            null => CreateNewDeviceTokenAsync(dto),
            { IsActive: false } => ReactivateDeviceTokenAsync(existingToken, dto),
            _ => UpdateExistingTokenAsync(existingToken, dto)
        });

        if (staleDuplicate != null)
        {
            await DeactivateStaleDuplicateAsync(staleDuplicate);
        }
    }

    private async Task<(DeviceTokenEntity? Existing, DeviceTokenEntity? StaleDuplicate)> ResolveExistingTokensAsync(
        RegisterDeviceTokenDto dto)
    {
        if (string.IsNullOrEmpty(dto.DeviceId))
        {
            var tokenMatch = await _deviceTokenRepository.GetByUserAndTokenAsync(dto.UserId, dto.UserType, dto.FcmToken);
            return (tokenMatch, null);
        }

        // ponytail: device-id is canonical when FCM rotates; fcm-only row becomes stale duplicate
        var byDeviceId = await _deviceTokenRepository.GetByUserAndDeviceIdAsync(dto.UserId, dto.UserType, dto.DeviceId);
        var byToken = await _deviceTokenRepository.GetByUserAndTokenAsync(dto.UserId, dto.UserType, dto.FcmToken);

        if (byDeviceId != null)
        {
            var staleDuplicate = byToken != null && byToken.Id != byDeviceId.Id ? byToken : null;
            return (byDeviceId, staleDuplicate);
        }

        return (byToken, null);
    }

    private async Task CreateNewDeviceTokenAsync(RegisterDeviceTokenDto dto)
    {
        var deviceToken = new DeviceTokenEntity
        {
            UserId = dto.UserId,
            UserType = dto.UserType,
            FcmToken = dto.FcmToken,
            DeviceId = dto.DeviceId,
            DeviceModel = dto.DeviceModel,
            DeviceOs = dto.DeviceOs,
            AppVersion = dto.AppVersion,
            IsActive = true,
            LastUsedAt = AppUtcTime.UtcNow(_timeProvider),
        };

        await _deviceTokenRepository.AddAsync(deviceToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation(
            "Registered new FCM token for user {UserId} ({UserType})",
            dto.UserId,
            dto.UserType);
    }

    private async Task ReactivateDeviceTokenAsync(DeviceTokenEntity existingToken, RegisterDeviceTokenDto dto)
    {
        ApplyDtoToToken(existingToken, dto);
        existingToken.IsActive = true;
        existingToken.LastUsedAt = AppUtcTime.UtcNow(_timeProvider);
        await _deviceTokenRepository.UpdateAsync(existingToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation(
            "Reactivated FCM token {TokenId} for user {UserId} ({UserType})",
            existingToken.Id,
            existingToken.UserId,
            existingToken.UserType);
    }

    private async Task UpdateExistingTokenAsync(DeviceTokenEntity existingToken, RegisterDeviceTokenDto dto)
    {
        var needsUpdate = existingToken.FcmToken != dto.FcmToken
            || existingToken.DeviceId != dto.DeviceId
            || existingToken.DeviceModel != dto.DeviceModel
            || existingToken.DeviceOs != dto.DeviceOs
            || existingToken.AppVersion != dto.AppVersion;

        await (needsUpdate
            ? UpdateTokenWithNewDataAsync(existingToken, dto)
            : UpdateDeviceTokenLastUsedAsync(existingToken));
    }

    private async Task UpdateTokenWithNewDataAsync(DeviceTokenEntity existingToken, RegisterDeviceTokenDto dto)
    {
        ApplyDtoToToken(existingToken, dto);
        existingToken.LastUsedAt = AppUtcTime.UtcNow(_timeProvider);
        await _deviceTokenRepository.UpdateAsync(existingToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation(
            "Updated FCM token {TokenId} for user {UserId} ({UserType})",
            existingToken.Id,
            existingToken.UserId,
            existingToken.UserType);
    }

    private static void ApplyDtoToToken(DeviceTokenEntity token, RegisterDeviceTokenDto dto)
    {
        token.FcmToken = dto.FcmToken;
        token.DeviceId = dto.DeviceId ?? token.DeviceId;
        token.DeviceModel = dto.DeviceModel ?? token.DeviceModel;
        token.DeviceOs = dto.DeviceOs ?? token.DeviceOs;
        token.AppVersion = dto.AppVersion ?? token.AppVersion;
    }

    private async Task UpdateDeviceTokenLastUsedAsync(DeviceTokenEntity existingToken)
    {
        await _deviceTokenRepository.UpdateLastUsedAsync(existingToken.Id);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task DeactivateStaleDuplicateAsync(DeviceTokenEntity staleDuplicate)
    {
        if (!staleDuplicate.IsActive)
        {
            return;
        }

        await _deviceTokenRepository.DeactivateTokenAsync(staleDuplicate.Id);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation(
            "Deactivated stale FCM duplicate {TokenId} for user {UserId} ({UserType}) after device-id rotation merge",
            staleDuplicate.Id,
            staleDuplicate.UserId,
            staleDuplicate.UserType);
    }
}
