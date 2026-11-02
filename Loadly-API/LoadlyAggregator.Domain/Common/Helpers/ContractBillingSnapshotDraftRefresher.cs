using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractBillingSnapshotDraftRefresher
{
    public static async Task RefreshPreservingManualOverridesAsync(
        ContractBillingSnapshot snapshot,
        CalculateIncludedContractSlotChargesResultDto calculation,
        Contract contract,
        ITripRepository tripRepository)
    {
        var manualDays = snapshot.CalendarDays
            .Where(day => day.DeletedAt == null
                          && day.DispositionSource == ContractBillingSnapshotDispositionSource.Manual)
            .Select(day => (
                day.ContractSlotId,
                day.Date,
                day.Disposition,
                day.OverrideReason,
                day.FacilityId,
                day.FacilityType,
                day.ChangedBy,
                day.ChangedAt))
            .ToList();
        var manualTripCounts = snapshot.CalendarDays
            .Where(day => day.DeletedAt == null
                          && day.TripCountSource == ContractBillingSnapshotDispositionSource.Manual)
            .Select(day => (day.ContractSlotId, day.Date, TripIds: day.TripIds.ToArray(), day.OverrideReason))
            .ToList();

        ContractBillingSnapshotMaterializer.ReplaceChildren(snapshot, calculation);

        foreach (var manual in manualDays)
        {
            var replacement = FindReplacementDay(snapshot, manual.ContractSlotId, manual.Date);
            if (replacement is null)
                continue;

            replacement.Disposition = manual.Disposition;
            replacement.DispositionSource = ContractBillingSnapshotDispositionSource.Manual;
            replacement.OverrideReason = manual.OverrideReason;
            replacement.ChangedBy = manual.ChangedBy;
            replacement.ChangedAt = manual.ChangedAt;
            var keepsFacility = manual.Disposition is ContractSlotDayDisposition.Skipped
                or ContractSlotDayDisposition.Working
                or ContractSlotDayDisposition.HalfWorking;
            replacement.FacilityId = keepsFacility ? manual.FacilityId : null;
            replacement.FacilityType = keepsFacility ? manual.FacilityType : null;
        }

        foreach (var manual in manualTripCounts)
        {
            var replacement = FindReplacementDay(snapshot, manual.ContractSlotId, manual.Date);
            if (replacement is null)
                continue;

            replacement.TripIds = manual.TripIds;
            replacement.TripCountSource = ContractBillingSnapshotDispositionSource.Manual;
            if (!manualDays.Any(day => day.ContractSlotId == manual.ContractSlotId && day.Date == manual.Date))
                replacement.OverrideReason = manual.OverrideReason;
        }

        var hasCommercialOverrides = snapshot.CommercialOverrides.Any(row => row.DeletedAt == null);
        if (manualDays.Count > 0 || manualTripCounts.Count > 0 || hasCommercialOverrides)
        {
            await ContractBillingSnapshotChargeRebuildHelper.RebuildDraftChargesAsync(
                snapshot,
                contract,
                tripRepository);
        }
    }

    public static void ApplyName(
        ContractBillingSnapshot snapshot,
        Contract contract,
        string? requestedName)
    {
        if (!string.IsNullOrWhiteSpace(requestedName))
            snapshot.Name = ContractBillingSnapshotNameFormatter.Resolve(contract, requestedName);
        else if (string.IsNullOrWhiteSpace(snapshot.Name))
            snapshot.Name = ContractBillingSnapshotNameFormatter.BuildDefault(contract);
    }

    private static ContractBillingSnapshotCalendarDay? FindReplacementDay(
        ContractBillingSnapshot snapshot,
        long contractSlotId,
        DateOnly date) =>
        snapshot.CalendarDays.FirstOrDefault(day =>
            day.DeletedAt == null
            && day.ContractSlotId == contractSlotId
            && day.Date == date);
}
