using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractBillingSnapshotDeletion
{
    public static void SoftDelete(ContractBillingSnapshot snapshot)
    {
        var now = DateTime.UtcNow;
        foreach (var day in snapshot.CalendarDays.Where(d => d.DeletedAt == null).ToList())
            day.SetDeletedAt(now);
        foreach (var line in snapshot.ChargeLines.Where(line => line.DeletedAt == null).ToList())
            line.SetDeletedAt(now);
        foreach (var km in snapshot.KmEvidence.Where(evidence => evidence.DeletedAt == null).ToList())
            km.SetDeletedAt(now);
        foreach (var row in snapshot.CommercialOverrides.Where(overrideRow => overrideRow.DeletedAt == null).ToList())
            row.SetDeletedAt(now);
        snapshot.SetDeletedAt(now);
    }
}
