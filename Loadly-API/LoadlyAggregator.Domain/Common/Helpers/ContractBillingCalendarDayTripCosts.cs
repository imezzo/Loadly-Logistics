using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Common.Helpers;

/// <summary>
/// Builds each calendar day's trip rows for the response and the totals counted from those rows.
/// Trips are ordered by start time. When the slot's vehicle type has no <see cref="ContractVehicleType.TripsPerDay"/>, the first trip is main (day fee) and the rest are extra.
/// When <see cref="ContractVehicleType.TripsPerDay"/> is set (via the slot's <see cref="ContractSlot.VehicleTypeId"/>), that many trips are main (day fee on the first only); further trips are extra and share the day's extra-trip line.
/// </summary>
public static class ContractBillingCalendarDayTripCosts
{
    public static async Task ApplyAsync(
        CalculateIncludedContractSlotChargesResultDto calculation,
        Contract contract,
        ITripRepository tripRepository)
    {
        await ApplyFromCalendarBillingFactsAsync([calculation], [(calculation, contract)], tripRepository);
    }

    public static async Task ApplyAsync(
        IReadOnlyList<(CalculateIncludedContractSlotChargesResultDto Calculation, Contract Contract)> items,
        ITripRepository tripRepository)
    {
        if (items.Count == 0)
            return;

        await ApplyFromCalendarBillingFactsAsync(
            items.Select(item => item.Calculation),
            items,
            tripRepository);
    }

    public static async Task ApplyAsync(
        InvoiceSlotChargesDto slotCharges,
        Contract contract,
        ITripRepository tripRepository)
    {
        var tripIds = slotCharges.Calendar
            .SelectMany(group => group.Days)
            .SelectMany(day => day.TripIds)
            .Distinct()
            .ToArray();
        var startDates = await tripRepository.GetStartDatesByIdsAsync(tripIds);
        var tripsPerDayBySlotId = TripsPerDayBySlotId(contract);
        var amountsByDay = IndexAmounts(slotCharges.Slots
            .Where(slotGroup => slotGroup.ContractSlotId is > 0)
            .SelectMany(slotGroup => slotGroup.Lines
                .Where(line => line.Date.HasValue)
                .Select(line => (
                    slotGroup.ContractSlotId!.Value,
                    line.Date!.Value,
                    line.ChargeKind,
                    line.Amount))));

        foreach (var group in slotCharges.Calendar)
        {
            tripsPerDayBySlotId.TryGetValue(group.ContractSlotId, out var tripsPerDay);
            foreach (var day in group.Days)
            {
                day.Trips = Build(
                    day.IsExtra,
                    day.TripIds,
                    tripsPerDay,
                    AmountsFor(amountsByDay, group.ContractSlotId, day.Date),
                    startDates);
            }
        }

        await StampOvernightAsync(
            slotCharges.Calendar.SelectMany(group => group.Days).SelectMany(day => day.Trips),
            tripRepository);
    }

    public static void Apply(
        CalculateIncludedContractSlotChargesResultDto calculation,
        IEnumerable<ContractSlot> slots,
        IEnumerable<ContractVehicleType> contractVehicleTypes,
        IReadOnlyDictionary<long, DateTime?> startDateByTripId) =>
        Apply(calculation, TripsPerDayBySlotId(slots, contractVehicleTypes), startDateByTripId);

    public static void Apply(
        CalculateIncludedContractSlotChargesResultDto calculation,
        IReadOnlyDictionary<long, int?> tripsPerDayBySlotId,
        IReadOnlyDictionary<long, DateTime?> startDateByTripId)
    {
        var amountsByDay = IndexAmounts(calculation.Lines
            .Where(line => line.Date.HasValue && line.ContractSlotId > 0)
            .Select(line => (
                line.ContractSlotId,
                line.Date!.Value,
                (ContractSlotChargeKind)line.ChargeKind,
                line.Amount)));

        foreach (var slot in calculation.Calendar)
        {
            tripsPerDayBySlotId.TryGetValue(slot.SlotId, out var tripsPerDay);
            foreach (var day in slot.Days)
            {
                day.Trips = Build(
                    day.IsExtra,
                    day.TripIds,
                    tripsPerDay,
                    AmountsFor(amountsByDay, slot.SlotId, day.Date),
                    startDateByTripId);
            }
        }
    }

