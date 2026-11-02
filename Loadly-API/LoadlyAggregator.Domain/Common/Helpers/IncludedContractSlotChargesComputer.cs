using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Extensions;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Common.Helpers;

internal readonly record struct SlotBillingContext(
    ContractSlot Slot,
    ContractSlot BillingSlot,
    IReadOnlyList<Trip> Trips,
    IReadOnlyList<VehicleFacilityVisit> Visits,
    IReadOnlyDictionary<long, VehicleFacilityBillingInfo> FacilityById);

public static class IncludedContractSlotChargesComputer
{
    public static async Task<CalculateIncludedContractSlotChargesResultDto> ComputeAsync(
        long contractId,
        IReadOnlyList<ContractSlot> slots,
        DateOnly periodStart,
        DateOnly periodEnd,
        int monthlyWorkingDays,
        ContractWorkingDayWindow workingDayWindow,
        ContractKmOverageMode kmOverageMode,
        Func<ContractSlot, ContractSlot> toBillingSlot,
        ITripRepository tripRepository,
        IVehicleFacilityVisitRepository visitRepository,
        IReadOnlyList<ContractSlot> allContractSlots,
        IReadOnlyList<ContractVehicleType> contractVehicleTypes,
        ContractType contractType = ContractType.Included)
    {
        var includeKmOverage = contractType == ContractType.Included;
        var usePooledKmOverage = includeKmOverage && kmOverageMode == ContractKmOverageMode.PooledAcrossSlots;

        var allLines = new List<IncludedContractSlotChargeLine>();
        var kmsSummaries = new List<IncludedContractSlotKmSummaryDto>();
        var calendar = new List<IncludedContractSlotCalendarDto>();
        var tripsBySlotId = new Dictionary<long, IReadOnlyList<Trip>>();
        var totalTripsCount = 0;
        var totalWorkingDaysCount = 0;
        var totalExtraWorkingDaysCount = 0;
        var totalExtraTripsCount = 0;
        decimal totalExtraKmsCount = 0m;

        var seats = IncludedContractSlotSeatHelper.GroupIntoSeats(slots);
        var seatContexts = new List<(IncludedContractSlotSeat Seat, List<SlotBillingContext> Slots)>();

        var loadCriteria = new List<ContractSlotBillingLoadCriteria>();
        var billingSlotsBySlotId = new Dictionary<long, (ContractSlot Slot, ContractSlot BillingSlot)>();
        // Resolved from every slot on the contract, not just the billed ones: a single-slot calculation still
        // needs its predecessors to work out how far back the vehicle's facility history must be read.
        var allSlotsById = allContractSlots.ToDictionary(s => s.Id);

        foreach (var seat in seats)
        {
            foreach (var slot in seat.SlotsInOrder)
            {
                if (!ContractSlotBillingLoadCriteriaBuilder.ShouldIncludeSlot(slot))
                    continue;

                var billingSlot = toBillingSlot(slot);
                billingSlotsBySlotId[slot.Id] = (slot, billingSlot);
                var chainAssignedAt = IncludedContractSlotSeatHelper.ResolveChainAssignedAt(slot, allSlotsById);
                loadCriteria.Add(ContractSlotBillingLoadCriteriaBuilder.Create(
                    slot,
                    billingSlot,
                    chainAssignedAt));
            }
        }

        var visitLoadEnd = workingDayWindow.ToUtcRange(periodEnd).End;
        var loadedTripsBySlotId = await tripRepository.GetForContractSlotBillingBySlotsAsync(
            contractId,
            loadCriteria,
            periodStart,
            periodEnd);
        var loadedVisitsBySlotId = await visitRepository.GetForContractSlotBillingBySlotsAsync(
            loadCriteria,
            visitLoadEnd);

        foreach (var seat in seats)
        {
            var slotContexts = new List<SlotBillingContext>();
            foreach (var slot in seat.SlotsInOrder)
            {
                if (!billingSlotsBySlotId.TryGetValue(slot.Id, out var slotEntry))
                    continue;

                var billingSlot = slotEntry.BillingSlot;
                var slotTrips = loadedTripsBySlotId.GetValueOrDefault(slot.Id) ?? [];
                tripsBySlotId[slot.Id] = slotTrips;

                totalTripsCount += slotTrips.Count(t =>
                    t.Status == TripStatus.Completed
                    && t.FinancialDate >= periodStart
                    && t.FinancialDate <= periodEnd);

                var visits = loadedVisitsBySlotId.GetValueOrDefault(slot.Id) ?? [];
                var facilityById = visits
                    .Where(v => v.VehicleFacility is not null)
                    .GroupBy(v => v.VehicleFacilityId)
                    .ToDictionary(
                        g => g.Key,
                        g => new VehicleFacilityBillingInfo(
                            g.First().VehicleFacility!.FacilityType,
                            g.First().VehicleFacility!.Name));

                slotContexts.Add(new SlotBillingContext(
                    slot,
                    billingSlot,
                    slotTrips,
                    visits,
                    facilityById));
            }

            if (slotContexts.Count == 0)
                continue;

            seatContexts.Add((seat, slotContexts));
            var seatTrips = slotContexts
                .SelectMany(context => context.Trips)
                .DistinctBy(trip => trip.Id)
                .ToList();
            var swapBillings = IncludedContractSlotSeatHelper.ResolveSameDayReplacementBillings(
                seat.SlotsInOrder,
                slotId => tripsBySlotId.GetValueOrDefault(slotId) ?? []);
            var swapBillingByPredecessorId = swapBillings.ToDictionary(billing => billing.PredecessorSlotId);
            var swapBillingBySuccessorId = swapBillings.ToDictionary(billing => billing.SuccessorSlotId);
            var slotContextsById = slotContexts.ToDictionary(entry => entry.Slot.Id);
            var calendarDaysBySlotId = new Dictionary<long, List<IncludedContractSlotCalendarDayDto>>();

            foreach (var context in slotContexts)
            {
                if (includeKmOverage)
                {
                    // Pooled overage lives only on the pool row; per-slot rows carry consumption only.
                    var isPrimaryOverageSlot = !usePooledKmOverage && context.Slot.Id == slotContexts[^1].Slot.Id;
                    var kmSummary = IncludedContractSlotChargeRules.BuildKmSummaryForPeriod(
                        context.BillingSlot,
                        periodStart,
                        periodEnd,
                        workingDayWindow,
                        context.Trips,
                        seat.SlotsInOrder,
                        seatTrips);
                    kmsSummaries.Add(new IncludedContractSlotKmSummaryDto
                    {
                        SlotId = kmSummary.ContractSlotId,
                        Consumption = kmSummary.Consumption,
                        OverageKm = isPrimaryOverageSlot ? kmSummary.OverageKm : 0m,
                        OverConsumptionRate = kmSummary.OverConsumptionRate,
                        SuggestedOverageAmount = isPrimaryOverageSlot ? kmSummary.SuggestedOverageAmount : 0m
                    });
                }
            }

            foreach (var (monthStart, monthEnd) in EnumerateCalendarMonths(periodStart, periodEnd))
            {
                var billingStart = monthStart > periodStart ? monthStart : periodStart;
                var billingEnd = monthEnd < periodEnd ? monthEnd : periodEnd;
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
                var usesSharedMonthlyAllowance = IncludedContractSlotSeatHelper.UsesSharedMonthlyAllowance(
                    seat.SlotsInOrder,
                    monthStart,
                    monthEnd);

                var workingDayIndexOffset = 0;
                ContractSlot? seatKmOverageSlot = null;

                foreach (var context in slotContexts)
                {
                    if (IncludedContractSlotSeatHelper.CountActiveDaysInMonth(context.Slot, monthStart, monthEnd) == 0)
                        continue;

                    seatKmOverageSlot = context.BillingSlot;
                    var assignedAtUtc = context.BillingSlot.AssignedAt;
                    var assignedDate = EgyptTime.ToCalendarDate(assignedAtUtc);
                    DateTime? archivedAtUtc = context.Slot.ArchivedAt;
                    DateOnly? archivedDate = archivedAtUtc is { } archivedAt
                        ? EgyptTime.ToCalendarDate(archivedAt)
                        : null;

                    // On a same-day replacement the predecessor stops billing the day before the swap
                    // whenever it does not own the base day (its vehicle had no trip that day).
                    if (swapBillingByPredecessorId.TryGetValue(context.Slot.Id, out var predecessorSwap)
                        && predecessorSwap.BaseOwnerSlotId != context.Slot.Id
                        && archivedDate is { } predecessorArchivedDate)
                    {
                        archivedDate = predecessorArchivedDate.AddDays(-1);
                    }

                    // The successor loses its base day on the swap date when the predecessor owns it.
                    HashSet<DateOnly>? nonBillableDates = null;
                    if (swapBillingBySuccessorId.TryGetValue(context.Slot.Id, out var successorSwap)
                        && successorSwap.BaseOwnerSlotId != context.Slot.Id)
                    {
                        nonBillableDates = [successorSwap.Date];
                    }

                    // The base owner shows the aggregated seat trips on the swap date in its calendar row.
                    var seatAggregateDates = ResolveSeatAggregateDates(
                        context.Slot.Id,
                        predecessorSwap,
                        successorSwap,
                        swapBillingByPredecessorId.ContainsKey(context.Slot.Id),
                        swapBillingBySuccessorId.ContainsKey(context.Slot.Id));

                    DateOnly? successorSwapDate = null;
                    SameDaySwapPredecessorDayContext? swapPredecessor = null;
                    if (swapBillingBySuccessorId.TryGetValue(context.Slot.Id, out var successorSwapForInheritance)
                        && nonBillableDates?.Contains(successorSwapForInheritance.Date) != true
                        && slotContextsById.TryGetValue(
                            ResolveSwapDispositionSourceSlotId(
                                successorSwapForInheritance,
                                swapBillingBySuccessorId,
                                slotContextsById),
                            out var predecessorContext))
                    {
                        successorSwapDate = successorSwapForInheritance.Date;
                        swapPredecessor = new SameDaySwapPredecessorDayContext(
                            EgyptTime.ToCalendarDate(predecessorContext.BillingSlot.AssignedAt),
                            predecessorContext.Slot.ArchivedAt is { } predecessorArchivedAt
                                ? EgyptTime.ToCalendarDate(predecessorArchivedAt)
                                : null,
                            predecessorContext.BillingSlot.AssignedAt,
                            predecessorContext.Slot.ArchivedAt,
                            predecessorContext.Slot.VehicleId,
                            predecessorContext.Trips,
                            predecessorContext.Visits,
                            predecessorContext.FacilityById);
                    }

                    var evaluations = ContractSlotDayClassifier.EvaluatePeriod(
                        monthStart,
                        monthEnd,
                        assignedDate,
                        archivedDate,
                        assignedAtUtc,
                        archivedAtUtc,
                        context.Slot.VehicleId,
                        context.Trips,
                        context.Visits,
                        context.FacilityById,
                        workingDayWindow,
                        workingDayIndexOffset,
                        nonBillableDates,
                        successorSwapDate,
                        swapPredecessor);

                    IncludedContractSlotChargeRules.AccumulateDayCountsForBillingPeriod(
                        evaluations,
                        billingStart,
                        billingEnd,
                        monthlyWorkingDayAllowance,
                        ref totalWorkingDaysCount,
                        ref totalExtraWorkingDaysCount);

                    var slotCalendarDays = IncludedContractSlotChargeRules.BuildCalendarDaysForBillingPeriod(
                            evaluations,
                            billingStart,
                            billingEnd,
                            monthlyWorkingDayAllowance,
                            context.Trips,
                            seatTrips,
                            seatAggregateDates)
                        .Select(day => new IncludedContractSlotCalendarDayDto
                        {
                            Date = day.Date,
                            Disposition = day.Disposition,
                            TripCount = day.TripCount,
                            TripIds = day.TripIds,
                            WorkingDayIndex = day.WorkingDayIndex,
                            IsExtra = day.IsExtra,
                            FacilityType = day.FacilityType,
                            FacilityId = day.FacilityId,
                            FacilityName = day.FacilityName
                        })
                        .ToList();

                    if (!calendarDaysBySlotId.TryGetValue(context.Slot.Id, out var calendarDays))
                    {
                        calendarDays = [];
                        calendarDaysBySlotId[context.Slot.Id] = calendarDays;
                    }

                    calendarDays.AddRange(slotCalendarDays);

                    allLines.AddRange(IncludedContractSlotChargeRules.BuildForMonth(
                        context.BillingSlot,
                        monthStart,
                        monthEnd,
                        billingStart,
                        billingEnd,
                        monthlyWorkingDayAllowance,
                        evaluations,
                        context.Trips,
                        contractType,
                        workingDayWindow,
                        includeKmOverageLine: includeKmOverage && !usePooledKmOverage && !usesSharedMonthlyAllowance,
                        monthlyMaxKmsAllowance: monthlyMaxKmsAllowance));

                    if (contractType == ContractType.Excluded)
                    {
                        var (extraTripLines, extraTripCount) = IncludedContractSlotChargeRules.BuildExtraTripChargesForMonth(
                            context.BillingSlot,
                            billingStart,
                            billingEnd,
                            evaluations,
                            context.Trips);
                        allLines.AddRange(extraTripLines);
                        totalExtraTripsCount += extraTripCount;
                    }

                    workingDayIndexOffset = IncludedContractSlotChargeRules.GetMaxWorkingDayIndex(evaluations);
                }

                // Same-day replacement overflow: when both vehicles had a trip on the swap day, the successor
                // owns one extra charge on top of the predecessor's base day (extra working day for Included,
                // extra trip for Excluded).
                foreach (var swap in swapBillings)
                {
                    if (swap.ExtraOwnerSlotId is not long extraOwnerSlotId)
                        continue;

                    if (swap.Date < billingStart || swap.Date > billingEnd)
                        continue;

                    var extraContext = slotContexts.First(entry => entry.Slot.Id == extraOwnerSlotId);
                    if (contractType == ContractType.Excluded)
                    {
                        AppendSwapExtraTripLine(allLines, extraContext.BillingSlot, swap.Date, seatTrips, ref totalExtraTripsCount);
                    }
                    else
                    {
                        AppendSwapExtraWorkingDayLine(allLines, extraContext.BillingSlot, swap.Date, ref totalExtraWorkingDaysCount);
                    }
                }

                if (includeKmOverage
                    && !usePooledKmOverage
                    && usesSharedMonthlyAllowance
                    && seatKmOverageSlot is not null
                    && IncludedContractSlotChargeRules.TryBuildSeatKmOverageLine(
                        seatKmOverageSlot,
                        monthStart,
                        monthEnd,
                        billingStart,
                        billingEnd,
                        seatTrips,
                        monthlyMaxKmsAllowance,
                        workingDayWindow,
                        out var seatKmLine))
                {
                    allLines.Add(seatKmLine);
                }
            }

            foreach (var (slotId, days) in calendarDaysBySlotId)
            {
                calendar.Add(new IncludedContractSlotCalendarDto
                {
                    SlotId = slotId,
                    Days = days.OrderBy(day => day.Date).ToList()
                });
            }
        }

        if (includeKmOverage && !usePooledKmOverage)
        {
            totalExtraKmsCount = kmsSummaries.Sum(summary => summary.OverageKm);
        }

        IncludedContractKmPoolDto? kmPool = null;
        if (includeKmOverage && usePooledKmOverage && seatContexts.Count > 0)
        {
            var (pooledLines, poolSummary) = IncludedContractSlotChargeRules.BuildPooledKmOverageForPeriod(
                seatContexts.Select(entry => entry.Seat).ToList(),
                tripsBySlotId,
                periodStart,
                periodEnd,
                workingDayWindow,
                IncludedContractSlotChargeRules.ResolveContractedPoolMonthlyMaxKms(contractVehicleTypes));
            allLines.AddRange(pooledLines);
            totalExtraKmsCount = poolSummary.OverageKm;
            kmPool = new IncludedContractKmPoolDto
            {
                TotalConsumption = poolSummary.TotalConsumption,
                TotalMaxKms = poolSummary.TotalMaxKms,
                OverageKm = poolSummary.OverageKm,
                OverConsumptionRate = poolSummary.OverConsumptionRate,
                SuggestedOverageAmount = poolSummary.SuggestedOverageAmount
            };
        }

        var lineDtos = allLines
            .Select(line => new IncludedContractSlotChargeLineDto
            {
                ContractSlotId = line.ContractSlotId,
                Date = line.Date,
                ChargeKind = (int)line.Kind,
                Amount = line.Amount
            })
            .ToList();

        var result = new CalculateIncludedContractSlotChargesResultDto
        {
            Lines = lineDtos,
            Kms = kmsSummaries,
            Calendar = calendar,
            KmPool = kmPool,
            TotalAmount = lineDtos.Sum(l => l.Amount),
            TotalWorkingDaysAmount = lineDtos
                .Where(l => l.ChargeKind == (int)ContractSlotChargeKind.BaseWorkingDay)
                .Sum(l => l.Amount),
            TotalExtraWorkingDaysAmount = lineDtos
                .Where(l => l.ChargeKind == (int)ContractSlotChargeKind.ExtraWorkingDay)
                .Sum(l => l.Amount),
            TotalExtraKmsAmount = lineDtos
                .Where(l => l.ChargeKind == (int)ContractSlotChargeKind.ExtraKmOverage)
                .Sum(l => l.Amount),
            TotalExtraTripsAmount = lineDtos
                .Where(l => l.ChargeKind == (int)ContractSlotChargeKind.ExtraTrip)
                .Sum(l => l.Amount),
            TotalTripsCount = totalTripsCount,
            TotalDaysWithTripsCount = calendar
                .SelectMany(entry => entry.Days)
                .Count(day => day.TripCount > 0),
            TotalWorkingDaysCount = totalWorkingDaysCount,
            TotalExtraWorkingDaysCount = totalExtraWorkingDaysCount,
            TotalExtraTripsCount = totalExtraTripsCount,
            TotalExtraKmsCount = totalExtraKmsCount
        };

        ContractSlotSeatRootMapper.Apply(result, allContractSlots);
        var startDateByTripId = tripsBySlotId
            .SelectMany(pair => pair.Value)
            .GroupBy(trip => trip.Id)
            .ToDictionary(group => group.Key, group => group.First().StartDate);
        ContractBillingCalendarDayTripCosts.Apply(
            result,
            billingSlotsBySlotId.Values.Select(entry => entry.BillingSlot),
            contractVehicleTypes,
            startDateByTripId);
        ContractBillingSnapshotReader.ApplyWaitingForLoadingTotals(result);
        await ContractBillingCalendarDayTripCosts.ApplyTripTotalsAsync(result, tripRepository);
        return result;
    }

