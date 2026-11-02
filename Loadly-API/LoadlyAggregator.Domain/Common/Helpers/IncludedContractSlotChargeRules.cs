using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public readonly record struct IncludedContractSlotChargeLine(
    long ContractSlotId,
    DateOnly? Date,
    ContractSlotChargeKind Kind,
    decimal Amount);

public readonly record struct IncludedContractSlotKmSummary(
    long ContractSlotId,
    decimal Consumption,
    decimal OverageKm,
    decimal OverConsumptionRate,
    decimal SuggestedOverageAmount);

public readonly record struct IncludedContractSlotCalendarDay(
    DateOnly Date,
    ContractSlotDayDisposition Disposition,
    int TripCount,
    int? WorkingDayIndex,
    bool IsExtra,
    VehicleFacilityType? FacilityType,
    long? FacilityId,
    string? FacilityName,
    IReadOnlyList<long> TripIds);

public readonly record struct IncludedContractKmPoolSummary(
    decimal TotalConsumption,
    decimal TotalMaxKms,
    decimal OverageKm,
    decimal OverConsumptionRate,
    decimal SuggestedOverageAmount);

public static class IncludedContractSlotChargeRules
{
    public static IReadOnlyList<IncludedContractSlotChargeLine> BuildForMonth(
        ContractSlot slot,
        DateOnly monthStart,
        DateOnly monthEnd,
        DateOnly billingStart,
        DateOnly billingEnd,
        decimal monthlyWorkingDayAllowance,
        IReadOnlyList<ContractSlotDayEvaluation> monthEvaluations,
        IReadOnlyList<Trip> slotTrips,
        ContractType contractType,
        ContractWorkingDayWindow workingDayWindow,
        bool includeKmOverageLine = true,
        decimal? monthlyMaxKmsAllowance = null,
        Func<DateOnly, ContractSlotBillingRates?>? resolveRatesForDate = null)
    {
        var lines = new List<IncludedContractSlotChargeLine>();

        foreach (var evaluation in monthEvaluations)
        {
            if (evaluation.Date < billingStart || evaluation.Date > billingEnd)
                continue;

            if (TryBuildDayLine(
                    slot,
                    evaluation.Date,
                    evaluation.Disposition,
                    evaluation.WorkingDayIndex,
                    monthlyWorkingDayAllowance,
                    contractType,
                    out var line,
                    resolveRatesForDate?.Invoke(evaluation.Date)))
                lines.Add(line);
        }

        if (includeKmOverageLine
            && TryBuildKmOverageLine(
                slot,
                monthStart,
                monthEnd,
                billingStart,
                billingEnd,
                slotTrips,
                monthlyMaxKmsAllowance ?? slot.MaxKms ?? 0m,
                workingDayWindow,
                out var kmLine))
        {
            lines.Add(kmLine);
        }

        return lines;
    }

    public static (IReadOnlyList<IncludedContractSlotChargeLine> Lines, int ExtraTripCount) BuildExtraTripChargesForMonth(
        ContractSlot slot,
        DateOnly billingStart,
        DateOnly billingEnd,
        IReadOnlyList<ContractSlotDayEvaluation> monthEvaluations,
        IReadOnlyList<Trip> slotTrips,
        Func<DateOnly, int?>? resolveTripCountForDate = null,
        Func<DateOnly, ContractSlotBillingRates?>? resolveRatesForDate = null)
    {
        if (slot.TripsPerDay is not int tripsPerDay || tripsPerDay <= 0)
            return ([], 0);

        var lines = new List<IncludedContractSlotChargeLine>();
        var extraTripCount = 0;

        foreach (var evaluation in monthEvaluations)
        {
            if (evaluation.Date < billingStart || evaluation.Date > billingEnd)
                continue;

            if (evaluation.Disposition != ContractSlotDayDisposition.Working)
                continue;

            var tripCount = resolveTripCountForDate?.Invoke(evaluation.Date)
                ?? ContractSlotDayClassifier.CountCompletedTripsOnDate(slotTrips, evaluation.Date);
            if (tripCount <= tripsPerDay)
                continue;

            decimal rate = resolveRatesForDate?.Invoke(evaluation.Date)?.OverConsumption
                       ?? slot.OverConsumption
                       ?? 0m;
            if (rate <= 0m)
                continue;

            var dayExtraTrips = tripCount - tripsPerDay;
            extraTripCount += dayExtraTrips;

            var amount = dayExtraTrips * rate;
            if (amount > 0m)
                lines.Add(new IncludedContractSlotChargeLine(
                    slot.Id,
                    evaluation.Date,
                    ContractSlotChargeKind.ExtraTrip,
                    amount));
        }

        return (lines, extraTripCount);
    }

