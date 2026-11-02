using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class ApproveTripBookingRequestAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripBookingRequestRepository _bookingRequestRepository;
    private readonly ITripBookingInvitationRepository _tripBookingInvitationRepository;
    private readonly ITripBookingInvitationRoundRepository _invitationRoundRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationEventDispatcher _notificationDispatcher;

    public ApproveTripBookingRequestAction(
        ITripRepository tripRepository,
        ITripBookingRequestRepository bookingRequestRepository,
        ITripBookingInvitationRepository tripBookingInvitationRepository,
        ITripBookingInvitationRoundRepository invitationRoundRepository,
        ICurrentUserService currentUserService,
        INotificationEventDispatcher notificationDispatcher)
    {
        _tripRepository = tripRepository;
        _bookingRequestRepository = bookingRequestRepository;
        _tripBookingInvitationRepository = tripBookingInvitationRepository;
        _invitationRoundRepository = invitationRoundRepository;
        _currentUserService = currentUserService;
        _notificationDispatcher = notificationDispatcher;
    }

    public async Task ExecuteAsync(ApproveTripBookingRequestDto dto)
    {
        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);
        if (trip.Status == TripStatus.CompanyDraft)
            throw new NotFoundException("trips.not_found_with_id", dto.TripId);

        TripBookingHelper.EnsureOpenForContractorBooking(trip);

        var request = await _bookingRequestRepository.GetByIdOrThrowAsync(dto.RequestId);
        if (request.TripId != dto.TripId)
            throw new NotFoundException(TripMessageKeys.BookingRequestNotFound, dto.RequestId);

        if (request.Status != TripBookingRequestStatus.Pending)
            throw new ValidationException(TripMessageKeys.BookingRequestNotPending);

        var adminId = _currentUserService.GetCurrentUserId();
        var now = DateTime.UtcNow;

        request.Status = TripBookingRequestStatus.Approved;
        request.ReviewedAt = now;
        request.ReviewedByAdminId = adminId;
        _bookingRequestRepository.Update(request);

        await _bookingRequestRepository.RejectPendingByTripIdAsync(dto.TripId, adminId, now, exceptRequestId: request.Id);
        await _tripBookingInvitationRepository.ResolveOnBookingApprovalAsync(
            dto.TripId,
            request.ContractorId,
            now,
            request.TripBookingInvitationRoundId);

        if (request.TripBookingInvitationRoundId.HasValue)
        {
            var round = await _invitationRoundRepository.GetByIdAsync(request.TripBookingInvitationRoundId.Value);
            if (round is not null)
            {
                round.ContractorId = request.ContractorId;
                round.Status = TripBookingInvitationRoundStatus.Completed;
                _invitationRoundRepository.Update(round);
            }
        }

        trip.ContractorId = request.ContractorId;
        TripUpdateHelper.ApplyStatus(
            trip,
            request.ContractorId,
            trip.DriverId,
            trip.VehicleId,
            trip.StartDate ?? default,
            trip.EndDate ?? default);
        _tripRepository.Update(trip);

        await _tripRepository.SaveChangesAsync();

        await _notificationDispatcher.DispatchAsync(new TripBookingRequestApprovedNotificationData
        {
            TripId = trip.Id,
            ContractorId = request.ContractorId,
            BookingRequestId = request.Id
        });
    }
}
