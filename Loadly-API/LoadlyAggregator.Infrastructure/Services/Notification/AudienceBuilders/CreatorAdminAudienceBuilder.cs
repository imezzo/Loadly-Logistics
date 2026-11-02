using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Infrastructure.Services.Notification.AudienceBuilders;

/// <summary>
/// Builds a push + in-app <see cref="NotificationRecipient"/> for the admin who created a trip
/// (the <c>AddedBy</c> field). Returns <c>null</c> when the admin is missing or inactive.
/// </summary>
public class CreatorAdminAudienceBuilder : ICreatorAdminAudienceBuilder
{
    private readonly IAdminRepository _adminRepository;
    private readonly ILogger<CreatorAdminAudienceBuilder> _logger;

    public CreatorAdminAudienceBuilder(
        IAdminRepository adminRepository,
        ILogger<CreatorAdminAudienceBuilder> logger)
    {
        _adminRepository = adminRepository;
        _logger = logger;
    }

    public async Task<NotificationRecipient?> BuildAsync(
        long? addedByAdminId,
        NotificationEventType notificationEventType,
        string title,
        string body,
        Dictionary<string, string>? pushData = null,
        string? pushClickAction = null,
        NotificationResourceType? resourceType = null,
        long? resourceId = null,
        string? clickAction = null)
    {
        if (addedByAdminId is null)
            return null;

        try
        {
            var admin = await _adminRepository.GetByIdAsync(addedByAdminId.Value);
            if (admin is null || admin.Status != UserStatus.ACTIVE)
                return null;

            return new NotificationRecipient
            {
                ModelId = admin.Id,
                ModelType = "Admin",
                NotificationEventType = notificationEventType,
                Domain = NotificationDomain.Admin,
                Channels = new HashSet<NotificationChannel> { NotificationChannel.PUSH, NotificationChannel.IN_APP },
                Title = title,
                Body = body,
                ResourceType = resourceType,
                ResourceId = resourceId,
                ClickAction = clickAction,
                PushData = pushData,
                PushClickAction = pushClickAction
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to build creator-admin recipient (AdminId={AdminId}) for {EventType}.",
                addedByAdminId, notificationEventType);
            return null;
        }
    }
}
