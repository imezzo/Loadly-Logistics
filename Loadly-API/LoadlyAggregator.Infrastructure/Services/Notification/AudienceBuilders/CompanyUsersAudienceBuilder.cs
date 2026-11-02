using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Infrastructure.Services.Notification.AudienceBuilders;

/// <summary>
/// Builds <see cref="NotificationRecipient"/>s for every active user that belongs to a company
/// (the company record itself <em>plus</em> sub-users whose <c>CompanyId</c> matches).
/// Default channels: Push + InApp.
/// </summary>
public class CompanyUsersAudienceBuilder : ICompanyUsersAudienceBuilder
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogger<CompanyUsersAudienceBuilder> _logger;

    public CompanyUsersAudienceBuilder(
        ICompanyRepository companyRepository,
        ILogger<CompanyUsersAudienceBuilder> logger)
    {
        _companyRepository = companyRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<NotificationRecipient>> BuildAsync(
        long companyId,
        NotificationEventType notificationEventType,
        string title,
        string body,
        HashSet<NotificationChannel>? channels = null,
        Dictionary<string, string>? pushData = null,
        string? pushClickAction = null,
        NotificationResourceType? resourceType = null,
        long? resourceId = null,
        string? clickAction = null)
    {
        var defaultChannels = channels
            ?? new HashSet<NotificationChannel> { NotificationChannel.PUSH, NotificationChannel.IN_APP };

        try
        {
            var companyUsers = await _companyRepository.Query()
                .Where(c => (c.Id == companyId || c.CompanyId == companyId)
                    && c.Status == UserStatus.ACTIVE)
                .ToListAsync();

            return companyUsers.Select(user => new NotificationRecipient
            {
                ModelId = user.Id,
                ModelType = "Company",
                NotificationEventType = notificationEventType,
                Domain = NotificationDomain.Company,
                Channels = defaultChannels,
                Title = title,
                Body = body,
                ResourceType = resourceType,
                ResourceId = resourceId,
                ClickAction = clickAction,
                PushData = pushData,
                PushClickAction = pushClickAction
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to build company user recipients for {EventType}, company {CompanyId}.",
                notificationEventType, companyId);
            return [];
        }
    }
}
