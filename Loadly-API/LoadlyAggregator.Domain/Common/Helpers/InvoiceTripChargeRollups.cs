using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public sealed record InvoiceTripChargeRollup(TripChargeType ChargeType, int Count, decimal Amount);

/// <summary>
/// Per-<see cref="TripChargeType"/> rollups for invoice trip lines (document + persisted snapshot).
/// </summary>
public static class InvoiceTripChargeRollups
{
    public static IReadOnlyList<InvoiceTripChargeRollup> Build(IReadOnlyList<Trip> trips, InvoiceType invoiceType) =>
        trips
            .SelectMany(trip => trip.TripCharges ?? [])
            .Where(charge => TripChargeInvoiceRouting.GetEffectiveInvoiceType(charge) == invoiceType)
            .Select(charge => (charge.ChargeType, Amount: TripChargeTotals.BillableAmount(charge)))
            .Where(entry => entry.Amount > 0m)
            .GroupBy(entry => entry.ChargeType)
            .OrderBy(group => (int)group.Key)
            .Select(group => new InvoiceTripChargeRollup(
                group.Key,
                group.Count(),
                group.Sum(entry => entry.Amount)))
            .ToList();

    public static decimal SumAmount(IReadOnlyList<InvoiceTripChargeRollup> rollups) =>
        rollups.Sum(rollup => rollup.Amount);
}
