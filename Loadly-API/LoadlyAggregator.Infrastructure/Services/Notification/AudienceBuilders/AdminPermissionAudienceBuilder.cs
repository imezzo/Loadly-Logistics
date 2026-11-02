using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Infrastructure.Services.Notification.AudienceBuilders;


public class AdminPermissionAudienceBuilder : IAdminPermissionAudienceBuilder
{
    private readonly IPermissionService _permissionService;
    private readonly IAdminRepository _adminRepository;
    private readonly INotificationTemplateService _templateService;
    private readonly ILogger<AdminPermissionAudienceBuilder> _logger;

    public AdminPermissionAudienceBuilder(
        IPermissionService permissionService,
        IAdminRepository adminRepository,
        INotificationTemplateService templateService,
        ILogger<AdminPermissionAudienceBuilder> logger)
    {
        _permissionService = permissionService;
        _adminRepository = adminRepository;
        _templateService = templateService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<NotificationRecipient>> BuildForTripEventAsync(
        string permissionName,
        string permissionGuard,
        NotificationEventType notificationEventType,
        Trip trip,
        Company company,
        string emailHtmlHeading,
        NotificationResourceType? resourceType = null,
        long? resourceId = null,
        string? clickAction = null,
        CancellationToken cancellationToken = default)
    {
        var definition = NotificationDefinitionRegistry.TryGetDefinition(notificationEventType);
        var allowedChannels = definition?.AudienceChannels
            .Where(ac => ac.ModelType.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            .Select(ac => ac.Channel)
            .ToHashSet()
            ?? new HashSet<NotificationChannel> { NotificationChannel.IN_APP };

        if (allowedChannels.Count == 0)
        {
            _logger.LogWarning("No Admin channels defined in registry for {EventType}. Skipping.", notificationEventType);
            return [];
        }

        var adminIds = await _permissionService.GetModelIdsWithPermissionAsync(
            permissionName, permissionGuard, "Admin", cancellationToken);

        if (adminIds.Count == 0)
            return [];

        var admins = await _adminRepository.Query()
            .Where(a => adminIds.Contains(a.Id) && a.Status == UserStatus.ACTIVE)
            .Select(a => new { a.Id, a.Email, a.Phone })
            .ToListAsync(cancellationToken);

        if (admins.Count == 0)
            return [];

        string companyName = !string.IsNullOrWhiteSpace(company.CompanyDetail?.NameEn)
            ? company.CompanyDetail.NameEn
            : company.FullName;

        string eventKey = notificationEventType.ToString();
        var placeholders = NotificationPlaceholderBuilder.ForTrip(trip.Id, trip.StartDate, trip.EndDate);
        placeholders["companyName"] = companyName;

        string title = await _templateService.ResolveAsync($"{eventKey}_subject", NotificationLanguage.Default, placeholders);
        string body = await _templateService.ResolveAsync($"{eventKey}_body", NotificationLanguage.Default, placeholders);

        bool includeEmail = allowedChannels.Contains(NotificationChannel.EMAIL);

        string? emailSubject = null;
        string? emailBody = null;
        string? htmlBody = null;
        if (includeEmail)
        {
            emailSubject = await _templateService.ResolveAsync($"{eventKey}_subject", NotificationLanguage.Email, placeholders);
            emailBody = await _templateService.ResolveAsync($"{eventKey}_body", NotificationLanguage.Email, placeholders);
            htmlBody = $@"
            <h2>{emailHtmlHeading}</h2>
            <p>{emailBody}</p>
            <br>
            <p>Best regards,<br>Loadly Aggregator Team</p>
        ";
        }

        var recipients = new List<NotificationRecipient>(admins.Count);
        foreach (var admin in admins)
        {
            var channels = new HashSet<NotificationChannel>(allowedChannels);
            if (channels.Contains(NotificationChannel.EMAIL) && string.IsNullOrWhiteSpace(admin.Email))
                channels.Remove(NotificationChannel.EMAIL);

            recipients.Add(new NotificationRecipient
            {
                ModelId = admin.Id,
                ModelType = "Admin",
                NotificationEventType = notificationEventType,
                Domain = NotificationDomain.Admin,
                Channels = channels,
                Title = title,
                Body = body,
                ResourceType = resourceType,
                ResourceId = resourceId,
                ClickAction = clickAction,
                EmailAddress = includeEmail ? admin.Email : null,
                EmailSubject = emailSubject,
                EmailHtmlBody = htmlBody
            });
        }

        return recipients;
    }
}
