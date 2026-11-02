using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Shared.Enums;
using Lang = LoadlyAggregator.Shared.Constants.NotificationLanguage;

namespace LoadlyAggregator.Infrastructure.Services.Notification.Handlers;

public class TripPushedToContractorsHandler : INotificationHandler<TripPushedToContractorsNotificationData>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly INotificationTemplateService _templateService;
    private readonly ILogger<TripPushedToContractorsHandler> _logger;

    public TripPushedToContractorsHandler(
        IContractorRepository contractorRepository,
        INotificationTemplateService templateService,
        ILogger<TripPushedToContractorsHandler> logger)
    {
        _contractorRepository = contractorRepository;
        _templateService = templateService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<NotificationRecipient>> BuildRecipientsAsync(
        TripPushedToContractorsNotificationData data,
        CancellationToken cancellationToken = default)
    {
        var contractor = await _contractorRepository.GetByIdAsync(data.ContractorId);
        if (contractor is null)
        {
            _logger.LogWarning("Contractor {ContractorId} not found for trip push notification.", data.ContractorId);
            return [];
        }

        string eventKey = NotificationEventType.TRIP_PUSHED_TO_CONTRACTORS.ToString();
        var placeholders = new Dictionary<string, string>
        {
            ["tripCount"] = data.TripCount.ToString()
        };
        string bodyKey = data.TripCount == 1 ? $"{eventKey}_body_single" : $"{eventKey}_body";
        string title = await _templateService.ResolveAsync($"{eventKey}_subject", Lang.Default, placeholders);
        string body = await _templateService.ResolveAsync(bodyKey, Lang.Default, placeholders);

        return
        [
            new NotificationRecipient
            {
                ModelId = contractor.Id,
                ModelType = "Contractor",
                NotificationEventType = NotificationEventType.TRIP_PUSHED_TO_CONTRACTORS,
                Domain = NotificationDomain.Contractor,
                Channels = new HashSet<NotificationChannel> { NotificationChannel.PUSH, NotificationChannel.IN_APP },
                Title = title,
                Body = body,
                ResourceType = NotificationResourceType.TRIP,
                ClickAction = "open_trip_booking_invites",
                PushClickAction = "open_trip_booking_invites",
                PushData = new Dictionary<string, string>
                {
                    ["eventType"] = NotificationEventType.TRIP_PUSHED_TO_CONTRACTORS.ToString(),
                    ["tripCount"] = data.TripCount.ToString()
                }
            }
        ];
    }
}