    public static IReadOnlyList<ContractSlot> ResolveTargetSlots(
        Contract contract,
        long? contractSlotId,
        DateOnly periodStart,
        DateOnly periodEnd)
    {
        if (contractSlotId is > 0)
        {
            var slot = contract.ContractSlots.FirstOrDefault(s => s.Id == contractSlotId.Value)
                ?? throw new Shared.Exceptions.NotFoundException("contracts.contract_slot_not_found", contractSlotId.Value);

            return [slot];
        }

        return contract.ContractSlots
            .Where(s => SlotOverlapsPeriod(s, periodStart, periodEnd))
            .ToList();
    }

    public static bool SlotOverlapsPeriod(ContractSlot slot, DateOnly periodStart, DateOnly periodEnd)
    {
        var assignedDate = EgyptTime.ToCalendarDate(slot.AssignedAt);
        if (assignedDate > periodEnd)
            return false;

        if (slot.ArchivedAt is DateTime archivedAt && EgyptTime.ToCalendarDate(archivedAt) < periodStart)
            return false;

        if (slot.VehicleId is > 0)
            return true;

        // Archived rows may have vehicle cleared; trips can remain pinned by contract_slot_id.
        return slot.ArchivedAt.HasValue;
    }

    private static long ResolveSwapDispositionSourceSlotId(
        SameDayReplacementBilling ownerLink,
        IReadOnlyDictionary<long, SameDayReplacementBilling> swapBillingBySuccessorId,
        IReadOnlyDictionary<long, SlotBillingContext> slotContextsById)
    {
        var cursorId = ownerLink.PredecessorSlotId;
        var visited = 0;

        while (visited++ < 32 && slotContextsById.TryGetValue(cursorId, out var context))
        {
            if (ContractSlotDayClassifier.HasOwnSwapDaySignal(
                    ownerLink.Date,
                    context.Slot.VehicleId,
                    context.Trips,
                    context.Visits,
                    context.FacilityById))
                return cursorId;

            if (!swapBillingBySuccessorId.TryGetValue(cursorId, out var earlierLink)
                || earlierLink.Date != ownerLink.Date)
                break;

            cursorId = earlierLink.PredecessorSlotId;
        }

        return ownerLink.PredecessorSlotId;
    }