    private static async Task ApplyFromCalendarBillingFactsAsync(
        IEnumerable<CalculateIncludedContractSlotChargesResultDto> calculations,
        IReadOnlyList<(CalculateIncludedContractSlotChargesResultDto Calculation, Contract Contract)> items,
        ITripRepository tripRepository)
    {
        var calculationList = calculations.ToList();
        var tripIds = calculationList
            .SelectMany(calculation => calculation.Calendar)
            .SelectMany(slot => slot.Days)
            .SelectMany(day => day.TripIds)
            .Distinct()
            .ToArray();
        var facts = await tripRepository.GetCalendarBillingFactsByIdsAsync(tripIds);
        var startDates = facts.ToDictionary(pair => pair.Key, pair => pair.Value.StartDate);

        foreach (var (calculation, contract) in items)
        {
            Apply(calculation, TripsPerDayBySlotId(contract), startDates);
            ApplyOvernightTotals(calculation, facts);
            ApplyFuelTotals(calculation, contract, facts);
        }
    }

    private static void ApplyOvernightTotals(
        CalculateIncludedContractSlotChargesResultDto calculation,
        IReadOnlyDictionary<long, (DateTime? StartDate, bool HasOvernight, decimal OvernightAmount, decimal OvernightCompanyShare, bool HasFuel, decimal FuelAmount, decimal FuelDistanceKm)> facts)
    {
        foreach (var trip in CalendarTrips(calculation))
        {
            if (!facts.TryGetValue(trip.TripId, out var fact) || !fact.HasOvernight)
                continue;

            trip.HasOvernight = true;
            trip.OvernightAmount = fact.OvernightAmount - fact.OvernightCompanyShare;
        }

        var overnightTripIds = CalendarTrips(calculation)
            .Where(trip => trip.HasOvernight)
            .Select(trip => trip.TripId)
            .Distinct()
            .ToList();
        calculation.TotalOvernightTripsCount = overnightTripIds.Count;
        calculation.TotalOvernightCustomerAmount = overnightTripIds.Sum(id =>
            facts[id].OvernightAmount - facts[id].OvernightCompanyShare);
        calculation.TotalOvernightLoadlyAmount = overnightTripIds.Sum(id => facts[id].OvernightCompanyShare);
    }

    private static void ApplyFuelTotals(
        CalculateIncludedContractSlotChargesResultDto calculation,
        Contract contract,
        IReadOnlyDictionary<long, (DateTime? StartDate, bool HasOvernight, decimal OvernightAmount, decimal OvernightCompanyShare, bool HasFuel, decimal FuelAmount, decimal FuelDistanceKm)> facts)
    {
        if (contract.ContractType != ContractType.Excluded
            || contract.ConsumptionType != ContractConsumptionTypes.Trips)
        {
            calculation.TotalFuelAmount = 0;
            calculation.TotalFuelKms = 0;
            return;
        }

        var chargedTripIds = CalendarTripIds(calculation)
            .Where(id => facts.TryGetValue(id, out var fact) && fact.HasFuel)
            .Distinct()
            .ToList();
        calculation.TotalFuelAmount = chargedTripIds.Sum(id => facts[id].FuelAmount);
        calculation.TotalFuelKms = chargedTripIds.Sum(id => facts[id].FuelDistanceKm);
    }

    public static Task ApplyTripTotalsAsync(
        CalculateIncludedContractSlotChargesResultDto calculation,
        ITripRepository tripRepository) =>
        ApplyTripTotalsAsync([calculation], tripRepository);

    private static async Task ApplyTripTotalsAsync(
        IEnumerable<CalculateIncludedContractSlotChargesResultDto> calculations,
        ITripRepository tripRepository)
    {
        var items = calculations.ToList();
        if (items.Count == 0)
            return;

        var trips = items.SelectMany(CalendarTrips).ToList();
        var charges = await LoadOvernightChargesAsync(trips, tripRepository);
        foreach (var calculation in items)
        {
            StampOvernight(CalendarTrips(calculation), charges);
            var overnightTripIds = CalendarTrips(calculation)
                .Where(trip => trip.HasOvernight)
                .Select(trip => trip.TripId)
                .Distinct()
                .ToList();
            calculation.TotalOvernightTripsCount = overnightTripIds.Count;
            calculation.TotalOvernightCustomerAmount = overnightTripIds.Sum(id => charges[id].Amount - charges[id].CompanyShare);
            calculation.TotalOvernightLoadlyAmount = overnightTripIds.Sum(id => charges[id].CompanyShare);
        }
    }

