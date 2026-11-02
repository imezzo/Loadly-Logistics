using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class Contract : BaseEntity
{
    public string ReferenceNumber { get; set; } = string.Empty;
    public int ReferenceSequence { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public long CompanyId { get; set; }
    public ContractType ContractType { get; set; }
    public decimal Price { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public short PaymentDueDays { get; set; }
    public ContractBillingCycles BillingCycle { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ContractConsumptionTypes ConsumptionType { get; set; }
    public decimal CargoCommission { get; set; }
    public int? MonthlyWorkingDays { get; set; }
    public ContractStatus Status { get; set; }
    public string? TermsAndConditions { get; set; }
    public long? AddedBy { get; set; }
    public long? SalesRepId { get; set; }
    public ApprovalStage? CurrentApprovalStage { get; set; }
    public DateTime? DeactivationDate { get; set; }
    public long? RenewedFromContractId { get; set; }
    public bool AutoAssignMode { get; set; }
    public ContractKmOverageMode? KmOverageMode { get; set; }
    public int? VehiclesCount { get; set; }
    public virtual Company? Company { get; set; }
    public virtual Admin? SalesRep { get; set; }
    public virtual ICollection<ContractApproval> Approvals { get; set; } = new List<ContractApproval>();
    public virtual ICollection<ContractStatusHistory> StatusHistories { get; set; } = new List<ContractStatusHistory>();
    public virtual ICollection<ContractVehicleType> ContractVehicleTypes { get; set; } = new List<ContractVehicleType>();
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public virtual ICollection<ContractDocument> ContractDocuments { get; set; } = new List<ContractDocument>();
    public virtual ICollection<ContractExtraFee> ContractExtraFees { get; set; } = new List<ContractExtraFee>();
    public virtual ICollection<ContractDestination> ContractDestinations { get; set; } = new List<ContractDestination>();
    public virtual ICollection<ContractSlot> ContractSlots { get; set; } = new List<ContractSlot>();
    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
    public virtual Contract? RenewedFromContract { get; set; }
}
