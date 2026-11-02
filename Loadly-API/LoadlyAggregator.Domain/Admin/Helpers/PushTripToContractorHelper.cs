using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Notifications;
using LoadlyAggregator.Domain.Common.Notifications.EventData;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Helpers;

public class PushTripToContractorHelper
{
    private readonly IContractorRepository _contractorRepository;
    private readonly ITripBookingInvitationRepository _tripBookingInvitationRepository;
    private readonly ITripBookingRequestRepository _bookingRequestRepository;
    private readonly ITripBookingInvitationRoundRepository _invitationRoundRepository;
    private readonly INotificationEventDispatcher _notificationDispatcher;
    private readonly ICurrentUserService _currentUserService;

    public PushTripToContractorHelper(
        IContractorRepository contractorRepository,
        ITripBookingInvitationRepository tripBookingInvitationRepository,
        ITripBookingRequestRepository bookingRequestRepository,
        ITripBookingInvitationRoundRepository invitationRoundRepository,
        INotificationEventDispatcher notificationDispatcher,
        ICurrentUserService currentUserService)
    {
        _contractorRepository = contractorRepository;
        _tripBookingInvitationRepository = tripBookingInvitationRepository;
        _bookingRequestRepository = bookingRequestRepository;
        _invitationRoundRepository = invitationRoundRepository;
        _notificationDispatcher = notificationDispatcher;
        _currentUserService = currentUserService;
    }

    public async Task InviteContractorsAsync(
        Trip trip,
        bool sendToAllContractors,
        List<long>? contractorIds)
    {
        var tripId = trip.Id;
        TripBookingHelper.EnsureOpenForContractorBooking(trip);

        List<long> audience;
        if (sendToAllContractors)
        {
            audience = (await _contractorRepository.FindAsync(c => c.ContractorStatus == ContractorStatus.ACTIVE))
                .Select(c => c.Id).ToList();
            if (audience.Count == 0)
                throw new ValidationException(TripMessageKeys.PushAudienceRequired);
        }
        else
        {
            if (contractorIds == null || contractorIds.Count == 0)
                throw new ValidationException(TripMessageKeys.PushAudienceRequired);

            var requestedIds = contractorIds.Distinct().ToList();
            var found = (await _contractorRepository.FindAsync(c => requestedIds.Contains(c.Id))).ToList();
            if (found.Count != requestedIds.Count)
                throw new NotFoundException("contractors.not_found_with_id");
            if (found.Any(c => c.ContractorStatus != ContractorStatus.ACTIVE))
                throw new ValidationException("contractors.not_active");
            audience = requestedIds;
        }

        var pendingInvites = await _tripBookingInvitationRepository.FindAsync(
            i => i.TripId == tripId && i.Status == TripBookingInvitationStatus.Pending);
        var pendingRequests = await _bookingRequestRepository.FindAsync(
            r => r.TripId == tripId && r.Status == TripBookingRequestStatus.Pending);
        var excluded = pendingInvites.Select(i => i.ContractorId)
            .Concat(pendingRequests.Select(r => r.ContractorId))
            .ToHashSet();

        var toInvite = audience.Where(id => !excluded.Contains(id)).ToList();
        if (toInvite.Count == 0)
            throw new ValidationException(TripMessageKeys.PushAudienceRequired);

        var addedBy = _currentUserService.GetCurrentUserId();

        var activeRounds = await _invitationRoundRepository.FindAsync(
            r => r.TripId == tripId && r.Status == TripBookingInvitationRoundStatus.Active);
        var round = activeRounds.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
        if (round == null)
        {
            round = new TripBookingInvitationRound
            {
                TripId = tripId,
                Status = TripBookingInvitationRoundStatus.Active,
                AddedBy = addedBy
            };
            await _invitationRoundRepository.AddAsync(round);
            await _invitationRoundRepository.SaveChangesAsync();
        }

        var invites = new List<TripBookingInvitation>();
        foreach (var contractorId in toInvite)
        {
            invites.Add(new TripBookingInvitation
            {
                TripId = tripId,
                ContractorId = contractorId,
                TripBookingInvitationRoundId = round.Id,
                Status = TripBookingInvitationStatus.Pending,
                AddedBy = addedBy
            });
        }

        await _tripBookingInvitationRepository.AddRangeAsync(invites);
        await _tripBookingInvitationRepository.SaveChangesAsync();

        foreach (var contractorId in toInvite)
        {
            await _notificationDispatcher.DispatchAsync(new TripPushedToContractorsNotificationData
            {
                ContractorId = contractorId,
                TripCount = 1
            });
        }
    }
}
