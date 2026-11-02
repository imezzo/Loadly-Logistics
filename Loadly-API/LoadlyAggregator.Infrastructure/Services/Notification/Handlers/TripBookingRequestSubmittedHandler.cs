using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Infrastructure.Services.Notification.AudienceBuilders;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Infrastructure.Services.Notification.Handlers;

public class TripBookingRequestSubmittedHandler : INotificationHandler<TripBookingRequestSubmittedNotificationData>
{
    private readonly ITripRepository _tripRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IAdminPermissionAudienceBuilder _adminBuilder;
    private readonly ILogger<TripBookingRequestSubmittedHandler> _logger;

    public TripBookingRequestSubmittedHandler(
        ITripRepository tripRepository,
        ICompanyRepository companyRepository,
        IContractRepository contractRepository,
        IAdminPermissionAudienceBuilder adminBuilder,
        ILogger<TripBookingRequestSubmittedHandler> logger)
    {
        _tripRepository = tripRepository;
        _companyRepository = companyRepository;
        _contractRepository = contractRepository;
        _adminBuilder = adminBuilder;
        _logger = logger;
    }

    public async Task<IReadOnlyList<NotificationRecipient>> BuildRecipientsAsync(
        TripBookingRequestSubmittedNotificationData data,
        CancellationToken cancellationToken = default)
    {
        var trip = await _tripRepository.GetByIdAsync(data.TripId);
        if (trip is null)
        {
            _logger.LogWarning("Trip {TripId} not found for booking submitted notification.", data.TripId);
            return [];
        }

        Company? company = null;
        if (trip.ContractId is > 0)
        {
            var contract = trip.Contract ?? await _contractRepository.GetByIdAsync(trip.ContractId.Value);
            if (contract is not null)
                company = await _companyRepository.GetByIdAsync(contract.CompanyId);
        }

        if (company is null)
        {
            _logger.LogWarning("Company not found for trip {TripId}. Skipping booking submitted admin notification.", data.TripId);
            return [];
        }

        return await _adminBuilder.BuildForTripEventAsync(
            "view_trip",
            "admin",
            NotificationEventType.TRIP_BOOKING_REQUEST_SUBMITTED,
            trip,
            company,
            "Trip Booking Request",
            resourceType: NotificationResourceType.TRIP,
            resourceId: data.TripId,
            clickAction: "open_trip",
            cancellationToken);
    }
}
