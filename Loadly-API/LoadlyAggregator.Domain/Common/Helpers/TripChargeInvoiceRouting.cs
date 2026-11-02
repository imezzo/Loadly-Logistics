using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripChargeInvoiceRouting
{
    public static InvoiceType GetDefaultInvoiceType(TripChargeType chargeType) =>
        TripChargeClassification.IsStructural(chargeType)
            ? InvoiceType.Trips
            : InvoiceType.Claims;

    public static InvoiceType GetEffectiveInvoiceType(TripCharge charge) =>
        charge.InvoiceType ?? GetDefaultInvoiceType(charge.ChargeType);

    public static decimal SumBillableForInvoiceType(Trip trip, InvoiceType invoiceType) =>
        trip.TripCharges?
            .Where(c => GetEffectiveInvoiceType(c) == invoiceType)
            .Sum(TripChargeTotals.BillableAmount) ?? 0m;

    public static bool HasBillableAmount(Trip trip, InvoiceType invoiceType) =>
        SumBillableForInvoiceType(trip, invoiceType) > 0m;

    public static IReadOnlyList<ChargeTypeInvoiceRoutingConflict> DetectRoutingConflicts(
        IEnumerable<TripCharge> charges) =>
        charges
            .GroupBy(c => c.ChargeType)
            .Select(g => new
            {
                ChargeType = g.Key,
                Routes = g
                    .GroupBy(GetEffectiveInvoiceType)
                    .Select(rg => (
                        InvoiceType: rg.Key,
                        TripIds: (IReadOnlyList<long>)rg.Select(c => c.TripId).Distinct().OrderBy(id => id).ToList()))
                    .ToList()
            })
            .Where(x => x.Routes.Count > 1)
            .Select(x => new ChargeTypeInvoiceRoutingConflict
            {
                ChargeType = x.ChargeType,
                Routes = x.Routes
            })
            .OrderBy(x => x.ChargeType)
            .ToList();
}

public sealed class ChargeTypeInvoiceRoutingConflict
{
    public TripChargeType ChargeType { get; init; }
    public IReadOnlyList<(InvoiceType InvoiceType, IReadOnlyList<long> TripIds)> Routes { get; init; } = [];
}
