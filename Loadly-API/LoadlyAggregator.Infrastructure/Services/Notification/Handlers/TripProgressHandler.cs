using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Infrastructure.Services.Notification.AudienceBuilders;
using Lang = LoadlyAggregator.Shared.Constants.NotificationLanguage;

namespace LoadlyAggregator.Infrastructure.Services.Notification.Handlers;

/// <summary>
/// Single handler for all driver trip-progress events (start → arrive → load → unload → depart).
/// Recipients: the admin who created the trip (push + in-app) and company users (push + in-app).
/// Resolves CompanyId and DispatcherId from the trip entity — actions only pass TripId.
/// </summary>
public class TripProgressHandler : INotificationHandler<TripProgressNotificationData>
{
    private readonly ITripRepository _tripRepository;
    private readonly INotificationTemplateService _templateService;
    private readonly ICreatorAdminAudienceBuilder _creatorAdminBuilder;
    private readonly ICompanyUsersAudienceBuilder _companyUsersBuilder;
    private readonly ILogger<TripProgressHandler> _logger;

    public TripProgressHandler(
        ITripRepository tripRepository,
        INotificationTemplateService templateService,
        ICreatorAdminAudienceBuilder creatorAdminBuilder,
        ICompanyUsersAudienceBuilder companyUsersBuilder,
        ILogger<TripProgressHandler> logger)
    {
        _tripRepository = tripRepository;
        _templateService = templateService;
        _creatorAdminBuilder = creatorAdminBuilder;
        _companyUsersBuilder = companyUsersBuilder;
        _logger = logger;
    }

    public async Task<IReadOnlyList<NotificationRecipient>> BuildRecipientsAsync(
        TripProgressNotificationData data, CancellationToken cancellationToken = default)
    {
        var trip = await _tripRepository.GetByIdAsync(data.TripId);
        if (trip is null)
        {
            _logger.LogWarning("Trip {TripId} not found. Skipping {EventType} notification.",
                data.TripId, data.NotificationEventType);
            return [];
        }

        if (trip.Contract is null)
        {
            _logger.LogWarning("Trip {TripId} has no contract. Skipping {EventType} notification.",
                data.TripId, data.NotificationEventType);
            return [];
        }

        string eventKey = data.NotificationEventType.ToString();

        var placeholders = NotificationPlaceholderBuilder.ForTripProgress(
            data.TripId, trip.StartDate, trip.EndDate, data.WaypointName);

        string title = await _templateService.ResolveAsync(
            $"{eventKey}_subject", Lang.Default, placeholders);
        string body = await _templateService.ResolveAsync(
            $"{eventKey}_body", Lang.Default, placeholders);

        bool isWaypointEvent = data.WaypointId.HasValue;
        var resourceType = isWaypointEvent ? NotificationResourceType.TRIP_WAYPOINT : NotificationResourceType.TRIP;
        long resourceId = data.TripId;

        var pushData = new Dictionary<string, string>
        {
            ["resourceId"] = resourceId.ToString(),
            ["resourceType"] = resourceType.ToString(),
            ["eventType"] = eventKey
        };

        var recipients = new List<NotificationRecipient>();

        var adminRecipient = await _creatorAdminBuilder.BuildAsync(
            trip.DispatcherId, data.NotificationEventType, title, body,
            pushData: pushData, pushClickAction: "open_trip",
            resourceType: resourceType, resourceId: resourceId, clickAction: "open_trip");
        if (adminRecipient is not null)
            recipients.Add(adminRecipient);

        var companyRecipients = await _companyUsersBuilder.BuildAsync(
            trip.Contract.CompanyId, data.NotificationEventType, title, body,
            pushData: pushData, pushClickAction: "open_trip",
            resourceType: resourceType, resourceId: resourceId, clickAction: "open_trip");
        recipients.AddRange(companyRecipients);

        return recipients;
    }
}
