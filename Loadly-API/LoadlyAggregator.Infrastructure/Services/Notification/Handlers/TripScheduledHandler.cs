using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.Channels;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Infrastructure.Services.Notification.AudienceBuilders;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Enums;
using Lang = LoadlyAggregator.Shared.Constants.NotificationLanguage;

namespace LoadlyAggregator.Infrastructure.Services.Notification.Handlers;

/// <summary>
/// Builds recipients for <see cref="TripScheduledNotificationData"/>:
/// 1. Company receives email + push + in-app (when company is present).
/// 2. Driver receives push + in-app (when driver is present).
/// </summary>
public class TripScheduledHandler : INotificationHandler<TripScheduledNotificationData>
{
    private readonly ITripRepository _tripRepository;
    private readonly ICompanyEmailAudienceBuilder _companyBuilder;
    private readonly INotificationTemplateService _templateService;
    private readonly ILogger<TripScheduledHandler> _logger;

    public TripScheduledHandler(
        ITripRepository tripRepository,
        ICompanyEmailAudienceBuilder companyBuilder,
        INotificationTemplateService templateService,
        ILogger<TripScheduledHandler> logger)
    {
        _tripRepository = tripRepository;
        _companyBuilder = companyBuilder;
        _templateService = templateService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<NotificationRecipient>> BuildRecipientsAsync(
        TripScheduledNotificationData data, CancellationToken cancellationToken = default)
    {
        var trip = await _tripRepository.GetByIdAsync(data.TripId);
        if (trip is null)
        {
            _logger.LogWarning("Trip {TripId} not found. Skipping TripScheduled notification.", data.TripId);
            return [];
        }

        var recipients = new List<NotificationRecipient>();

        if (data.CompanyId.HasValue)
            await AddCompanyRecipientAsync(recipients, data.CompanyId.Value, data.TripId, trip.StartDate, trip.EndDate);

        if (data.DriverId.HasValue && data.DriverPushKind.HasValue)
            await AddDriverRecipientAsync(recipients, data.TripId, trip.StartDate, trip.EndDate, data.DriverId.Value, data.DriverPushKind.Value);

        return recipients;
    }

    private async Task AddCompanyRecipientAsync(
        List<NotificationRecipient> recipients,
        long companyId,
        long tripId,
        DateTime? startDate,
        DateTime? endDate)
    {
        string eventKey = NotificationEventType.TRIP_SCHEDULED.ToString();

        var pushData = new Dictionary<string, string>
        {
            ["resourceId"] = tripId.ToString(),
            ["resourceType"] = "TRIP",
            ["eventType"] = eventKey
        };

        var recipient = await _companyBuilder.BuildAsync(
            companyId, tripId, NotificationEventType.TRIP_SCHEDULED,
            additionalChannels: [NotificationChannel.PUSH],
            pushData: pushData,
            pushClickAction: "open_trip",
            resourceType: NotificationResourceType.TRIP, resourceId: tripId, clickAction: "open_trip");

        if (recipient is not null)
            recipients.Add(recipient);
    }

    private async Task AddDriverRecipientAsync(
        List<NotificationRecipient> recipients,
        long tripId,
        DateTime? startDate,
        DateTime? endDate,
        long driverId,
        TripScheduleDriverPushKind kind)
    {
        NotificationDefinitionRegistry.NotificationDefinition? def =
            NotificationDefinitionRegistry.TryGetDefinition(NotificationEventType.TRIP_SCHEDULED);
        if (def?.DriverPushTemplateKeys is null)
        {
            throw new InvalidOperationException(
                $"NotificationDefinitionRegistry must define DriverTripScheduledPushTemplateKeys for {NotificationEventType.TRIP_SCHEDULED}.");
        }

        var keys = def.DriverPushTemplateKeys;
        var placeholders = new Dictionary<string, string>
        {
            ["startDate"] = ApiDateTimeContract.FormatUtcInstantForDisplay(startDate),
            ["endDate"] = ApiDateTimeContract.FormatUtcInstantForDisplay(endDate)
        };

        (string titleKey, string bodyKey) = kind switch
        {
            TripScheduleDriverPushKind.NewAssignment
                or TripScheduleDriverPushKind.DriverChangedOnTripUpdate
                or TripScheduleDriverPushKind.VehicleCascadeReassignment =>
                (keys.AssignedTitleTemplateKey, keys.AssignedBodyTemplateKey),
            TripScheduleDriverPushKind.ScheduleUpdatedOnTripUpdate =>
                (keys.ScheduleUpdateTitleTemplateKey, keys.ScheduleUpdateBodyTemplateKey),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
        };

        string title = await _templateService.ResolveAsync(titleKey, Lang.Default, placeholders);
        string body = await _templateService.ResolveAsync(bodyKey, Lang.Default, placeholders);

        string eventKey = NotificationEventType.TRIP_SCHEDULED.ToString();

        var pushData = new Dictionary<string, string>
        {
            ["resourceId"] = tripId.ToString(),
            ["resourceType"] = "TRIP",
            ["eventType"] = eventKey
        };

        recipients.Add(new NotificationRecipient
        {
            ModelId = driverId,
            ModelType = "Driver",
            NotificationEventType = NotificationEventType.TRIP_SCHEDULED,
            Domain = NotificationDomain.Driver,
            Channels = new HashSet<NotificationChannel> { NotificationChannel.PUSH, NotificationChannel.IN_APP },
            Title = title,
            Body = body,
            ResourceType = NotificationResourceType.TRIP,
            ResourceId = tripId,
            ClickAction = "open_trip",
            PushData = pushData,
            PushClickAction = "open_trip"
        });
    }
}
