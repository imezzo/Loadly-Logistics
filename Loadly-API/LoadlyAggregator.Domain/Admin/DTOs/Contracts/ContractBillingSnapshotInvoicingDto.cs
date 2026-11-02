using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;

namespace LoadlyAggregator.Domain.Admin.DTOs.Contracts;

public class ContractBillingSnapshotInvoicingDto
{
    public long? TripsInvoiceId { get; set; }
    public long? ClaimsInvoiceId { get; set; }
    public InvoiceType? ExtraKmOverageInvoiceType { get; set; }
    public bool RequiresTripsInvoice { get; set; }
    public bool RequiresClaimsInvoice { get; set; }
    public List<ContractBillingSnapshotSlotChargeInvoicingDto> SlotCharges { get; set; } = [];

    public static ContractBillingSnapshotInvoicingDto FromSnapshot(ContractBillingSnapshot snapshot) =>
        new()
        {
            TripsInvoiceId = snapshot.InvoiceId,
            ClaimsInvoiceId = snapshot.ClaimInvoiceId,
            ExtraKmOverageInvoiceType = snapshot.ExtraKmOverageInvoiceType,
            RequiresTripsInvoice = ContractBillingSnapshotInvoicingProgress.RequiresTripsInvoice(snapshot),
            RequiresClaimsInvoice = ContractBillingSnapshotInvoicingProgress.RequiresClaimsInvoice(snapshot),
            SlotCharges = BuildSlotChargeRows(snapshot)
        };

    private static List<ContractBillingSnapshotSlotChargeInvoicingDto> BuildSlotChargeRows(
        ContractBillingSnapshot snapshot)
    {
        var extraKmRoute = ContractSlotChargeInvoiceRouting.ResolveExtraKmOverageInvoiceType(snapshot);
        var candidates = new (ContractSlotChargeKind Kind, decimal Amount, InvoiceType RoutedTo)[]
        {
            (ContractSlotChargeKind.BaseWorkingDay, snapshot.WorkingDaysAmount, InvoiceType.Trips),
            (ContractSlotChargeKind.ExtraWorkingDay, snapshot.ExtraWorkingDaysAmount, InvoiceType.Trips),
            (ContractSlotChargeKind.ExtraKmOverage, snapshot.ExtraKmsAmount, extraKmRoute),
            (ContractSlotChargeKind.ExtraTrip, snapshot.ExtraTripsAmount, InvoiceType.Trips)
        };

        var rows = new List<ContractBillingSnapshotSlotChargeInvoicingDto>();
        foreach (var (kind, amount, routedTo) in candidates)
        {
            if (amount <= 0m)
                continue;

            var invoicedOn = routedTo == InvoiceType.Trips ? snapshot.InvoiceId : snapshot.ClaimInvoiceId;
            rows.Add(new ContractBillingSnapshotSlotChargeInvoicingDto
            {
                ChargeKind = kind,
                SnapshotAmount = amount,
                RoutedToInvoiceType = routedTo,
                IsInvoiced = invoicedOn is > 0,
                InvoicedOnInvoiceId = invoicedOn
            });
        }

        return rows;
    }
}

public class ContractBillingSnapshotSlotChargeInvoicingDto
{
    public ContractSlotChargeKind ChargeKind { get; set; }
    public decimal SnapshotAmount { get; set; }
    public InvoiceType RoutedToInvoiceType { get; set; }
    public bool IsInvoiced { get; set; }
    public long? InvoicedOnInvoiceId { get; set; }
}