    private static HashSet<DateOnly>? ResolveSeatAggregateDates(
        long slotId,
        SameDayReplacementBilling predecessorSwap,
        SameDayReplacementBilling successorSwap,
        bool isPredecessor,
        bool isSuccessor)
    {
        HashSet<DateOnly>? dates = null;

        if (isPredecessor && predecessorSwap.BaseOwnerSlotId == slotId)
            (dates ??= []).Add(predecessorSwap.Date);

        if (isSuccessor && successorSwap.BaseOwnerSlotId == slotId)
            (dates ??= []).Add(successorSwap.Date);

        return dates;
    }

    private static void AppendSwapExtraTripLine(
        List<IncludedContractSlotChargeLine> lines,
        ContractSlot billingSlot,
        DateOnly swapDate,
        IReadOnlyList<Trip> seatTrips,
        ref int totalExtraTripsCount)
    {
        if (billingSlot.TripsPerDay is not int tripsPerDay || tripsPerDay <= 0)
            return;

        var rate = billingSlot.OverConsumption ?? 0m;
        if (rate <= 0m)
            return;

        var seatTripCount = ContractSlotDayClassifier.CountCompletedTripsOnDate(seatTrips, swapDate);
        var extraTrips = seatTripCount - tripsPerDay;
        if (extraTrips <= 0)
            return;

        lines.Add(new IncludedContractSlotChargeLine(
            billingSlot.Id,
            swapDate,
            ContractSlotChargeKind.ExtraTrip,
            extraTrips * rate));
        totalExtraTripsCount += extraTrips;
    }

    private static void AppendSwapExtraWorkingDayLine(
        List<IncludedContractSlotChargeLine> lines,
        ContractSlot billingSlot,
        DateOnly swapDate,
        ref int totalExtraWorkingDaysCount)
    {
        var extraFee = billingSlot.ExtraWorkingDayFees ?? 0m;
        if (extraFee <= 0m)
            return;

        lines.Add(new IncludedContractSlotChargeLine(
            billingSlot.Id,
            swapDate,
            ContractSlotChargeKind.ExtraWorkingDay,
            extraFee));
        totalExtraWorkingDaysCount++;
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
