using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Notifications;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IEmailSender
{
    /// <summary>Typed overload used by the notification pipeline. No preference/dedup -- the pipeline handles those.</summary>
    Task<bool> SendAsync(EmailMessage message, CancellationToken cancellationToken = default);

    /// <summary>Legacy overload kept for backward compatibility with auth/OTP/MFA flows.</summary>
    Task<bool> SendAsync(string from, string fromName, string to, string toName, string subject, string body, bool isHtml = false, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null);
}