    public static async Task ApplyConsumptionTotalsAsync(
        IReadOnlyList<(CalculateIncludedContractSlotChargesResultDto Calculation, Contract Contract)> items,
        ITripRepository tripRepository)
    {
        if (items.Count == 0)
            return;

        var fuelItems = items
            .Where(item => item.Contract.ContractType == ContractType.Excluded
                && item.Contract.ConsumptionType == ContractConsumptionTypes.Trips)
            .Select(item => item.Calculation)
            .ToList();
        foreach (var (calculation, contract) in items)
        {
            if (contract.ContractType == ContractType.Excluded
                && contract.ConsumptionType == ContractConsumptionTypes.Trips)
                continue;

            calculation.TotalFuelAmount = 0;
            calculation.TotalFuelKms = 0;
        }

        if (fuelItems.Count == 0)
            return;

        var tripIds = fuelItems.SelectMany(CalendarTripIds).Distinct().ToArray();
        var fuelCharges = await tripRepository.GetFuelChargesByIdsAsync(tripIds);
        foreach (var calculation in fuelItems)
        {
            var chargedTripIds = CalendarTripIds(calculation)
                .Where(fuelCharges.ContainsKey)
                .Distinct()
                .ToList();
            calculation.TotalFuelAmount = chargedTripIds.Sum(id => fuelCharges[id].Amount);
            calculation.TotalFuelKms = chargedTripIds.Sum(id => fuelCharges[id].DistanceKm);
        }
    }

    private static IEnumerable<long> CalendarTripIds(CalculateIncludedContractSlotChargesResultDto calculation) =>
        calculation.Calendar
            .SelectMany(slot => slot.Days)
            .SelectMany(day => day.TripIds);

    private static async Task StampOvernightAsync(
        IEnumerable<IncludedContractSlotCalendarDayTripDto> trips,
        ITripRepository tripRepository)
    {
        var tripList = trips.ToList();
        StampOvernight(tripList, await LoadOvernightChargesAsync(tripList, tripRepository));
    }

    private static async Task<IReadOnlyDictionary<long, (decimal Amount, decimal CompanyShare)>> LoadOvernightChargesAsync(
        IReadOnlyList<IncludedContractSlotCalendarDayTripDto> trips,
        ITripRepository tripRepository) =>
        trips.Count == 0
            ? new Dictionary<long, (decimal Amount, decimal CompanyShare)>()
            : await tripRepository.GetOvernightChargesByIdsAsync(trips.Select(trip => trip.TripId).Distinct().ToArray());

    private static void StampOvernight(
        IEnumerable<IncludedContractSlotCalendarDayTripDto> trips,
        IReadOnlyDictionary<long, (decimal Amount, decimal CompanyShare)> charges)
    {
        foreach (var trip in trips)
        {
            if (!charges.TryGetValue(trip.TripId, out var charge))
                continue;

            trip.HasOvernight = true;
            trip.OvernightAmount = charge.Amount - charge.CompanyShare;
        }
    }

    private static IEnumerable<IncludedContractSlotCalendarDayTripDto> CalendarTrips(
        CalculateIncludedContractSlotChargesResultDto calculation) =>
        calculation.Calendar
            .SelectMany(slot => slot.Days)
            .SelectMany(day => day.Trips);

    private static async Task<IReadOnlyDictionary<long, DateTime?>> LoadStartDatesAsync(
        IEnumerable<CalculateIncludedContractSlotChargesResultDto> calculations,
        ITripRepository tripRepository)
    {
        var tripIds = calculations
            .SelectMany(calculation => calculation.Calendar)
            .SelectMany(slot => slot.Days)
            .SelectMany(day => day.TripIds)
            .Distinct()
            .ToArray();
        return await tripRepository.GetStartDatesByIdsAsync(tripIds);
    }

    private static Dictionary<long, int?> TripsPerDayBySlotId(Contract contract) =>
        TripsPerDayBySlotId(contract.ContractSlots, contract.ContractVehicleTypes);

    private static Dictionary<long, int?> TripsPerDayBySlotId(
        IEnumerable<ContractSlot> slots,
        IEnumerable<ContractVehicleType> contractVehicleTypes)
    {
        var tripsPerDayByVehicleTypeId = contractVehicleTypes
            .GroupBy(row => row.VehicleTypeId)
            .ToDictionary(group => group.Key, group => group.First().TripsPerDay);

        return slots
            .GroupBy(slot => slot.Id)
            .ToDictionary(
                group => group.Key,
                group =>
                {
                    var slot = group.First();
                    return tripsPerDayByVehicleTypeId.TryGetValue(slot.VehicleTypeId, out var tripsPerDay)
                        ? tripsPerDay
                        : null;
                });
    }

    private static Dictionary<(long SlotId, DateOnly Date), (decimal BaseWorkingDay, decimal ExtraWorkingDay, decimal ExtraTrip)> IndexAmounts(
        IEnumerable<(long SlotId, DateOnly Date, ContractSlotChargeKind Kind, decimal Amount)> lines) =>
        lines
            .GroupBy(line => (line.SlotId, line.Date))
            .ToDictionary(
                group => group.Key,
                group => (
                    group.Where(line => line.Kind == ContractSlotChargeKind.BaseWorkingDay).Sum(line => line.Amount),
                    group.Where(line => line.Kind == ContractSlotChargeKind.ExtraWorkingDay).Sum(line => line.Amount),
                    group.Where(line => line.Kind == ContractSlotChargeKind.ExtraTrip).Sum(line => line.Amount)));

