using System.Text.Json;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractBillingSnapshotReader
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static List<long> CollectDistinctTripIds(ContractBillingSnapshot snapshot) =>
        snapshot.CalendarDays
            .Where(day => day.DeletedAt == null)
            .SelectMany(day => day.TripIds)
            .Distinct()
            .OrderBy(tripId => tripId)
            .ToList();

    public static CalculateIncludedContractSlotChargesResultDto ToCalculationDto(ContractBillingSnapshot snapshot)
    {
        var calendar = snapshot.CalendarDays
            .Where(day => day.DeletedAt == null)
            .GroupBy(day => day.ContractSlotId)
            .Select(group => new IncludedContractSlotCalendarDto
            {
                SlotId = group.Key,
                Days = group
                    .OrderBy(day => day.Date)
                    .Select(day => new IncludedContractSlotCalendarDayDto
                    {
                        CalendarDayId = day.Id,
                        Date = day.Date,
                        Disposition = day.Disposition,
                        DispositionSource = day.DispositionSource,
                        OverrideReason = day.OverrideReason,
                        TripIds = day.TripIds,
                        TripCount = day.TripIds.Length,
                        TripCountSource = day.TripCountSource,
                        WorkingDayIndex = day.WorkingDayIndex,
                        IsExtra = day.IsExtra,
                        FacilityType = day.FacilityType,
                        FacilityId = day.FacilityId,
                        FacilityName = day.VehicleFacility?.Name,
                        ChangedBy = day.ChangedBy,
                        ChangedAt = day.ChangedAt
                    })
                    .ToList()
            })
            .ToList();

        var calendarDays = calendar.SelectMany(slot => slot.Days).ToList();

        var result = new CalculateIncludedContractSlotChargesResultDto
        {
            Lines = snapshot.ChargeLines
                .Where(line => line.DeletedAt == null)
                .Select(line => new IncludedContractSlotChargeLineDto
                {
                    ContractSlotId = line.ContractSlotId ?? 0,
                    Date = line.Date,
                    ChargeKind = (int)line.ChargeKind,
                    Amount = line.Amount
                })
                .ToList(),
            Kms = snapshot.KmEvidence
                .Where(km => km.DeletedAt == null && !km.IsPooled)
                .Select(km => new IncludedContractSlotKmSummaryDto
                {
                    SlotId = km.ContractSlotId ?? 0,
                    Consumption = km.Consumption,
                    OverageKm = km.OverageKm,
                    OverConsumptionRate = km.OverConsumptionRate,
                    SuggestedOverageAmount = km.Amount
                })
                .ToList(),
            Calendar = calendar,
            KmPool = DeserializeKmPool(snapshot.KmPoolJson),
            TotalAmount = snapshot.SubTotal,
            TotalWorkingDaysAmount = snapshot.WorkingDaysAmount,
            TotalExtraWorkingDaysAmount = snapshot.ExtraWorkingDaysAmount,
            TotalExtraKmsAmount = snapshot.ExtraKmsAmount,
            TotalExtraTripsAmount = snapshot.ExtraTripsAmount,
            TotalTripsCount = CollectDistinctTripIds(snapshot).Count,
            TotalDaysWithTripsCount = calendarDays.Count(day => day.TripCount > 0),
            TotalWorkingDaysCount = snapshot.WorkingDaysCount,
            TotalExtraWorkingDaysCount = snapshot.ExtraWorkingDaysCount,
            TotalExtraTripsCount = snapshot.ExtraTripsCount,
            TotalExtraKmsCount = snapshot.ExtraKmsCount,
            CommercialTermOverrides = snapshot.CommercialOverrides
                .Where(row => row.DeletedAt == null)
                .Select(row => new ContractBillingSnapshotCommercialOverrideDto
                {
                    Id = row.Id,
                    SeatRootSlotId = row.SeatRootSlotId,
                    EffectiveStart = row.EffectiveStart,
                    EffectiveEnd = row.EffectiveEnd,
                    DailyValue = row.DailyValue,
                    ExtraWorkingDayFees = row.ExtraWorkingDayFees,
                    OverConsumption = row.OverConsumption,
                    OverrideReason = row.OverrideReason
                })
                .ToList()
        };
        ApplyWaitingForLoadingTotals(result);
        return result;
    }

    public static void ApplyWaitingForLoadingTotals(CalculateIncludedContractSlotChargesResultDto result)
    {
        var parkingDays = result.Calendar
            .SelectMany(slot => slot.Days.Select(day => (SlotId: slot.SlotId, Day: day)))
            .Where(entry => entry.Day.FacilityType == VehicleFacilityType.Parking)
            .ToList();
        var parkingDayKeys = parkingDays
            .Select(entry => (entry.SlotId, entry.Day.Date))
            .ToHashSet();

        result.TotalWaitingForLoadingDaysCount = parkingDays.Count;
        result.TotalWaitingForLoadingExtraDaysCount = parkingDays.Count(entry => entry.Day.IsExtra);
        result.TotalWaitingForLoadingWorkingDaysCount = parkingDays.Count(entry =>
            !entry.Day.IsExtra
            && IncludedContractHalfWorkingDayRules.ParticipatesInWorkingDayIndex(entry.Day.Disposition));
        result.TotalWaitingForLoadingAmount = result.Lines
            .Where(line => line.Date.HasValue
                && parkingDayKeys.Contains((line.ContractSlotId, line.Date.Value))
                && line.ChargeKind is (int)ContractSlotChargeKind.BaseWorkingDay
                    or (int)ContractSlotChargeKind.ExtraWorkingDay)
            .Sum(line => line.Amount);
    }

    public static IncludedContractKmPoolDto? DeserializeKmPool(string? json) =>
        string.IsNullOrWhiteSpace(json)
            ? null
            : JsonSerializer.Deserialize<IncludedContractKmPoolDto>(json, JsonOptions);

    public static void ApplyManualCalendarOverrides(
        CalculateIncludedContractSlotChargesResultDto calculated,
        IReadOnlyList<ContractBillingSnapshotCalendarDay> manualDays)
    {
        if (manualDays.Count == 0)
            return;

        var manualByKey = manualDays.ToDictionary(day => (day.ContractSlotId, day.Date));
        var calendars = calculated.Calendar.Select(slot => new IncludedContractSlotCalendarDto
        {
            SlotId = slot.SlotId,
            SeatRootSlotId = slot.SeatRootSlotId,
            Days = slot.Days
                .Select(day =>
                {
                    if (!manualByKey.TryGetValue((slot.SlotId, day.Date), out var manual))
                        return day;

                    var keepsFacility = manual.Disposition is ContractSlotDayDisposition.Skipped
                        or ContractSlotDayDisposition.Working
                        or ContractSlotDayDisposition.HalfWorking;
                    return new IncludedContractSlotCalendarDayDto
                    {
                        Date = day.Date,
                        Disposition = manual.Disposition,
                        TripIds = day.TripIds,
                        TripCount = day.TripIds.Count,
                        Trips = day.Trips,
                        WorkingDayIndex = manual.WorkingDayIndex,
                        IsExtra = manual.IsExtra,
                        FacilityType = keepsFacility ? manual.FacilityType : null,
                        FacilityId = keepsFacility ? manual.FacilityId : null,
                        FacilityName = keepsFacility ? manual.VehicleFacility?.Name : null,
                        ChangedBy = manual.ChangedBy,
                        ChangedAt = manual.ChangedAt
                    };
                })
                .ToList()
        }).ToList();

        calculated.Calendar = calendars;
    }
}
