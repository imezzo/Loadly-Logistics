using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripBookingHelper
{
    public static void EnsureOpenForContractorBooking(Trip trip)
    {
        if (trip.Status is not TripStatus.Pending
            || trip.DispatchingMode != TripDispatchingMode.Contractors
            || trip.ContractorId is not null)
            throw new ValidationException(TripMessageKeys.BookingTripNotOpen);
    }

    public static async Task CloseAsync(
        long tripId,
        ITripBookingRequestRepository bookingRequestRepository,
        ITripBookingInvitationRepository tripBookingInvitationRepository,
        ITripBookingInvitationRoundRepository invitationRoundRepository,
        long? reviewedByAdminId = null)
    {
        var now = DateTime.UtcNow;
        await bookingRequestRepository.RejectPendingByTripIdAsync(tripId, reviewedByAdminId, now);
        await tripBookingInvitationRepository.CancelPendingByTripIdAsync(tripId, now);
        await invitationRoundRepository.CompleteActiveByTripIdAsync(tripId, now);
    }
}
