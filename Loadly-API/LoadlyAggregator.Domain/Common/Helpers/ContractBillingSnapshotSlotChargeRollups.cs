using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public sealed class ContractBillingSnapshotSlotChargeRollups
{
    public decimal SubTotal { get; init; }
    public decimal WorkingDaysAmount { get; init; }
    public decimal ExtraWorkingDaysAmount { get; init; }
    public decimal ExtraKmsAmount { get; init; }
    public decimal ExtraTripsAmount { get; init; }
    public int WorkingDaysCount { get; init; }
    public int ExtraWorkingDaysCount { get; init; }
    public int ExtraTripsCount { get; init; }
    public decimal ExtraKmsCount { get; init; }

    public static ContractBillingSnapshotSlotChargeRollups ForInvoiceType(
        ContractBillingSnapshot snapshot,
        InvoiceType invoiceType)
    {
        var extraKmRoute = ContractSlotChargeInvoiceRouting.ResolveExtraKmOverageInvoiceType(snapshot);
        var lines = snapshot.ChargeLines
            .Where(line => line.DeletedAt == null)
            .Where(line =>
                ContractSlotChargeInvoiceRouting.GetEffectiveInvoiceType(line.ChargeKind, extraKmRoute)
                == invoiceType)
            .ToList();

        return new ContractBillingSnapshotSlotChargeRollups
        {
            SubTotal = lines.Sum(line => line.Amount),
            WorkingDaysAmount = SumAmount(lines, ContractSlotChargeKind.BaseWorkingDay),
            ExtraWorkingDaysAmount = SumAmount(lines, ContractSlotChargeKind.ExtraWorkingDay),
            ExtraKmsAmount = SumAmount(lines, ContractSlotChargeKind.ExtraKmOverage),
            ExtraTripsAmount = SumAmount(lines, ContractSlotChargeKind.ExtraTrip),
            WorkingDaysCount = CountLines(lines, ContractSlotChargeKind.BaseWorkingDay),
            ExtraWorkingDaysCount = CountLines(lines, ContractSlotChargeKind.ExtraWorkingDay),
            ExtraTripsCount = CountLines(lines, ContractSlotChargeKind.ExtraTrip),
            ExtraKmsCount = SumKmOverage(snapshot, invoiceType, extraKmRoute)
        };
    }

    private static decimal SumAmount(
        IReadOnlyList<ContractBillingSnapshotChargeLine> lines,
        ContractSlotChargeKind kind) =>
        lines.Where(line => line.ChargeKind == kind).Sum(line => line.Amount);

    private static int CountLines(
        IReadOnlyList<ContractBillingSnapshotChargeLine> lines,
        ContractSlotChargeKind kind) =>
        lines.Count(line => line.ChargeKind == kind);

    private static decimal SumKmOverage(
        ContractBillingSnapshot snapshot,
        InvoiceType invoiceType,
        InvoiceType extraKmRoute)
    {
        if (extraKmRoute != invoiceType)
            return 0m;

        return snapshot.ExtraKmsCount;
    }
}
