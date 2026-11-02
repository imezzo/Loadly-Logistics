using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripInvoicingStatusResolver
{
    public sealed class Snapshot
    {
        public TripInvoicingStatus Status { get; init; }
        public bool RequiresMainInvoice { get; init; }
        public bool RequiresClaimInvoice { get; init; }
        public bool FullyInvoiced { get; init; }
        public bool PartiallyInvoiced { get; init; }
    }

    public static Snapshot Resolve(Trip trip)
    {
        bool requiresMain = TripChargeInvoiceRouting.HasBillableAmount(trip, InvoiceType.Trips);
        bool requiresClaim = TripChargeInvoiceRouting.HasBillableAmount(trip, InvoiceType.Claims);
        bool mainLinked = trip.InvoiceId.HasValue;
        bool claimLinked = trip.ClaimInvoiceId.HasValue;

        bool fullyInvoiced = (!requiresMain || mainLinked) && (!requiresClaim || claimLinked);
        bool partiallyInvoiced = (mainLinked || claimLinked) && !fullyInvoiced;

        var status = fullyInvoiced && (requiresMain || requiresClaim)
            ? TripInvoicingStatus.FullyInvoiced
            : partiallyInvoiced
                ? TripInvoicingStatus.PartiallyInvoiced
                : TripInvoicingStatus.NotInvoiced;

        return new Snapshot
        {
            Status = status,
            RequiresMainInvoice = requiresMain,
            RequiresClaimInvoice = requiresClaim,
            FullyInvoiced = fullyInvoiced,
            PartiallyInvoiced = partiallyInvoiced
        };
    }

    public static int CountNotFullyInvoiced(IEnumerable<Trip>? trips) =>
        trips?.Count(t => Resolve(t).Status != TripInvoicingStatus.FullyInvoiced) ?? 0;
}
