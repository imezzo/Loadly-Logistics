using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Admin.DTOs.Logs;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Extensions;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Settings.PushNotifications;

public class SendPushNotificationAction
{
    private readonly INotificationTransport _notificationTransport;
    private readonly ILogger<SendPushNotificationAction> _logger;

    public SendPushNotificationAction(
        INotificationTransport notificationTransport,
        ILogger<SendPushNotificationAction> logger)
    {
        _notificationTransport = notificationTransport;
        _logger = logger;
    }

    public async Task<bool> ExecuteAsync(SendPushNotificationDto dto)
    {
        _logger.LogInformation(
            "Sending push notification to device {DeviceToken} with title: {Title} in domain: {Domain}",
            dto.DeviceToken,
            dto.Title,
            dto.Domain);

        var result = await _notificationTransport.SendPushNotificationAsync(
            dto.DeviceToken,
            dto.Title,
            dto.Body,
            dto.Data,
            dto.ImageUrl,
            dto.ClickAction,
            dto.Domain.ToNotificationDomain());

        _logger.LogInformation(
            "Push notification queued successfully to device {DeviceToken}",
            dto.DeviceToken);

        return result;
    }
}
