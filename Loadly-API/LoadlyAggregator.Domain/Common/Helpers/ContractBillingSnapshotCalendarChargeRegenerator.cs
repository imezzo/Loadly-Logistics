using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

/// <summary>
/// Rebuilds snapshot charge lines and header rollups from persisted calendar days (after manual edits).
/// </summary>
public static class ContractBillingSnapshotCalendarChargeRegenerator
{
    public static void RebuildCharges(
        ContractBillingSnapshot snapshot,
        Contract contract,
        IReadOnlyDictionary<long, IReadOnlyList<Trip>> tripsBySlotId,
        ContractWorkingDayWindow workingDayWindow)
    {
        var monthlyWorkingDays = ContractCalculationHelper.ResolveMonthlyWorkingDays(
            contract.MonthlyWorkingDays,
            contract.ContractType) ?? ContractCalculationHelper.DefaultMonthlyWorkingDays;

        var periodStartDate = snapshot.PeriodStart;
        var periodEndDate = snapshot.PeriodEnd;
        var lines = new List<IncludedContractSlotChargeLine>();
        var extraTripsCount = 0;
        var workingDaysCount = 0;
        var extraWorkingDaysCount = 0;
        var seats = IncludedContractSlotSeatHelper.GroupIntoSeats(contract.ContractSlots.ToList());
        var billedSeats = new List<IncludedContractSlotSeat>();
        var includeKmOverage = contract.ContractType == ContractType.Included;
        var usePooledKmOverage = includeKmOverage
                                 && ContractCalculationHelper.ResolveIncludedKmOverageMode(contract.KmOverageMode)
                                 == ContractKmOverageMode.PooledAcrossSlots;
        var commercialOverrides = snapshot.CommercialOverrides
            .Where(row => row.DeletedAt == null)
            .ToList();

        foreach (var seat in seats)
        {
            var slotIds = seat.SlotsInOrder.Select(s => s.Id).ToHashSet();
            var seatDays = snapshot.CalendarDays
                .Where(day => day.DeletedAt == null && slotIds.Contains(day.ContractSlotId))
                .ToList();
            if (seatDays.Count == 0)
                continue;

            billedSeats.Add(seat);

            ContractSlotBillingRates? ResolveRatesForDate(DateOnly date) =>
                ContractBillingSnapshotCommercialTermsResolver.ResolveRatesForDate(
                    commercialOverrides,
                    seat.SeatRootId,
                    date);

            foreach (var (monthStart, monthEnd) in EnumerateCalendarMonths(periodStartDate, periodEndDate))
            {
                var billingStart = monthStart > periodStartDate ? monthStart : periodStartDate;
                var billingEnd = monthEnd < periodEndDate ? monthEnd : periodEndDate;
                var monthlyWorkingDayAllowance = IncludedContractSlotSeatHelper.ResolveMonthlyWorkingDayAllowance(
                    monthlyWorkingDays,
                    seat.SlotsInOrder,
                    monthStart,
                    monthEnd);
                var termsSlot = IncludedContractSlotSeatHelper.ResolveSeatMonthTermsSlot(
                    seat.SlotsInOrder,
                    monthStart,
                    monthEnd);
                var monthlyMaxKmsAllowance = IncludedContractSlotSeatHelper.ResolveMonthlyMaxKmsAllowance(
                    termsSlot.MaxKms,
                    seat.SlotsInOrder,
                    monthStart,
                    monthEnd);
                var usesSharedKm = IncludedContractSlotSeatHelper.UsesSharedMonthlyAllowance(
                    seat.SlotsInOrder,
                    monthStart,
                    monthEnd);

                // The working-day sequence continues across a seat's replacement chain within a month,
                // matching the offset used by IncludedContractSlotChargesComputer.
                var workingDayIndexOffset = 0;

                foreach (var slot in seat.SlotsInOrder)
                {
                    if (!ContractSlotBillingLoadCriteriaBuilder.ShouldIncludeSlot(slot))
                        continue;

                    var slotDays = seatDays
                        .Where(day => day.ContractSlotId == slot.Id && day.Date >= billingStart && day.Date <= billingEnd)
                        .OrderBy(day => day.Date)
                        .ToList();
                    if (slotDays.Count == 0)
                        continue;

                    var maxWorkingDayIndex = ReindexWorkingDays(
                        slotDays,
                        monthlyWorkingDayAllowance,
                        workingDayIndexOffset);
                    if (IncludedContractSlotSeatHelper.CountActiveDaysInMonth(slot, monthStart, monthEnd) > 0)
                        workingDayIndexOffset = maxWorkingDayIndex;

                    var evaluations = slotDays
                        .Select(day => new ContractSlotDayEvaluation(
                            day.Date,
                            day.Disposition,
                            day.WorkingDayIndex,
                            day.FacilityType))
                        .ToList();

                    IncludedContractSlotChargeRules.AccumulateDayCountsForBillingPeriod(
                        evaluations,
                        billingStart,
                        billingEnd,
                        monthlyWorkingDayAllowance,
                        ref workingDaysCount,
                        ref extraWorkingDaysCount);

                    var slotTrips = tripsBySlotId.GetValueOrDefault(slot.Id) ?? [];
                    lines.AddRange(IncludedContractSlotChargeRules.BuildForMonth(
                        slot,
                        monthStart,
                        monthEnd,
                        billingStart,
                        billingEnd,
                        monthlyWorkingDayAllowance,
                        evaluations,
                        slotTrips,
                        contract.ContractType,
                        workingDayWindow,
                        includeKmOverageLine: includeKmOverage && !usePooledKmOverage && !usesSharedKm,
                        monthlyMaxKmsAllowance: monthlyMaxKmsAllowance,
                        resolveRatesForDate: ResolveRatesForDate));

                    if (contract.ContractType == ContractType.Excluded)
                    {
                        var (extraTripLines, dayExtraTrips) = IncludedContractSlotChargeRules.BuildExtraTripChargesForMonth(
                            slot,
                            billingStart,
                            billingEnd,
                            evaluations,
                            slotTrips,
                            resolveTripCountForDate: date => ResolveManualTripCount(slotDays, date),
                            resolveRatesForDate: ResolveRatesForDate);
                        lines.AddRange(extraTripLines);
                        extraTripsCount += dayExtraTrips;
                    }
                }

                var seatTrips = seat.SlotsInOrder
                    .SelectMany(s => tripsBySlotId.GetValueOrDefault(s.Id) ?? [])
                    .DistinctBy(trip => trip.Id)
                    .ToList();
                var swapBillings = IncludedContractSlotSeatHelper.ResolveSameDayReplacementBillings(
                    seat.SlotsInOrder,
                    slotId => tripsBySlotId.GetValueOrDefault(slotId) ?? []);
                foreach (var swap in swapBillings)
                {
                    if (swap.ExtraOwnerSlotId is not long extraOwnerSlotId)
                        continue;

                    if (swap.Date < billingStart || swap.Date > billingEnd)
                        continue;

                    var extraOwnerSlot = seat.SlotsInOrder.First(s => s.Id == extraOwnerSlotId);
                    var swapRates = ResolveRatesForDate(swap.Date);
                    if (contract.ContractType == ContractType.Included)
                    {
                        if (IncludedContractSlotChargeRules.TryBuildSwapExtraWorkingDayLine(
                                extraOwnerSlot,
                                swap.Date,
                                contract.ContractType,
                                out var swapLine,
                                swapRates))
                        {
                            lines.Add(swapLine);
                            extraWorkingDaysCount++;
                        }
                    }
                    else if (IncludedContractSlotChargeRules.TryBuildSwapExtraTripLine(
                                 extraOwnerSlot,
                                 swap.Date,
                                 seatTrips,
                                 out var swapTripLine,
                                 out var swapExtraTrips,
                                 swapRates,
                                 date => ResolveManualTripCount(
                                     seatDays.Where(day => day.ContractSlotId == extraOwnerSlotId),
                                     date)))
                    {
                        lines.Add(swapTripLine);
                        extraTripsCount += swapExtraTrips;
                    }
                }

                if (includeKmOverage
                    && !usePooledKmOverage
                    && usesSharedKm
                    && IncludedContractSlotChargeRules.TryBuildSeatKmOverageLine(
                        termsSlot,
                        monthStart,
                        monthEnd,
                        billingStart,
                        billingEnd,
                        seat.SlotsInOrder
                            .SelectMany(s => tripsBySlotId.GetValueOrDefault(s.Id) ?? [])
                            .DistinctBy(t => t.Id)
                            .ToList(),
                        monthlyMaxKmsAllowance,
                        workingDayWindow,
                        out var seatKmLine))
                {
                    lines.Add(seatKmLine);
                }
            }
        }

        if (usePooledKmOverage && billedSeats.Count > 0)
        {
            var (pooledLines, _) = IncludedContractSlotChargeRules.BuildPooledKmOverageForPeriod(
                billedSeats,
                tripsBySlotId,
                periodStartDate,
                periodEndDate,
                workingDayWindow,
                IncludedContractSlotChargeRules.ResolveContractedPoolMonthlyMaxKms(contract.ContractVehicleTypes));
            lines.AddRange(pooledLines);
        }

        var now = DateTime.UtcNow;
        foreach (var line in snapshot.ChargeLines.Where(l => l.DeletedAt == null).ToList())
            line.SetDeletedAt(now);

        foreach (var line in lines)
        {
            snapshot.ChargeLines.Add(new ContractBillingSnapshotChargeLine
            {
                Snapshot = snapshot,
                SnapshotId = snapshot.Id,
                ContractSlotId = line.ContractSlotId > 0 ? line.ContractSlotId : null,
                Date = line.Date,
                ChargeKind = line.Kind,
                Amount = line.Amount
            });
        }

        // KM evidence, km_pool_json and ExtraKmsCount depend only on trips, not on calendar dispositions,
        // so the values persisted at create/refresh stay authoritative here.
        ApplyHeaderTotals(snapshot, lines, workingDaysCount, extraWorkingDaysCount, extraTripsCount);
        ContractBillingSnapshotMaterializer.EnsureIntegrity(snapshot);
    }

