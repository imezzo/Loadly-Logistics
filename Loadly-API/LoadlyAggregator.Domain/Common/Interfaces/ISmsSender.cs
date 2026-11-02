using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Notifications;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ISmsSender
{
    /// <summary>Typed overload used by the notification pipeline. No preference/dedup -- the pipeline handles those.</summary>
    Task<bool> SendAsync(SmsMessage message, CancellationToken cancellationToken = default);

    /// <summary>Legacy overload kept for backward compatibility with auth/OTP/MFA flows.</summary>
    Task<bool> SendAsync(string phoneNumber, string message, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null);
}