    public static bool TryBuildSeatKmOverageLine(
        ContractSlot slot,
        DateOnly monthStart,
        DateOnly monthEnd,
        DateOnly billingStart,
        DateOnly billingEnd,
        IReadOnlyList<Trip> seatTrips,
        decimal monthlyMaxKmsAllowance,
        ContractWorkingDayWindow workingDayWindow,
        out IncludedContractSlotChargeLine line) =>
        TryBuildKmOverageLine(
            slot,
            monthStart,
            monthEnd,
            billingStart,
            billingEnd,
            seatTrips,
            monthlyMaxKmsAllowance,
            workingDayWindow,
            out line);

    public static bool TryBuildDayLine(
        ContractSlot slot,
        DateOnly date,
        ContractSlotDayDisposition disposition,
        int? workingDayIndex,
        decimal monthlyWorkingDayAllowance,
        ContractType contractType,
        out IncludedContractSlotChargeLine line,
        ContractSlotBillingRates? rateOverride = null)
    {
        line = default;

        if (!IncludedContractHalfWorkingDayRules.ParticipatesInWorkingDayIndex(disposition)
            || workingDayIndex is not > 0)
            return false;

        if (IncludedContractHalfWorkingDayRules.IsHalfWorking(disposition)
            && contractType != ContractType.Included)
            return false;

        if (workingDayIndex <= monthlyWorkingDayAllowance)
        {
            var dailyValue = rateOverride?.ResolveDailyValue(slot) ?? slot.DailyValue ?? 0m;
            if (dailyValue <= 0m)
                return false;

            var amount = IncludedContractHalfWorkingDayRules.ApplyAmountFactor(dailyValue, disposition);
            if (amount <= 0m)
                return false;

            line = new IncludedContractSlotChargeLine(slot.Id, date, ContractSlotChargeKind.BaseWorkingDay, amount);
            return true;
        }

        decimal extraFee = rateOverride?.ResolveExtraWorkingDayAmount(slot)
                       ?? ResolveExtraWorkingDayAmount(slot);
        if (extraFee <= 0m)
            return false;

        decimal extraAmount = IncludedContractHalfWorkingDayRules.ApplyAmountFactor(extraFee, disposition);
        if (extraAmount <= 0m)
            return false;

        line = new IncludedContractSlotChargeLine(slot.Id, date, ContractSlotChargeKind.ExtraWorkingDay, extraAmount);
        return true;
    }

    public static bool TryBuildSwapExtraWorkingDayLine(
        ContractSlot slot,
        DateOnly swapDate,
        ContractType contractType,
        out IncludedContractSlotChargeLine line,
        ContractSlotBillingRates? rateOverride = null)
    {
        line = default;
        decimal extraFee = rateOverride?.ResolveExtraWorkingDayAmount(slot)
                       ?? ResolveExtraWorkingDayAmount(slot);
        if (extraFee <= 0m)
            return false;

        line = new IncludedContractSlotChargeLine(slot.Id, swapDate, ContractSlotChargeKind.ExtraWorkingDay, extraFee);
        return true;
    }

    public static bool TryBuildSwapExtraTripLine(
        ContractSlot slot,
        DateOnly swapDate,
        IReadOnlyList<Trip> seatTrips,
        out IncludedContractSlotChargeLine line,
        out int extraTripCount,
        ContractSlotBillingRates? rateOverride = null,
        Func<DateOnly, int?>? resolveTripCountForDate = null)
    {
        line = default;
        extraTripCount = 0;
        if (slot.TripsPerDay is not int tripsPerDay || tripsPerDay <= 0)
            return false;

        decimal rate = rateOverride?.OverConsumption
                   ?? slot.OverConsumption ?? 0m;
        if (rate <= 0m)
            return false;

        var seatTripCount = resolveTripCountForDate?.Invoke(swapDate)
            ?? ContractSlotDayClassifier.CountCompletedTripsOnDate(seatTrips, swapDate);
        var extraTrips = seatTripCount - tripsPerDay;
        if (extraTrips <= 0)
            return false;

        extraTripCount = extraTrips;
        line = new IncludedContractSlotChargeLine(
            slot.Id,
            swapDate,
            ContractSlotChargeKind.ExtraTrip,
            extraTrips * rate);
        return true;
    }

