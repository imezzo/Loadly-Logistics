using LoadlyAggregator.API.Resources.Admin.Contracts;
using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.API.Resources.Admin.Invoices;

public class InvoiceSlotChargesResource
{
    public long? BillingSnapshotId { get; set; }
    public decimal SubTotal { get; set; }
    public decimal WorkingDaysAmount { get; set; }
    public decimal ExtraWorkingDaysAmount { get; set; }
    public decimal ExtraKmsAmount { get; set; }
    public decimal ExtraTripsAmount { get; set; }
    public int WorkingDaysCount { get; set; }
    public int ExtraWorkingDaysCount { get; set; }
    public int ExtraTripsCount { get; set; }
    public decimal ExtraKmsCount { get; set; }
    public List<InvoiceSlotChargeGroupResource> Slots { get; set; } = [];
    public Dictionary<string, VehicleRelationResource> Vehicles { get; set; } = new();
    public List<InvoiceSlotCalendarGroupResource> Calendar { get; set; } = [];
    public List<InvoiceSlotKmSummaryResource> KmSummaries { get; set; } = [];
    public IncludedContractKmPoolResource? KmPool { get; set; }

    public static InvoiceSlotChargesResource? FromDto(InvoiceSlotChargesDto? dto) =>
        dto is null
            ? null
            : new InvoiceSlotChargesResource
            {
                BillingSnapshotId = dto.BillingSnapshotId,
                SubTotal = dto.SubTotal,
                WorkingDaysAmount = dto.WorkingDaysAmount,
                ExtraWorkingDaysAmount = dto.ExtraWorkingDaysAmount,
                ExtraKmsAmount = dto.ExtraKmsAmount,
                ExtraTripsAmount = dto.ExtraTripsAmount,
                WorkingDaysCount = dto.WorkingDaysCount,
                ExtraWorkingDaysCount = dto.ExtraWorkingDaysCount,
                ExtraTripsCount = dto.ExtraTripsCount,
                ExtraKmsCount = dto.ExtraKmsCount,
                Slots = dto.Slots.Select(InvoiceSlotChargeGroupResource.FromDto).ToList(),
                Vehicles = dto.Vehicles.ToDictionary(
                    kvp => kvp.Key.ToString(),
                    kvp => VehicleRelationResource.FromDto(kvp.Value)!),
                Calendar = dto.Calendar.Select(InvoiceSlotCalendarGroupResource.FromDto).ToList(),
                KmSummaries = dto.KmSummaries.Select(InvoiceSlotKmSummaryResource.FromDto).ToList(),
                KmPool = IncludedContractKmPoolResource.FromDto(dto.KmPool)
            };
}

public class InvoiceSlotChargeGroupResource
{
    public long? ContractSlotId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<InvoiceSlotChargeLineResource> Lines { get; set; } = [];

    public static InvoiceSlotChargeGroupResource FromDto(InvoiceSlotChargeGroupDto dto) =>
        new()
        {
            ContractSlotId = dto.ContractSlotId,
            TotalAmount = dto.TotalAmount,
            Lines = dto.Lines.Select(InvoiceSlotChargeLineResource.FromDto).ToList()
        };
}

public class InvoiceSlotChargeLineResource
{
    public DateOnly? Date { get; set; }
    public int ChargeKind { get; set; }
    public string ChargeKindName { get; set; } = string.Empty;
    public decimal Amount { get; set; }

    public static InvoiceSlotChargeLineResource FromDto(InvoiceSlotChargeLineDto dto) =>
        new()
        {
            Date = dto.Date,
            ChargeKind = (int)dto.ChargeKind,
            ChargeKindName = dto.ChargeKind.ToLocalizedDisplay(),
            Amount = dto.Amount
        };
}

public class InvoiceSlotCalendarGroupResource
{
    public long ContractSlotId { get; set; }
    public List<InvoiceSlotCalendarDayResource> Days { get; set; } = [];

    public static InvoiceSlotCalendarGroupResource FromDto(InvoiceSlotCalendarGroupDto dto) =>
        new()
        {
            ContractSlotId = dto.ContractSlotId,
            Days = dto.Days.Select(InvoiceSlotCalendarDayResource.FromDto).ToList()
        };
}

public class InvoiceSlotCalendarDayResource
{
    public long? Id { get; set; }
    public DateOnly Date { get; set; }
    public int Disposition { get; set; }
    public string DispositionName { get; set; } = string.Empty;
    public int? DispositionSource { get; set; }
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

    public static InvoiceSlotCalendarDayResource FromDto(InvoiceSlotCalendarDayDto dto) =>
        new()
        {
            Id = dto.Id,
            Date = dto.Date,
            Disposition = (int)dto.Disposition,
            DispositionName = dto.Disposition.ToLocalizedDisplay(),
            DispositionSource = dto.DispositionSource.HasValue ? (int)dto.DispositionSource.Value : null,
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

public class InvoiceSlotKmSummaryResource
{
    public long? ContractSlotId { get; set; }
    public bool IsPooled { get; set; }
    public decimal Consumption { get; set; }
    public decimal? MaxKms { get; set; }
    public decimal OverageKm { get; set; }
    public decimal OverConsumptionRate { get; set; }
    public decimal Amount { get; set; }

    public static InvoiceSlotKmSummaryResource FromDto(InvoiceSlotKmSummaryDto dto) =>
        new()
        {
            ContractSlotId = dto.ContractSlotId,
            IsPooled = dto.IsPooled,
            Consumption = dto.Consumption,
            MaxKms = dto.MaxKms,
            OverageKm = dto.OverageKm,
            OverConsumptionRate = dto.OverConsumptionRate,
            Amount = dto.Amount
        };
}
