using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripContractorPayoutHelper
{
    public static bool IsPaidToContractor(Trip trip) =>
        trip.ContractorPayoutId != null
        && trip.ContractorPayout != null
        && trip.ContractorPayout.Status == ContractorPayoutStatus.Paid;
}
