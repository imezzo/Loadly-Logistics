using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.API.Resources.Admin.Contracts;

public class IncludedContractSlotChargeLineResource
{
    public long ContractSlotId { get; set; }
    public DateOnly? Date { get; set; }
    public int ChargeKind { get; set; }
    public string ChargeKindName { get; set; } = string.Empty;
    public decimal Amount { get; set; }

    public static IncludedContractSlotChargeLineResource FromDto(IncludedContractSlotChargeLineDto dto) =>
        new()
        {
            ContractSlotId = dto.ContractSlotId,
            Date = dto.Date,
            ChargeKind = dto.ChargeKind,
            ChargeKindName = ((ContractSlotChargeKind)dto.ChargeKind).ToLocalizedDisplay(),
            Amount = dto.Amount
        };
}

public class IncludedContractSlotKmSummaryResource
{
    public long SlotId { get; set; }
    public decimal Consumption { get; set; }
    public decimal OverageKm { get; set; }
    public decimal OverConsumptionRate { get; set; }
    public decimal SuggestedOverageAmount { get; set; }

    public static IncludedContractSlotKmSummaryResource FromDto(IncludedContractSlotKmSummaryDto dto) =>
        new()
        {
            SlotId = dto.SlotId,
            Consumption = dto.Consumption,
            OverageKm = dto.OverageKm,
            OverConsumptionRate = dto.OverConsumptionRate,
            SuggestedOverageAmount = dto.SuggestedOverageAmount
        };
}

