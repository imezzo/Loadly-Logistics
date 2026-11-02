using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Shared.Enums;
using Lang = LoadlyAggregator.Shared.Constants.NotificationLanguage;

namespace LoadlyAggregator.Infrastructure.Services.Notification.Handlers;

public class TripContractorUnassignedHandler : INotificationHandler<TripContractorUnassignedNotificationData>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly INotificationTemplateService _templateService;
    private readonly ILogger<TripContractorUnassignedHandler> _logger;

    public TripContractorUnassignedHandler(
        IContractorRepository contractorRepository,
        INotificationTemplateService templateService,
        ILogger<TripContractorUnassignedHandler> logger)
    {
        _contractorRepository = contractorRepository;
        _templateService = templateService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<NotificationRecipient>> BuildRecipientsAsync(
        TripContractorUnassignedNotificationData data,
        CancellationToken cancellationToken = default)
    {
        var contractor = await _contractorRepository.GetByIdAsync(data.ContractorId);
        if (contractor is null)
        {
            _logger.LogWarning("Contractor {ContractorId} not found for trip contractor unassigned notification.", data.ContractorId);
            return [];
        }

        string eventKey = NotificationEventType.TRIP_CONTRACTOR_UNASSIGNED.ToString();
        var placeholders = new Dictionary<string, string>
        {
            ["tripId"] = data.TripId.ToString()
        };
        string title = await _templateService.ResolveAsync($"{eventKey}_subject", Lang.Default, placeholders);
        string body = await _templateService.ResolveAsync($"{eventKey}_body", Lang.Default, placeholders);

        return
        [
            new NotificationRecipient
            {
                ModelId = contractor.Id,
                ModelType = "Contractor",
                NotificationEventType = NotificationEventType.TRIP_CONTRACTOR_UNASSIGNED,
                Domain = NotificationDomain.Contractor,
                Channels = new HashSet<NotificationChannel> { NotificationChannel.PUSH, NotificationChannel.IN_APP },
                Title = title,
                Body = body,
                ResourceType = NotificationResourceType.TRIP,
                ResourceId = data.TripId,
                ClickAction = "open_trip",
                PushClickAction = "open_trip",
                PushData = new Dictionary<string, string>
                {
                    ["resourceId"] = data.TripId.ToString(),
                    ["resourceType"] = "TRIP",
                    ["eventType"] = NotificationEventType.TRIP_CONTRACTOR_UNASSIGNED.ToString()
                }
            }
        ];
    }
}
