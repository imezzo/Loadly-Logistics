using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractorPayoutTripSelection
{
    public static List<Trip> ResolveEligibleTrips(
        IReadOnlyList<long> tripIds,
        IReadOnlyDictionary<long, Trip> fetchedTrips,
        long? existingPayoutId = null)
    {
        var trips = new List<Trip>();

        foreach (var tripId in tripIds)
        {
            if (!fetchedTrips.TryGetValue(tripId, out Trip? trip))
                throw new NotFoundException("trips.not_found_with_id", tripId);

            trips.Add(trip);
        }

        var firstContractorId = trips[0].ContractorId;
        if (trips.Exists(t => t.ContractorId != firstContractorId))
            throw new ValidationException("contractor_payouts.trips_must_same_contractor");

        foreach (var trip in trips)
        {
            if (trip.ContractorPayoutId.HasValue && trip.ContractorPayoutId != existingPayoutId)
                throw new ValidationException("contractor_payouts.trip_already_in_payout");

            if (trip.Status != TripStatus.Completed
                || !trip.ContractorId.HasValue
                || trip.ContractorPrice is not > 0)
                throw new ValidationException("contractor_payouts.trip_ids_not_eligible");
        }

        return trips;
    }
}