    private static (decimal BaseWorkingDay, decimal ExtraWorkingDay, decimal ExtraTrip) AmountsFor(
        IReadOnlyDictionary<(long SlotId, DateOnly Date), (decimal BaseWorkingDay, decimal ExtraWorkingDay, decimal ExtraTrip)> amountsByDay,
        long slotId,
        DateOnly date) =>
        amountsByDay.TryGetValue((slotId, date), out var amounts)
            ? amounts
            : (0, 0, 0);

    private static IReadOnlyList<IncludedContractSlotCalendarDayTripDto> Build(
        bool isExtra,
        IReadOnlyList<long> tripIds,
        int? tripsPerDay,
        (decimal BaseWorkingDay, decimal ExtraWorkingDay, decimal ExtraTrip) amounts,
        IReadOnlyDictionary<long, DateTime?> startDateByTripId)
    {
        if (tripIds.Count == 0)
            return [];

        var ordered = tripIds
            .Distinct()
            .OrderBy(id => StartDateOrLast(id, startDateByTripId))
            .ThenBy(id => id)
            .ToList();

        int includedCount = tripsPerDay is > 0
            ? Math.Min(tripsPerDay.Value, ordered.Count)
            : ordered.Count;
        int extraCount = ordered.Count - includedCount;
        var (mainKind, mainAmount) = ResolveMainCharge(isExtra, amounts.BaseWorkingDay, amounts.ExtraWorkingDay);
        if (extraCount == 0 && amounts.ExtraTrip != 0)
            mainAmount += amounts.ExtraTrip;

        var trips = new List<IncludedContractSlotCalendarDayTripDto>(ordered.Count);
        decimal allocatedExtra = 0;
        for (int index = 0; index < ordered.Count; index++)
        {
            long tripId = ordered[index];
            startDateByTripId.TryGetValue(tripId, out var startDate);
            if (IsMainTripRole(index, tripsPerDay, includedCount))
            {
                decimal dayAmount = index == 0 ? mainAmount : 0;
                trips.Add(Create(tripId, startDate, ContractBillingCalendarTripRole.Main, mainKind, dayAmount));
                continue;
            }

            decimal amount = 0;
            if (index >= includedCount)
            {
                int extraIndex = index - includedCount;
                bool isLastExtra = extraIndex == extraCount - 1;
                amount = isLastExtra
                    ? amounts.ExtraTrip - allocatedExtra
                    : RoundMoney(amounts.ExtraTrip / extraCount);
                allocatedExtra += amount;
            }

            trips.Add(Create(
                tripId,
                startDate,
                ContractBillingCalendarTripRole.Extra,
                ContractSlotChargeKind.ExtraTrip,
                amount));
        }

        return trips;
    }

    private static bool IsMainTripRole(int index, int? tripsPerDay, int includedCount) =>
        tripsPerDay is > 0
            ? index < includedCount
            : index == 0;

    private static (ContractSlotChargeKind Kind, decimal Amount) ResolveMainCharge(
        bool isExtra,
        decimal baseWorkingDayAmount,
        decimal extraWorkingDayAmount)
    {
        if (baseWorkingDayAmount > 0 && extraWorkingDayAmount <= 0)
            return (ContractSlotChargeKind.BaseWorkingDay, baseWorkingDayAmount);

        if (extraWorkingDayAmount > 0 && baseWorkingDayAmount <= 0)
            return (ContractSlotChargeKind.ExtraWorkingDay, extraWorkingDayAmount);

        decimal combined = baseWorkingDayAmount + extraWorkingDayAmount;
        return isExtra
            ? (ContractSlotChargeKind.ExtraWorkingDay, combined)
            : (ContractSlotChargeKind.BaseWorkingDay, combined);
    }

    private static IncludedContractSlotCalendarDayTripDto Create(
        long tripId,
        DateTime? startDate,
        ContractBillingCalendarTripRole role,
        ContractSlotChargeKind chargeKind,
        decimal amount) =>
        new()
        {
            TripId = tripId,
            StartDate = startDate,
            Role = role,
            ChargeKind = chargeKind,
            Amount = amount
        };

    private static DateTime StartDateOrLast(long tripId, IReadOnlyDictionary<long, DateTime?> startDateByTripId) =>
        startDateByTripId.TryGetValue(tripId, out var startDate) && startDate.HasValue
            ? startDate.Value
            : DateTime.MaxValue;

    private static decimal RoundMoney(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
