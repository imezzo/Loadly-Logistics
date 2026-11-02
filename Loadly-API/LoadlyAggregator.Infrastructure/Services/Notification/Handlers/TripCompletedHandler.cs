using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Infrastructure.Services.Notification.AudienceBuilders;
using LoadlyAggregator.Shared.Enums;
using Lang = LoadlyAggregator.Shared.Constants.NotificationLanguage;

namespace LoadlyAggregator.Infrastructure.Services.Notification.Handlers;

/// <summary>
/// Builds recipients for <see cref="TripCompletedByDriverNotificationData"/>:
/// 1. The admin who created the trip (trip.DispatcherId) receives push + in-app.
/// 2. All active users of the trip's company receive push + in-app.
/// Resolves CompanyId and DispatcherId from the trip entity — action only passes TripId.
/// </summary>
public class TripCompletedHandler : INotificationHandler<TripCompletedByDriverNotificationData>
{
    private readonly ITripRepository _tripRepository;
    private readonly INotificationTemplateService _templateService;
    private readonly ICreatorAdminAudienceBuilder _creatorAdminBuilder;
    private readonly ICompanyUsersAudienceBuilder _companyUsersBuilder;
    private readonly ILogger<TripCompletedHandler> _logger;

    public TripCompletedHandler(
        ITripRepository tripRepository,
        INotificationTemplateService templateService,
        ICreatorAdminAudienceBuilder creatorAdminBuilder,
        ICompanyUsersAudienceBuilder companyUsersBuilder,
        ILogger<TripCompletedHandler> logger)
    {
        _tripRepository = tripRepository;
        _templateService = templateService;
        _creatorAdminBuilder = creatorAdminBuilder;
        _companyUsersBuilder = companyUsersBuilder;
        _logger = logger;
    }

    public async Task<IReadOnlyList<NotificationRecipient>> BuildRecipientsAsync(
        TripCompletedByDriverNotificationData data, CancellationToken cancellationToken = default)
    {
        var trip = await _tripRepository.GetByIdAsync(data.TripId);
        if (trip is null)
        {
            _logger.LogWarning("Trip {TripId} not found. Skipping TripCompleted notification.", data.TripId);
            return [];
        }

        if (trip.Contract is null)
        {
            _logger.LogWarning("Trip {TripId} has no contract. Skipping TripCompleted notification.", data.TripId);
            return [];
        }

        string eventKey = NotificationEventType.TRIP_COMPLETED.ToString();

        var placeholders = NotificationPlaceholderBuilder.ForTrip(data.TripId, trip.StartDate, trip.EndDate);
        string title = await _templateService.ResolveAsync(
            $"{eventKey}_company_subject", Lang.Default, placeholders);
        string body = await _templateService.ResolveAsync(
            $"{eventKey}_company_body", Lang.Default, placeholders);

        var pushData = new Dictionary<string, string>
        {
            ["resourceId"] = data.TripId.ToString(),
            ["resourceType"] = "TRIP",
            ["eventType"] = eventKey
        };

        var recipients = new List<NotificationRecipient>();

        var adminRecipient = await _creatorAdminBuilder.BuildAsync(
            trip.DispatcherId, NotificationEventType.TRIP_COMPLETED, title, body,
            pushData: pushData, pushClickAction: "open_trip",
            resourceType: NotificationResourceType.TRIP, resourceId: data.TripId, clickAction: "open_trip");
        if (adminRecipient is not null)
            recipients.Add(adminRecipient);

        var companyRecipients = await _companyUsersBuilder.BuildAsync(
            trip.Contract.CompanyId, NotificationEventType.TRIP_COMPLETED, title, body,
            pushData: pushData, pushClickAction: "open_trip",
            resourceType: NotificationResourceType.TRIP, resourceId: data.TripId, clickAction: "open_trip");
        recipients.AddRange(companyRecipients);

        return recipients;
    }
}
