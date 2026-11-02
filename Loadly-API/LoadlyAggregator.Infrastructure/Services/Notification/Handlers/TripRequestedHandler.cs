using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Infrastructure.Services.Notification.AudienceBuilders;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Infrastructure.Services.Notification.Handlers;

/// <summary>
/// Builds recipients for <see cref="TripRequestedByCompanyNotificationData"/>:
/// All active admins with view_trip permission receive email + push + in-app.
/// </summary>
public class TripRequestedHandler : INotificationHandler<TripRequestedByCompanyNotificationData>
{
    private readonly ITripRepository _tripRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IAdminPermissionAudienceBuilder _adminBuilder;
    private readonly ILogger<TripRequestedHandler> _logger;

    public TripRequestedHandler(
        ITripRepository tripRepository,
        ICompanyRepository companyRepository,
        IAdminPermissionAudienceBuilder adminBuilder,
        ILogger<TripRequestedHandler> logger)
    {
        _tripRepository = tripRepository;
        _companyRepository = companyRepository;
        _adminBuilder = adminBuilder;
        _logger = logger;
    }

    public async Task<IReadOnlyList<NotificationRecipient>> BuildRecipientsAsync(
        TripRequestedByCompanyNotificationData data, CancellationToken cancellationToken = default)
    {
        try
        {
            var trip = await _tripRepository.GetByIdAsync(data.TripId);
            var company = await _companyRepository.GetByIdAsync(data.CompanyId);

            if (trip is null || company is null)
            {
                _logger.LogWarning(
                    "Trip {TripId} or Company {CompanyId} not found. Skipping TripRequested notification.",
                    data.TripId, data.CompanyId);
                return [];
            }

            return await _adminBuilder.BuildForTripEventAsync(
                "view_trip", "admin",
                NotificationEventType.TRIP_REQUESTED,
                trip, company,
                "New Trip Request",
                resourceType: NotificationResourceType.TRIP,
                resourceId: data.TripId,
                clickAction: "open_trip",
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to build admin recipients for trip {TripId}.", data.TripId);
            return [];
        }
    }
}
