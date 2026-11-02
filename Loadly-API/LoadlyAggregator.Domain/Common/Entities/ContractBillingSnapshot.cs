using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class ContractBillingSnapshot : BaseEntity
{
    public long ContractId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public ContractType ContractType { get; set; }
    public ContractBillingSnapshotStatus Status { get; set; }
    public int EngineVersion { get; set; }
    public long? InvoiceId { get; set; }
    public long? ClaimInvoiceId { get; set; }
    public InvoiceType? ExtraKmOverageInvoiceType { get; set; }
    public DateTime? FinalizedAt { get; set; }

    public decimal SubTotal { get; set; }
    public decimal WorkingDaysAmount { get; set; }
    public decimal ExtraWorkingDaysAmount { get; set; }
    public decimal ExtraKmsAmount { get; set; }
    public decimal ExtraTripsAmount { get; set; }
    public int WorkingDaysCount { get; set; }
    public int ExtraWorkingDaysCount { get; set; }
    public int ExtraTripsCount { get; set; }
    public decimal ExtraKmsCount { get; set; }
    public string? KmPoolJson { get; set; }

    public virtual Contract? Contract { get; set; }
    public virtual Invoice? Invoice { get; set; }
    public virtual ICollection<ContractBillingSnapshotCalendarDay> CalendarDays { get; set; } =
        new List<ContractBillingSnapshotCalendarDay>();
    public virtual ICollection<ContractBillingSnapshotChargeLine> ChargeLines { get; set; } =
        new List<ContractBillingSnapshotChargeLine>();
    public virtual ICollection<ContractBillingSnapshotKmEvidence> KmEvidence { get; set; } =
        new List<ContractBillingSnapshotKmEvidence>();
    public virtual ICollection<ContractBillingSnapshotCommercialOverride> CommercialOverrides { get; set; } =
        new List<ContractBillingSnapshotCommercialOverride>();
}
