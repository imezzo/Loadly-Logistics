using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

public class ContractSlot : BaseEntity
{
    public DateTime AssignedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public long ContractId { get; set; }
    public long VehicleTypeId { get; set; }
    public long? VehicleId { get; set; }
    public ContractSlotStatus Status { get; set; } = ContractSlotStatus.ACTIVE;
    public decimal? ConsumptionRate { get; set; }
    public int? TripsPerMonth { get; set; }
    public int? TripsPerDay { get; set; }
    public decimal? MaxKms { get; set; }
    public decimal? MonthlyRental { get; set; }
    public decimal? DailyValue { get; set; }
    public decimal? OverConsumption { get; set; }
    public decimal? MinBillableWeightTons { get; set; }
    public decimal? MaxPayloadTons { get; set; }
    public decimal? OverWeightPricePerTon { get; set; }
    public decimal? ContractorOverWeightPricePerTon { get; set; }
    public decimal? ExtraWorkingDayFees { get; set; }
    public long? AddedBy { get; set; }
    public long? ReplacedSlotId { get; set; }

    public virtual Contract? Contract { get; set; }
    public virtual ContractSlot? ReplacedSlot { get; set; }
    public virtual VehicleType? VehicleType { get; set; }
    public virtual Vehicle? Vehicle { get; set; }
}