    private static decimal ResolveExtraWorkingDayAmount(ContractSlot slot) =>
        slot.ExtraWorkingDayFees ?? 0m;

    private static bool TryBuildKmOverageLine(
        ContractSlot slot,
        DateOnly monthStart,
        DateOnly monthEnd,
        DateOnly billingStart,
        DateOnly billingEnd,
        IReadOnlyList<Trip> slotTrips,
        decimal monthlyMaxKmsAllowance,
        ContractWorkingDayWindow workingDayWindow,
        out IncludedContractSlotChargeLine line)
    {
        line = default;

        if (billingStart > monthEnd || billingEnd < monthStart)
            return false;

        var overConsumption = slot.OverConsumption ?? 0m;
        if (monthlyMaxKmsAllowance <= 0m || overConsumption <= 0m)
            return false;

        var totalKm = GetMonthlyCompletedKm(slotTrips, monthStart, monthEnd, workingDayWindow);
        if (totalKm <= monthlyMaxKmsAllowance)
            return false;

        var amount = (totalKm - monthlyMaxKmsAllowance) * overConsumption;
        if (amount <= 0m)
            return false;

        line = new IncludedContractSlotChargeLine(slot.Id, null, ContractSlotChargeKind.ExtraKmOverage, amount);
        return true;
    }

    public static void AccumulateDayCountsForBillingPeriod(
        IReadOnlyList<ContractSlotDayEvaluation> evaluations,
        DateOnly billingStart,
        DateOnly billingEnd,
        decimal monthlyWorkingDayAllowance,
        ref int workingDaysCount,
        ref int extraWorkingDaysCount)
    {
        foreach (var evaluation in evaluations)
        {
            if (evaluation.Date < billingStart || evaluation.Date > billingEnd)
                continue;

            if (!IncludedContractHalfWorkingDayRules.ParticipatesInWorkingDayIndex(evaluation.Disposition)
                || evaluation.WorkingDayIndex is not > 0)
                continue;

            if (evaluation.WorkingDayIndex > monthlyWorkingDayAllowance)
                extraWorkingDaysCount++;
            else
                workingDaysCount++;
        }
    }

    public static IReadOnlyList<IncludedContractSlotCalendarDay> BuildCalendarDaysForBillingPeriod(
        IReadOnlyList<ContractSlotDayEvaluation> evaluations,
        DateOnly billingStart,
        DateOnly billingEnd,
        decimal monthlyWorkingDayAllowance,
        IReadOnlyList<Trip> slotTrips,
        IReadOnlyList<Trip>? seatTrips = null,
        IReadOnlySet<DateOnly>? seatAggregateDates = null)
    {
        var days = new List<IncludedContractSlotCalendarDay>();

        foreach (var evaluation in evaluations)
        {
            if (evaluation.Date < billingStart || evaluation.Date > billingEnd)
                continue;

            var isExtra = IncludedContractHalfWorkingDayRules.ParticipatesInWorkingDayIndex(evaluation.Disposition)
                && evaluation.WorkingDayIndex > monthlyWorkingDayAllowance;

            IReadOnlyList<long> tripIds = ResolveCalendarTripIds(
                evaluation,
                slotTrips,
                seatTrips,
                seatAggregateDates);
            days.Add(new IncludedContractSlotCalendarDay(
                evaluation.Date,
                evaluation.Disposition,
                tripIds.Count,
                evaluation.WorkingDayIndex,
                isExtra,
                evaluation.FacilityType,
                evaluation.FacilityId,
                evaluation.FacilityName,
                tripIds));
        }

        return days;
    }

