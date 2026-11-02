using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

/// <summary>
/// Low-level, channel-specific transport for transactional messages (OTP, password-reset, manual push).
/// Callers choose the channel explicitly; no preference gate or deduplication.
/// For business-event notifications use <see cref="Notifications.INotificationEventDispatcher"/> instead.
/// Prefer injecting <see cref="IEmailSender"/>, <see cref="ISmsSender"/>, or <see cref="IPushSender"/>
/// when only one channel is needed (ISP).
/// </summary>
public interface INotificationTransport
{
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null);

    Task<bool> SendEmailAsync(string to, string toName, string subject, string body, bool isHtml = false, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null);

    Task<bool> SendEmailAsync(string from, string fromName, string to, string toName, string subject, string body, bool isHtml = false, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null);

    Task<bool> SendSmsAsync(string phoneNumber, string message, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null);

    Task<bool> SendPushNotificationAsync(string deviceToken, string title, string body, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null);

    Task<bool> SendPushNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string>? data, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null);

    Task<bool> SendPushNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string>? data, string? imageUrl, string? clickAction, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null);

    Task SendPushToUserAsync(long modelId, string modelType, string title, string body, Dictionary<string, string>? data = null, string? clickAction = null, NotificationDomain domain = NotificationDomain.Default, string? eventType = null, long? broadcastNotificationId = null);
}

