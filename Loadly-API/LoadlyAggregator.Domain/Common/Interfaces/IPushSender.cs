using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Notifications;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IPushSender
{
    /// <summary>Typed overload used by the notification pipeline. No preference/dedup/in-app storage -- the pipeline handles those.</summary>
    Task SendAsync(PushUserMessage message, CancellationToken cancellationToken = default);

    /// <summary>Legacy overload kept for backward compatibility.</summary>
    Task<bool> SendToDeviceAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null, string? imageUrl = null, string? clickAction = null, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null);

    /// <summary>Legacy overload kept for backward compatibility with auth/OTP/MFA and broadcast queue delivery.</summary>
    Task SendToUserAsync(long modelId, string modelType, string title, string body, Dictionary<string, string>? data = null, string? clickAction = null, NotificationDomain domain = NotificationDomain.Default, string? eventType = null, long? broadcastNotificationId = null);
}
