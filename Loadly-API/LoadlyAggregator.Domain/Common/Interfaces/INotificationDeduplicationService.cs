using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

/// <summary>
/// Prevents duplicate notifications within a time window.
/// Uses an atomic "acquire" semantic: returns true only if this is the first send for the key within the window.
/// </summary>
public interface INotificationDeduplicationService
{
    /// <summary>
    /// Attempts to acquire the right to send. Returns true if not a duplicate (proceed with send), false if duplicate (skip).
    /// </summary>
    /// <param name="channel">Notification channel (email, sms, push).</param>
    /// <param name="modelId">Entity ID the notification relates to (e.g. trip ID, company ID).</param>
    /// <param name="modelType">Entity type (e.g. Trip, Company, Driver).</param>
    /// <param name="eventType">Event type enum value.</param>
    /// <param name="recipient">Recipient identifier: email, phone, device token, or "user:{userId}:{userType}" for push-to-user.</param>
    /// <param name="resourceId">Optional resource ID (e.g. trip ID) to distinguish notifications about different resources for the same recipient.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if we should send (not a duplicate); false if duplicate within the window.</returns>
    Task<bool> TryAcquireAsync(
        NotificationChannel channel,
        long modelId,
        string modelType,
        NotificationEventType eventType,
        string recipient,
        long? resourceId = null,
        CancellationToken cancellationToken = default);
}
