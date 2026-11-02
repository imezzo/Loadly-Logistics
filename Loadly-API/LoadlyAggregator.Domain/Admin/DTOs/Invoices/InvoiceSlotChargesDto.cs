using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;

namespace LoadlyAggregator.Domain.Admin.DTOs.Invoices;

public class InvoiceSlotChargesDto
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
    public List<InvoiceSlotChargeGroupDto> Slots { get; set; } = [];
    public IReadOnlyDictionary<long, VehicleRelationDto> Vehicles { get; set; } =
        new Dictionary<long, VehicleRelationDto>();
    public List<InvoiceSlotCalendarGroupDto> Calendar { get; set; } = [];
    public List<InvoiceSlotKmSummaryDto> KmSummaries { get; set; } = [];
    public IncludedContractKmPoolDto? KmPool { get; set; }

    public static InvoiceSlotChargesDto? FromInvoice(Invoice invoice)
    {
        if (!invoice.SlotChargesCalculatedAt.HasValue || invoice.ContractBillingSnapshot is null)
            return null;

        var snapshot = invoice.ContractBillingSnapshot;
        var calculation = ContractBillingSnapshotReader.ToCalculationDto(snapshot);
        var extraKmRoute = ContractSlotChargeInvoiceRouting.ResolveExtraKmOverageInvoiceType(snapshot);
        var filteredLines = calculation.Lines
            .Where(line =>
                ContractSlotChargeInvoiceRouting.GetEffectiveInvoiceType(
                    (ContractSlotChargeKind)line.ChargeKind,
                    extraKmRoute)
                == invoice.InvoiceType)
            .ToList();

        return new InvoiceSlotChargesDto
        {
            BillingSnapshotId = snapshot.Id,
            SubTotal = invoice.SlotChargesSubTotal,
            WorkingDaysAmount = invoice.SlotWorkingDaysAmount,
            ExtraWorkingDaysAmount = invoice.SlotExtraWorkingDaysAmount,
            ExtraKmsAmount = invoice.SlotExtraKmsAmount,
            ExtraTripsAmount = invoice.SlotExtraTripsAmount,
            WorkingDaysCount = invoice.SlotWorkingDaysCount,
            ExtraWorkingDaysCount = invoice.SlotExtraWorkingDaysCount,
            ExtraTripsCount = invoice.SlotExtraTripsCount,
            ExtraKmsCount = invoice.SlotExtraKmsCount,
            Slots = GroupChargeLines(filteredLines),
            Calendar = snapshot.CalendarDays
                .Where(day => day.DeletedAt == null)
                .GroupBy(day => day.ContractSlotId)
                .OrderBy(group => group.Key)
                .Select(group => new InvoiceSlotCalendarGroupDto
                {
                    ContractSlotId = group.Key,
                    Days = group
                        .OrderBy(day => day.Date)
                        .Select(day => new InvoiceSlotCalendarDayDto
                        {
                            Id = day.Id,
                            Date = day.Date,
                            Disposition = day.Disposition,
                            DispositionSource = day.DispositionSource,
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
                .ToList(),
            KmSummaries = invoice.SlotExtraKmsAmount > 0m
                ? snapshot.KmEvidence
                    .Where(km => km.DeletedAt == null)
                    .Select(InvoiceSlotKmSummaryDto.FromEntity)
                    .ToList()
                : [],
            KmPool = ContractBillingSnapshotReader.DeserializeKmPool(snapshot.KmPoolJson)
        };
    }

    private static List<InvoiceSlotChargeGroupDto> GroupChargeLines(
        IReadOnlyList<IncludedContractSlotChargeLineDto> lines) =>
        lines
            .GroupBy(line => line.ContractSlotId > 0 ? line.ContractSlotId : (long?)null)
            .OrderBy(group => group.Key ?? long.MaxValue)
            .Select(group => new InvoiceSlotChargeGroupDto
            {
                ContractSlotId = group.Key,
                TotalAmount = group.Sum(line => line.Amount),
                Lines = group
                    .OrderBy(line => line.Date ?? DateOnly.MaxValue)
                    .ThenBy(line => line.ChargeKind)
                    .Select(line => new InvoiceSlotChargeLineDto
                    {
                        Date = line.Date,
                        ChargeKind = (ContractSlotChargeKind)line.ChargeKind,
                        Amount = line.Amount
                    })
                    .ToList()
            })
            .ToList();
}

public class InvoiceSlotChargeGroupDto
{
    public long? ContractSlotId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<InvoiceSlotChargeLineDto> Lines { get; set; } = [];
}

public class InvoiceSlotChargeLineDto
{
    public DateOnly? Date { get; set; }
    public ContractSlotChargeKind ChargeKind { get; set; }
    public decimal Amount { get; set; }
}

public class InvoiceSlotCalendarGroupDto
{
    public long ContractSlotId { get; set; }
    public List<InvoiceSlotCalendarDayDto> Days { get; set; } = [];
}

public class InvoiceSlotCalendarDayDto
{
    public long? Id { get; set; }
    public DateOnly Date { get; set; }
    public ContractSlotDayDisposition Disposition { get; set; }
    public ContractBillingSnapshotDispositionSource? DispositionSource { get; set; }
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

public class InvoiceSlotKmSummaryDto
{
    public long? ContractSlotId { get; set; }
    public bool IsPooled { get; set; }
    public decimal Consumption { get; set; }
    public decimal? MaxKms { get; set; }
    public decimal OverageKm { get; set; }
    public decimal OverConsumptionRate { get; set; }
    public decimal Amount { get; set; }

    public static InvoiceSlotKmSummaryDto FromEntity(ContractBillingSnapshotKmEvidence summary) => new()
    {
        ContractSlotId = summary.ContractSlotId,
        IsPooled = summary.IsPooled,
        Consumption = summary.Consumption,
        MaxKms = summary.MaxKms,
        OverageKm = summary.OverageKm,
        OverConsumptionRate = summary.OverConsumptionRate,
        Amount = summary.Amount
    };
}
