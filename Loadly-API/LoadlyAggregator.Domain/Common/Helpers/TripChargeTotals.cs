using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripChargeTotals
{
    public static decimal BillableAmount(decimal amount, decimal? companyShare) =>
        amount - (companyShare ?? 0m);

    public static decimal BillableAmount(TripCharge charge) =>
        BillableAmount(charge.Amount, charge.CompanyShare);

    public static decimal CalculateTotalTripCost(IEnumerable<TripCharge> charges) =>
        charges.Sum(BillableAmount);

    public static decimal CalculateTotalAddonsCost(IEnumerable<TripCharge> charges) =>
        charges
            .Where(c => !TripChargeClassification.IsStructural(c.ChargeType))
            .Sum(BillableAmount);

    public static decimal SumBillableForChargeTypes(IEnumerable<TripCharge>? charges, params TripChargeType[] chargeTypes) =>
        charges?
            .Where(c => chargeTypes.Contains(c.ChargeType))
            .Sum(BillableAmount) ?? 0m;

    public static decimal SumForChargeTypes(IEnumerable<TripCharge>? charges, params TripChargeType[] chargeTypes) =>
        charges?
            .Where(c => chargeTypes.Contains(c.ChargeType))
            .Sum(c => c.Amount) ?? 0m;
}
