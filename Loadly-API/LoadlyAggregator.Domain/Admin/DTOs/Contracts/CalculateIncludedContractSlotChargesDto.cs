namespace LoadlyAggregator.Domain.Admin.DTOs.Contracts;

using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Enums;

public class CalculateContractSlotChargesDto
{
    public long ContractId { get; set; }
    public long? ContractSlotId { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public string? Name { get; set; }
}

public class IncludedContractSlotChargeLineDto
{
    public long ContractSlotId { get; set; }
    public DateOnly? Date { get; set; }
    public int ChargeKind { get; set; }
    public decimal Amount { get; set; }
}

public class IncludedContractSlotKmSummaryDto
{
    public long SlotId { get; set; }
    public decimal Consumption { get; set; }
    public decimal OverageKm { get; set; }
    public decimal OverConsumptionRate { get; set; }
    public decimal SuggestedOverageAmount { get; set; }
}

public class IncludedContractSlotCalendarDayDto
{
    public long? CalendarDayId { get; set; }
    public DateOnly Date { get; set; }
    public ContractSlotDayDisposition Disposition { get; set; }
    public ContractBillingSnapshotDispositionSource? DispositionSource { get; set; }
    public string? OverrideReason { get; set; }
    public int TripCount { get; set; }
    public IReadOnlyList<long> TripIds { get; set; } = [];
    public ContractBillingSnapshotDispositionSource? TripCountSource { get; set; }
    public int? WorkingDayIndex { get; set; }
    public bool IsExtra { get; set; }
    public VehicleFacilityType? FacilityType { get; set; }
    public long? FacilityId { get; set; }
    public string? FacilityName { get; set; }
    public long? ChangedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public IReadOnlyList<IncludedContractSlotCalendarDayTripDto> Trips { get; set; } = [];
}

public class IncludedContractSlotCalendarDayTripDto
{
    public long TripId { get; set; }
    public DateTime? StartDate { get; set; }
    public ContractBillingCalendarTripRole Role { get; set; }
    public ContractSlotChargeKind ChargeKind { get; set; }
    public decimal Amount { get; set; }
    public bool HasOvernight { get; set; }
    public decimal OvernightAmount { get; set; }
}

public class IncludedContractSlotCalendarDto
{
    public long SlotId { get; set; }
    public long SeatRootSlotId { get; set; }
    public IReadOnlyList<IncludedContractSlotCalendarDayDto> Days { get; set; } = [];
}

public class IncludedContractKmPoolDto
{
    public decimal TotalConsumption { get; set; }
    public decimal TotalMaxKms { get; set; }
    public decimal OverageKm { get; set; }
    public decimal OverConsumptionRate { get; set; }
    public decimal SuggestedOverageAmount { get; set; }
}

public class ContractBillingSnapshotCommercialOverrideDto
{
    public long Id { get; set; }
    public long SeatRootSlotId { get; set; }
    public DateOnly EffectiveStart { get; set; }
    public DateOnly EffectiveEnd { get; set; }
    public decimal? DailyValue { get; set; }
    public decimal? ExtraWorkingDayFees { get; set; }
    public decimal? OverConsumption { get; set; }
    public string OverrideReason { get; set; } = string.Empty;
}

public class CalculateIncludedContractSlotChargesResultDto
{
    public IReadOnlyDictionary<long, VehicleRelationDto> Vehicles { get; set; } =
        new Dictionary<long, VehicleRelationDto>();
    public IReadOnlyList<ContractBillingSnapshotCommercialOverrideDto> CommercialTermOverrides { get; set; } = [];
    public IReadOnlyList<IncludedContractSlotChargeLineDto> Lines { get; set; } = [];
    public IReadOnlyList<IncludedContractSlotKmSummaryDto> Kms { get; set; } = [];
    public IReadOnlyList<IncludedContractSlotCalendarDto> Calendar { get; set; } = [];
    public IncludedContractKmPoolDto? KmPool { get; set; }
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
}
