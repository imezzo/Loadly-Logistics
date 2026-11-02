using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Domain.Company.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Trips;

public class CompanyCancelTripAction
{
    private readonly ITripRepository _tripRepository;
    private readonly INotificationEventDispatcher _notificationDispatcher;

    public CompanyCancelTripAction(
        ITripRepository tripRepository,
        INotificationEventDispatcher notificationDispatcher)
    {
        _tripRepository = tripRepository;
        _notificationDispatcher = notificationDispatcher;
    }

    public async Task ExecuteAsync(CompanyCancelTripDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);

        if (trip.Contract?.CompanyId != dto.CompanyId)
            throw new ForbiddenException(TripMessageKeys.NotAuthorized);

        // (Idempotency by status check) If the trip is already cancelled, we don't need to cancel it again.
        if (trip.Status is TripStatus.Cancelled)
            return;

        if (trip.Status != TripStatus.Pending && trip.Status != TripStatus.Scheduled)
            throw new ValidationException(TripMessageKeys.CanOnlyCancelRequestedOrScheduled);

        // Capture the assigned driver before mutating; the driver only needs to be notified
        // when the trip had already been scheduled (i.e. they could see/start it).
        long? assignedDriverId = trip.Status == TripStatus.Scheduled ? trip.DriverId : null;
        long? contractorId = trip.ContractorId;

        trip.Status = TripStatus.Cancelled;
        _tripRepository.Update(trip);
        await _tripRepository.SaveChangesAsync();

        await _notificationDispatcher.DispatchAsync(new TripCancelledNotificationData
        {
            TripId = trip.Id,
            CompanyId = trip.Contract!.CompanyId,
            DriverId = assignedDriverId,
            ContractorId = contractorId
        });
    }
}
