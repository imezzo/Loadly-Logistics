using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.DTOs.DeviceToken;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Common.Actions.DeviceToken;

public class DeactivateDeviceTokensAction
{
    private readonly IDeviceTokenRepository _deviceTokenRepository;
    private readonly ILogger<DeactivateDeviceTokensAction> _logger;

    public DeactivateDeviceTokensAction(
        IDeviceTokenRepository deviceTokenRepository,
        ILogger<DeactivateDeviceTokensAction> logger)
    {
        _deviceTokenRepository = deviceTokenRepository;
        _logger = logger;
    }

    public async Task ExecuteAsync(DeactivateDeviceTokensDto dto)
    {
        if (dto.RevokeAll)
        {
            await _deviceTokenRepository.DeactivateUserTokensAsync(dto.UserId, dto.UserType);
            _logger.LogInformation(
                "Deactivated all device tokens for {UserType} {UserId} on logout.",
                dto.UserType,
                dto.UserId);
            return;
        }

        if (!string.IsNullOrWhiteSpace(dto.FcmToken))
        {
            var token = await _deviceTokenRepository.GetByUserAndTokenAsync(dto.UserId, dto.UserType, dto.FcmToken);
            await TryDeactivateTokenAsync(token, dto.UserType, dto.UserId, "fcm token");
        }

        if (!string.IsNullOrWhiteSpace(dto.DeviceId))
        {
            var token = await _deviceTokenRepository.GetByUserAndDeviceIdAsync(dto.UserId, dto.UserType, dto.DeviceId);
            await TryDeactivateTokenAsync(token, dto.UserType, dto.UserId, "device id");
        }
    }

    private async Task TryDeactivateTokenAsync(
        Domain.Common.Entities.DeviceToken? token,
        string userType,
        long userId,
        string matchedBy)
    {
        if (token == null || !token.IsActive)
        {
            return;
        }

        await _deviceTokenRepository.DeactivateTokenAsync(token.Id);
        _logger.LogInformation(
            "Deactivated device token for {UserType} {UserId} on logout (matched by {MatchedBy}).",
            userType,
            userId,
            matchedBy);
    }
}