    private static IReadOnlyList<long> ResolveCalendarTripIds(
        ContractSlotDayEvaluation evaluation,
        IReadOnlyList<Trip> slotTrips,
        IReadOnlyList<Trip>? seatTrips,
        IReadOnlySet<DateOnly>? seatAggregateDates)
    {
        if (evaluation.Disposition == ContractSlotDayDisposition.NotActive)
            return [];

        IReadOnlyList<Trip> trips = seatAggregateDates?.Contains(evaluation.Date) == true && seatTrips is not null
            ? seatTrips
            : slotTrips;
        return ContractSlotDayClassifier.ListCompletedTripIdsOnDate(trips, evaluation.Date);
    }

    public static IncludedContractSlotKmSummary BuildKmSummaryForPeriod(
        ContractSlot slot,
        DateOnly periodStart,
        DateOnly periodEnd,
        ContractWorkingDayWindow workingDayWindow,
        IReadOnlyList<Trip> slotTrips,
        IReadOnlyList<ContractSlot>? seatSlots = null,
        IReadOnlyList<Trip>? seatTripsForOverage = null)
    {
        var consumption = slotTrips
            .Where(t => t.Status == TripStatus.Completed
                        && t.FinancialDate >= periodStart
                        && t.FinancialDate <= periodEnd)
            .Sum(t => t.ActualDistanceKm ?? 0m);

        var maxKms = slot.MaxKms ?? 0m;
        var overConsumptionRate = slot.OverConsumption ?? 0m;
        decimal overageKm = 0m;
        decimal suggestedOverageAmount = 0m;
        var seatContext = seatSlots ?? [slot];
        var overageTrips = seatTripsForOverage ?? slotTrips;
        ContractSlot? lastTermsSlot = null;

        if (maxKms > 0m || seatContext.Count > 1)
        {
            foreach (var (monthStart, monthEnd) in EnumerateCalendarMonths(periodStart, periodEnd))
            {
                var billingStart = monthStart > periodStart ? monthStart : periodStart;
                var billingEnd = monthEnd < periodEnd ? monthEnd : periodEnd;
                if (billingStart > monthEnd || billingEnd < monthStart)
                    continue;

                var termsSlot = IncludedContractSlotSeatHelper.UsesSharedMonthlyAllowance(seatContext, monthStart, monthEnd)
                    ? IncludedContractSlotSeatHelper.ResolveSeatMonthTermsSlot(seatContext, monthStart, monthEnd)
                    : slot;
                lastTermsSlot = termsSlot;

                var monthMaxKms = termsSlot.MaxKms ?? 0m;
                var monthOverConsumptionRate = termsSlot.OverConsumption ?? 0m;
                if (monthMaxKms <= 0m || monthOverConsumptionRate <= 0m)
                    continue;

                var monthlyMaxKmsAllowance = IncludedContractSlotSeatHelper.ResolveMonthlyMaxKmsAllowance(
                    monthMaxKms,
                    seatContext,
                    monthStart,
                    monthEnd);
                var monthTrips = IncludedContractSlotSeatHelper.UsesSharedMonthlyAllowance(seatContext, monthStart, monthEnd)
                    ? overageTrips
                    : slotTrips;

                var monthExtraKm = GetExtraKmOverageDistance(
                    monthTrips,
                    monthStart,
                    monthEnd,
                    monthlyMaxKmsAllowance,
                    workingDayWindow);
                if (monthExtraKm <= 0m)
                    continue;

                overageKm += monthExtraKm;
                suggestedOverageAmount += monthExtraKm * monthOverConsumptionRate;
            }
        }

        if (lastTermsSlot is not null)
            overConsumptionRate = lastTermsSlot.OverConsumption ?? overConsumptionRate;

        return new IncludedContractSlotKmSummary(
            slot.Id,
            consumption,
            overageKm,
            overConsumptionRate,
            suggestedOverageAmount);
    }

    public static decimal GetExtraKmOverageDistance(
        IReadOnlyList<Trip> slotTrips,
        DateOnly monthStart,
        DateOnly monthEnd,
        decimal monthlyMaxKmsAllowance,
        ContractWorkingDayWindow workingDayWindow)
    {
        if (monthlyMaxKmsAllowance <= 0m)
            return 0m;

        var totalKm = GetMonthlyCompletedKm(slotTrips, monthStart, monthEnd, workingDayWindow);
        return totalKm > monthlyMaxKmsAllowance ? totalKm - monthlyMaxKmsAllowance : 0m;
    }

