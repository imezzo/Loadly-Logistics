using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class InvoiceSlotChargeHeaderSync
{
    public static void ApplyFromSnapshot(Invoice invoice, ContractBillingSnapshot snapshot)
    {
        invoice.ContractBillingSnapshotId = snapshot.Id;
        var rollups = ContractBillingSnapshotSlotChargeRollups.ForInvoiceType(snapshot, invoice.InvoiceType);
        invoice.SlotChargesSubTotal = rollups.SubTotal;
        invoice.SlotChargesCalculatedAt = snapshot.FinalizedAt ?? DateTime.UtcNow;
        invoice.SlotWorkingDaysAmount = rollups.WorkingDaysAmount;
        invoice.SlotExtraWorkingDaysAmount = rollups.ExtraWorkingDaysAmount;
        invoice.SlotExtraKmsAmount = rollups.ExtraKmsAmount;
        invoice.SlotExtraTripsAmount = rollups.ExtraTripsAmount;
        invoice.SlotWorkingDaysCount = rollups.WorkingDaysCount;
        invoice.SlotExtraWorkingDaysCount = rollups.ExtraWorkingDaysCount;
        invoice.SlotExtraTripsCount = rollups.ExtraTripsCount;
        invoice.SlotExtraKmsCount = rollups.ExtraKmsCount;
    }
}