    private static int? ResolveManualTripCount(
        IEnumerable<ContractBillingSnapshotCalendarDay> days,
        DateOnly date)
    {
        var day = days.FirstOrDefault(candidate =>
            candidate.Date == date
            && candidate.TripCountSource == ContractBillingSnapshotDispositionSource.Manual);
        return day is null ? null : day.TripIds.Length;
    }

    /// <returns>The highest index assigned, or 0 when the slot has no working days (same as GetMaxWorkingDayIndex).</returns>
    private static int ReindexWorkingDays(
        IList<ContractBillingSnapshotCalendarDay> orderedDays,
        decimal monthlyWorkingDayAllowance,
        int indexOffset)
    {
        var workingIndex = indexOffset;
        var maxIndex = 0;
        foreach (var day in orderedDays.OrderBy(d => d.Date))
        {
            if (!IncludedContractHalfWorkingDayRules.ParticipatesInWorkingDayIndex(day.Disposition))
            {
                day.WorkingDayIndex = null;
                day.IsExtra = false;
                continue;
            }

            workingIndex++;
            maxIndex = workingIndex;
            day.WorkingDayIndex = workingIndex;
            day.IsExtra = workingIndex > monthlyWorkingDayAllowance;
        }

        return maxIndex;
    }

    private static void ApplyHeaderTotals(
        ContractBillingSnapshot snapshot,
        IReadOnlyList<IncludedContractSlotChargeLine> lines,
        int workingDaysCount,
        int extraWorkingDaysCount,
        int extraTripsCount)
    {
        snapshot.SubTotal = lines.Sum(l => l.Amount);
        snapshot.WorkingDaysAmount = lines
            .Where(l => l.Kind == ContractSlotChargeKind.BaseWorkingDay)
            .Sum(l => l.Amount);
        snapshot.ExtraWorkingDaysAmount = lines
            .Where(l => l.Kind == ContractSlotChargeKind.ExtraWorkingDay)
            .Sum(l => l.Amount);
        snapshot.ExtraKmsAmount = lines
            .Where(l => l.Kind == ContractSlotChargeKind.ExtraKmOverage)
            .Sum(l => l.Amount);
        snapshot.ExtraTripsAmount = lines
            .Where(l => l.Kind == ContractSlotChargeKind.ExtraTrip)
            .Sum(l => l.Amount);
        snapshot.WorkingDaysCount = workingDaysCount;
        snapshot.ExtraWorkingDaysCount = extraWorkingDaysCount;
        snapshot.ExtraTripsCount = extraTripsCount;
    }

    private static IEnumerable<(DateOnly MonthStart, DateOnly MonthEnd)> EnumerateCalendarMonths(
        DateOnly periodStart,
        DateOnly periodEnd)
    {
        var cursor = new DateOnly(periodStart.Year, periodStart.Month, 1);
        while (cursor <= periodEnd)
        {
            var monthStart = cursor;
            var monthEnd = cursor.AddMonths(1).AddDays(-1);
            yield return (monthStart, monthEnd);
            cursor = cursor.AddMonths(1);
        }
    }
}