    public static decimal GetMonthlyCompletedKm(
        IReadOnlyList<Trip> slotTrips,
        DateOnly monthStart,
        DateOnly monthEnd,
        ContractWorkingDayWindow workingDayWindow)
    {
        return slotTrips
            .Where(t => t.Status == TripStatus.Completed
                        && t.FinancialDate >= monthStart
                        && t.FinancialDate <= monthEnd)
            .Sum(t => t.ActualDistanceKm ?? 0m);
    }

    /// <summary>
    /// Contracted monthly pool cap: each vehicle type contributes <c>quantity × max kms</c>.
    /// Slot chains are not counted, so removing a seat does not shrink the allowance.
    /// </summary>
    public static decimal ResolveContractedPoolMonthlyMaxKms(IEnumerable<ContractVehicleType> vehicleTypes) =>
        vehicleTypes
            .Where(type => type.DeletedAt == null)
            .Sum(type => (type.MaxKms ?? 0m) * Math.Max(type.Quantity, 0));

    public static (IReadOnlyList<IncludedContractSlotChargeLine> Lines, IncludedContractKmPoolSummary PoolSummary)
        BuildPooledKmOverageForPeriod(
            IReadOnlyList<IncludedContractSlotSeat> seats,
            IReadOnlyDictionary<long, IReadOnlyList<Trip>> tripsBySlotId,
            DateOnly periodStart,
            DateOnly periodEnd,
            ContractWorkingDayWindow workingDayWindow,
            decimal contractedMonthlyMaxKms)
    {
        var lines = new List<IncludedContractSlotChargeLine>();
        var overConsumptionRate = seats
            .SelectMany(seat => seat.SlotsInOrder)
            .Select(slot => slot.OverConsumption ?? 0m)
            .FirstOrDefault(rate => rate > 0m);

        var allTrips = tripsBySlotId.Values.SelectMany(trips => trips).DistinctBy(trip => trip.Id).ToList();
        var totalConsumption = allTrips
            .Where(t => t.Status == TripStatus.Completed
                        && t.FinancialDate >= periodStart
                        && t.FinancialDate <= periodEnd)
            .Sum(t => t.ActualDistanceKm ?? 0m);

        decimal totalMaxKmsAllowance = 0m;
        decimal totalOverageKm = 0m;
        decimal totalSuggestedAmount = 0m;

        foreach (var (monthStart, monthEnd) in EnumerateCalendarMonths(periodStart, periodEnd))
        {
            var billingStart = monthStart > periodStart ? monthStart : periodStart;
            var billingEnd = monthEnd < periodEnd ? monthEnd : periodEnd;
            if (billingStart > monthEnd || billingEnd < monthStart)
                continue;

            decimal monthMaxKms = contractedMonthlyMaxKms;
            decimal monthKm = 0m;
            foreach (var seat in seats)
            {
                monthKm += seat.SlotsInOrder.Sum(slot =>
                {
                    if (!tripsBySlotId.TryGetValue(slot.Id, out var trips))
                        return 0m;

                    return GetMonthlyCompletedKm(trips, monthStart, monthEnd, workingDayWindow);
                });
            }

            totalMaxKmsAllowance += monthMaxKms;
            if (monthMaxKms <= 0m || overConsumptionRate <= 0m || monthKm <= monthMaxKms)
                continue;

            var monthOverageKm = monthKm - monthMaxKms;
            var monthAmount = monthOverageKm * overConsumptionRate;
            if (monthAmount <= 0m)
                continue;

            totalOverageKm += monthOverageKm;
            totalSuggestedAmount += monthAmount;
            lines.Add(new IncludedContractSlotChargeLine(0, null, ContractSlotChargeKind.ExtraKmOverage, monthAmount));
        }

        return (lines, new IncludedContractKmPoolSummary(
            totalConsumption,
            totalMaxKmsAllowance,
            totalOverageKm,
            overConsumptionRate,
            totalSuggestedAmount));
    }

    public static int GetMaxWorkingDayIndex(IReadOnlyList<ContractSlotDayEvaluation> evaluations) =>
        evaluations
            .Where(e => e.WorkingDayIndex is > 0)
            .Select(e => e.WorkingDayIndex!.Value)
            .DefaultIfEmpty(0)
            .Max();

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