public class IncludedContractSlotCalendarDayTripResource
{
    public string TripId { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public int Role { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public int ChargeKind { get; set; }
    public string ChargeKindName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool HasOvernight { get; set; }
    public decimal OvernightAmount { get; set; }

    public static IncludedContractSlotCalendarDayTripResource FromDto(IncludedContractSlotCalendarDayTripDto dto) =>
        new()
        {
            TripId = TripReferenceNumber.Format(dto.TripId),
            StartDate = dto.StartDate,
            Role = (int)dto.Role,
            RoleName = dto.Role.ToLocalizedDisplay(),
            ChargeKind = (int)dto.ChargeKind,
            ChargeKindName = dto.ChargeKind.ToLocalizedDisplay(),
            Amount = dto.Amount,
            HasOvernight = dto.HasOvernight,
            OvernightAmount = dto.OvernightAmount
        };
}

public class IncludedContractSlotCalendarDayResource
{
    public long? CalendarDayId { get; set; }
    public DateOnly Date { get; set; }
    public int Disposition { get; set; }
    public string DispositionName { get; set; } = string.Empty;
    public int? DispositionSource { get; set; }
    public string? OverrideReason { get; set; }
    public int TripCount { get; set; }
    public IReadOnlyList<string> TripIds { get; set; } = [];
    public IReadOnlyList<IncludedContractSlotCalendarDayTripResource> Trips { get; set; } = [];
    public int? TripCountSource { get; set; }
    public int? WorkingDayIndex { get; set; }
    public bool IsExtra { get; set; }
    public int? FacilityType { get; set; }
    public long? FacilityId { get; set; }
    public string? FacilityName { get; set; }
    public long? ChangedBy { get; set; }
    public DateTime? ChangedAt { get; set; }

    public static IncludedContractSlotCalendarDayResource FromDto(IncludedContractSlotCalendarDayDto dto) =>
        new()
        {
            CalendarDayId = dto.CalendarDayId,
            Date = dto.Date,
            Disposition = (int)dto.Disposition,
            DispositionName = dto.Disposition.ToLocalizedDisplay(),
            DispositionSource = dto.DispositionSource.HasValue ? (int)dto.DispositionSource.Value : null,
            OverrideReason = dto.OverrideReason,
            TripCount = dto.TripIds.Count,
            TripIds = dto.TripIds.Select(TripReferenceNumber.Format).ToList(),
            Trips = dto.Trips.Select(IncludedContractSlotCalendarDayTripResource.FromDto).ToList(),
            TripCountSource = dto.TripCountSource.HasValue ? (int)dto.TripCountSource.Value : null,
            WorkingDayIndex = dto.WorkingDayIndex,
            IsExtra = dto.IsExtra,
            FacilityType = dto.FacilityType.HasValue ? (int)dto.FacilityType.Value : null,
            FacilityId = dto.FacilityId,
            FacilityName = dto.FacilityName,
            ChangedBy = dto.ChangedBy,
            ChangedAt = dto.ChangedAt
        };
}

public class IncludedContractSlotCalendarResource
{
    public long SlotId { get; set; }
    public long SeatRootSlotId { get; set; }
    public IReadOnlyList<IncludedContractSlotCalendarDayResource> Days { get; set; } = [];

    public static IncludedContractSlotCalendarResource FromDto(IncludedContractSlotCalendarDto dto) =>
        new()
        {
            SlotId = dto.SlotId,
            SeatRootSlotId = dto.SeatRootSlotId,
            Days = dto.Days.Select(IncludedContractSlotCalendarDayResource.FromDto).ToList()
        };
}

public class IncludedContractKmPoolResource
{
    public decimal TotalConsumption { get; set; }
    public decimal TotalMaxKms { get; set; }
    public decimal OverageKm { get; set; }
    public decimal OverConsumptionRate { get; set; }
    public decimal SuggestedOverageAmount { get; set; }

    public static IncludedContractKmPoolResource? FromDto(IncludedContractKmPoolDto? dto) =>
        dto is null
            ? null
            : new IncludedContractKmPoolResource
            {
                TotalConsumption = dto.TotalConsumption,
                TotalMaxKms = dto.TotalMaxKms,
                OverageKm = dto.OverageKm,
                OverConsumptionRate = dto.OverConsumptionRate,
                SuggestedOverageAmount = dto.SuggestedOverageAmount
            };
}

public class ContractBillingSnapshotCommercialOverrideResource
{
    public long Id { get; set; }
    public long SeatRootSlotId { get; set; }
    public DateOnly EffectiveStart { get; set; }
    public DateOnly EffectiveEnd { get; set; }
    public decimal? DailyValue { get; set; }
    public decimal? ExtraWorkingDayFees { get; set; }
    public decimal? OverConsumption { get; set; }
    public string OverrideReason { get; set; } = string.Empty;

    public static ContractBillingSnapshotCommercialOverrideResource FromDto(ContractBillingSnapshotCommercialOverrideDto dto) =>
        new()
        {
            Id = dto.Id,
            SeatRootSlotId = dto.SeatRootSlotId,
            EffectiveStart = dto.EffectiveStart,
            EffectiveEnd = dto.EffectiveEnd,
            DailyValue = dto.DailyValue,
            ExtraWorkingDayFees = dto.ExtraWorkingDayFees,
            OverConsumption = dto.OverConsumption,
            OverrideReason = dto.OverrideReason
        };
}

public class CalculateIncludedContractSlotChargesResource
{
    public IReadOnlyList<ContractBillingSnapshotCommercialOverrideResource> CommercialTermOverrides { get; set; } = [];
    public IReadOnlyList<IncludedContractSlotChargeLineResource> Lines { get; set; } = [];
    public IReadOnlyList<IncludedContractSlotKmSummaryResource> Kms { get; set; } = [];
    public IReadOnlyList<IncludedContractSlotCalendarResource> Calendar { get; set; } = [];
    public Dictionary<string, VehicleRelationResource> Vehicles { get; set; } = new();
    public IncludedContractKmPoolResource? KmPool { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalWorkingDaysAmount { get; set; }
    public decimal TotalExtraWorkingDaysAmount { get; set; }
    public decimal TotalExtraKmsAmount { get; set; }
    public decimal TotalExtraTripsAmount { get; set; }
    public int TotalTripsCount { get; set; }
    public int TotalDaysWithTripsCount { get; set; }
    public int TotalWorkingDaysCount { get; set; }
    public int TotalExtraWorkingDaysCount { get; set; }
    public int TotalExtraTripsCount { get; set; }
    public decimal TotalExtraKmsCount { get; set; }
    public decimal TotalFuelAmount { get; set; }
    public decimal TotalFuelKms { get; set; }
    public int TotalWaitingForLoadingDaysCount { get; set; }
    public int TotalWaitingForLoadingWorkingDaysCount { get; set; }
    public int TotalWaitingForLoadingExtraDaysCount { get; set; }
    public decimal TotalWaitingForLoadingAmount { get; set; }
    public int TotalOvernightTripsCount { get; set; }
    public decimal TotalOvernightCustomerAmount { get; set; }
    public decimal TotalOvernightLoadlyAmount { get; set; }
    public IReadOnlyList<long> SeatRootSlotIds { get; set; } = [];

    public static CalculateIncludedContractSlotChargesResource FromDto(CalculateIncludedContractSlotChargesResultDto dto) =>
        new()
        {
            CommercialTermOverrides = dto.CommercialTermOverrides
                .Select(ContractBillingSnapshotCommercialOverrideResource.FromDto)
                .ToList(),
            Lines = dto.Lines.Select(IncludedContractSlotChargeLineResource.FromDto).ToList(),
            Kms = dto.Kms.Select(IncludedContractSlotKmSummaryResource.FromDto).ToList(),
            Calendar = dto.Calendar.Select(IncludedContractSlotCalendarResource.FromDto).ToList(),
            Vehicles = dto.Vehicles.ToDictionary(
                kvp => kvp.Key.ToString(),
                kvp => VehicleRelationResource.FromDto(kvp.Value)!),
            KmPool = IncludedContractKmPoolResource.FromDto(dto.KmPool),
            TotalAmount = dto.TotalAmount,
            TotalWorkingDaysAmount = dto.TotalWorkingDaysAmount,
            TotalExtraWorkingDaysAmount = dto.TotalExtraWorkingDaysAmount,
            TotalExtraKmsAmount = dto.TotalExtraKmsAmount,
            TotalExtraTripsAmount = dto.TotalExtraTripsAmount,
            TotalTripsCount = dto.TotalTripsCount,
            TotalDaysWithTripsCount = dto.TotalDaysWithTripsCount,
            TotalWorkingDaysCount = dto.TotalWorkingDaysCount,
            TotalExtraWorkingDaysCount = dto.TotalExtraWorkingDaysCount,
            TotalExtraTripsCount = dto.TotalExtraTripsCount,
            TotalExtraKmsCount = dto.TotalExtraKmsCount,
            TotalFuelAmount = dto.TotalFuelAmount,
            TotalFuelKms = dto.TotalFuelKms,
            TotalWaitingForLoadingDaysCount = dto.TotalWaitingForLoadingDaysCount,
            TotalWaitingForLoadingWorkingDaysCount = dto.TotalWaitingForLoadingWorkingDaysCount,
            TotalWaitingForLoadingExtraDaysCount = dto.TotalWaitingForLoadingExtraDaysCount,
            TotalWaitingForLoadingAmount = dto.TotalWaitingForLoadingAmount,
            TotalOvernightTripsCount = dto.TotalOvernightTripsCount,
            TotalOvernightCustomerAmount = dto.TotalOvernightCustomerAmount,
            TotalOvernightLoadlyAmount = dto.TotalOvernightLoadlyAmount,
            SeatRootSlotIds = dto.SeatRootSlotIds
        };
}
