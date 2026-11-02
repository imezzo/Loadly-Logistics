using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Domain.Contractor.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Contractor.Actions.Trips;

public class ContractorCreateBookingRequestAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripBookingInvitationRepository _tripBookingInvitationRepository;
    private readonly ITripBookingRequestRepository _bookingRequestRepository;
    private readonly ITripBookingInvitationRoundRepository _invitationRoundRepository;
    private readonly IContractorRepository _contractorRepository;
    private readonly INotificationEventDispatcher _notificationDispatcher;

    public ContractorCreateBookingRequestAction(
        ITripRepository tripRepository,
        ITripBookingInvitationRepository tripBookingInvitationRepository,
        ITripBookingRequestRepository bookingRequestRepository,
        ITripBookingInvitationRoundRepository invitationRoundRepository,
        IContractorRepository contractorRepository,
        INotificationEventDispatcher notificationDispatcher)
    {
        _tripRepository = tripRepository;
        _tripBookingInvitationRepository = tripBookingInvitationRepository;
        _bookingRequestRepository = bookingRequestRepository;
        _invitationRoundRepository = invitationRoundRepository;
        _contractorRepository = contractorRepository;
        _notificationDispatcher = notificationDispatcher;
    }

    public async Task ExecuteAsync(ContractorCreateBookingRequestDto dto)
    {
        var contractor = await _contractorRepository.GetByIdOrThrowAsync(dto.ContractorId);
        if (contractor.ContractorStatus != ContractorStatus.ACTIVE)
            throw new ValidationException("contractors.not_active");

        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);
        TripBookingHelper.EnsureOpenForContractorBooking(trip);

        var round = await _invitationRoundRepository.GetActiveByTripIdAsync(dto.TripId)
            ?? throw new ValidationException(TripMessageKeys.BookingInviteRequired);

        var invitation = await _tripBookingInvitationRepository.GetByRoundAndContractorAsync(round.Id, dto.ContractorId);
        if (invitation is null || invitation.Status != TripBookingInvitationStatus.Pending)
            throw new ValidationException(TripMessageKeys.BookingInviteRequired);

        if (await _bookingRequestRepository.GetPendingByRoundAndContractorAsync(round.Id, dto.ContractorId) is not null)
            throw new ValidationException(TripMessageKeys.BookingAlreadyApplied);

        var request = new TripBookingRequest
        {
            TripId = dto.TripId,
            ContractorId = dto.ContractorId,
            TripBookingInvitationRoundId = round.Id,
            Status = TripBookingRequestStatus.Pending
        };
        await _bookingRequestRepository.AddAsync(request);

        await _bookingRequestRepository.SaveChangesAsync();

        await _notificationDispatcher.DispatchAsync(new TripBookingRequestSubmittedNotificationData
        {
            TripId = dto.TripId,
            BookingRequestId = request.Id,
            ContractorId = dto.ContractorId
        });
    }
}
