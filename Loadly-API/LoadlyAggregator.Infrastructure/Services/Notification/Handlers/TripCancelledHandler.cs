using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Infrastructure.Services.Notification.AudienceBuilders;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Enums;
using Lang = LoadlyAggregator.Shared.Constants.NotificationLanguage;

namespace LoadlyAggregator.Infrastructure.Services.Notification.Handlers;

/// <summary>
/// Builds recipients for <see cref="TripCancelledNotificationData"/>:
/// 1. Company receives in-app (and email if configured).
/// 2. Driver receives push + in-app when the cancelled trip had a driver assigned.
///    A driver finding out the trip is gone by opening the app is unsafe (they may already
///    be loading cargo), so cancellation must surface the same way as assignment/reschedule.
/// 3. Contractor receives push + in-app when the cancelled trip had a contractor assigned.
/// </summary>
public class TripCancelledHandler : INotificationHandler<TripCancelledNotificationData>
{
    private readonly ITripRepository _tripRepository;
    private readonly IContractorRepository _contractorRepository;
    private readonly ICompanyEmailAudienceBuilder _companyBuilder;
    private readonly INotificationTemplateService _templateService;
    private readonly ILogger<TripCancelledHandler> _logger;

    public TripCancelledHandler(
        ITripRepository tripRepository,
        IContractorRepository contractorRepository,
        ICompanyEmailAudienceBuilder companyBuilder,
        INotificationTemplateService templateService,
        ILogger<TripCancelledHandler> logger)
    {
        _tripRepository = tripRepository;
        _contractorRepository = contractorRepository;
        _companyBuilder = companyBuilder;
        _templateService = templateService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<NotificationRecipient>> BuildRecipientsAsync(
        TripCancelledNotificationData data, CancellationToken cancellationToken = default)
    {
        var recipients = new List<NotificationRecipient>();

        if (data.CompanyId.HasValue)
        {
            var companyRecipient = await _companyBuilder.BuildAsync(
                data.CompanyId.Value, data.TripId, NotificationEventType.TRIP_CANCELLED,
                resourceType: NotificationResourceType.TRIP, resourceId: data.TripId, clickAction: "open_trip");

            if (companyRecipient is null)
            {
                _logger.LogWarning(
                    "Trip {TripId} or Company {CompanyId} not found. Skipping company part of TripCancelled notification.",
                    data.TripId, data.CompanyId);
            }
            else
            {
                recipients.Add(companyRecipient);
            }
        }

        if (data.DriverId.HasValue)
            await AddDriverRecipientAsync(recipients, data.TripId, data.DriverId.Value);

        if (data.ContractorId.HasValue)
            await AddContractorRecipientAsync(recipients, data.TripId, data.ContractorId.Value);

        return recipients;
    }

    private async Task AddDriverRecipientAsync(
        List<NotificationRecipient> recipients,
        long tripId,
        long driverId)
    {
        var trip = await _tripRepository.GetByIdAsync(tripId);
        if (trip is null)
        {
            _logger.LogWarning("Trip {TripId} not found. Skipping driver part of TripCancelled notification.", tripId);
            return;
        }

        NotificationDefinitionRegistry.NotificationDefinition? def =
            NotificationDefinitionRegistry.TryGetDefinition(NotificationEventType.TRIP_CANCELLED);
        if (def?.DriverCancelPushTemplateKeys is null)
        {
            throw new InvalidOperationException(
                $"NotificationDefinitionRegistry must define DriverCancelPushTemplateKeys for {NotificationEventType.TRIP_CANCELLED}.");
        }

        var keys = def.DriverCancelPushTemplateKeys;
        var placeholders = new Dictionary<string, string>
        {
            ["startDate"] = ApiDateTimeContract.FormatUtcInstantForDisplay(trip.StartDate),
            ["endDate"] = ApiDateTimeContract.FormatUtcInstantForDisplay(trip.EndDate)
        };

        string title = await _templateService.ResolveAsync(keys.TitleTemplateKey, Lang.Default, placeholders);
        string body = await _templateService.ResolveAsync(keys.BodyTemplateKey, Lang.Default, placeholders);

        string eventKey = NotificationEventType.TRIP_CANCELLED.ToString();

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
            NotificationEventType = NotificationEventType.TRIP_CANCELLED,
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

    private async Task AddContractorRecipientAsync(
        List<NotificationRecipient> recipients,
        long tripId,
        long contractorId)
    {
        var contractor = await _contractorRepository.GetByIdAsync(contractorId);
        if (contractor is null)
        {
            _logger.LogWarning(
                "Contractor {ContractorId} not found. Skipping contractor part of TripCancelled notification.",
                contractorId);
            return;
        }

        var placeholders = new Dictionary<string, string>
        {
            ["tripId"] = tripId.ToString()
        };

        string title = await _templateService.ResolveAsync("trip_cancelled_contractor_title", Lang.Default, placeholders);
        string body = await _templateService.ResolveAsync("trip_cancelled_contractor_body", Lang.Default, placeholders);

        string eventKey = NotificationEventType.TRIP_CANCELLED.ToString();

        recipients.Add(new NotificationRecipient
        {
            ModelId = contractor.Id,
            ModelType = "Contractor",
            NotificationEventType = NotificationEventType.TRIP_CANCELLED,
            Domain = NotificationDomain.Contractor,
            Channels = new HashSet<NotificationChannel> { NotificationChannel.PUSH, NotificationChannel.IN_APP },
            Title = title,
            Body = body,
            ResourceType = NotificationResourceType.TRIP,
            ResourceId = tripId,
            ClickAction = "open_trip",
            PushClickAction = "open_trip",
            PushData = new Dictionary<string, string>
            {
                ["resourceId"] = tripId.ToString(),
                ["resourceType"] = "TRIP",
                ["eventType"] = eventKey
            }
        });
    }
}
