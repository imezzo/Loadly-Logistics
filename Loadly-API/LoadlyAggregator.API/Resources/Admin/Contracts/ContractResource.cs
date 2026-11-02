using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Resources.Admin.Contracts;

/// <summary>
/// Resource for Contract API responses (both listing and single get)
/// </summary>
public class ContractResource
{
    public long Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public long CompanyId { get; set; }
    public CompanyRelationResource? Company { get; set; }
    public int ContractType { get; set; }
    public decimal Price { get; set; }
    public int PaymentMethod { get; set; }
    public short PaymentDueDays { get; set; }
    public int BillingCycle { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int ConsumptionType { get; set; }
    public decimal CargoCommission { get; set; }
    public int? MonthlyWorkingDays { get; set; }
    public int? VehiclesCount { get; set; }
    public int? Status { get; set; }
    public string? TermsAndConditions { get; set; }
    public long? AddedBy { get; set; }
    public AdminRelationResource? SalesRep { get; set; }
    public ApprovalStage? CurrentApprovalStage { get; set; }
    public DateTime? DeactivationDate { get; set; }
    public long? RenewedFromContractId { get; set; }
    public bool AutoAssignMode { get; set; }
    public int? KmOverageMode { get; set; }
    public string? KmOverageModeName { get; set; }
    public int TotalTrips { get; set; }
    public int TotalUnInvoicedTrips { get; set; }
    public int TotalCompletedTrips { get; set; }
    public decimal TotalTripsCost { get; set; }
    public decimal TotalCompletedTripsCost { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ContractApprovalRelationResource> Approvals { get; set; } = new();
    public List<ContractStatusHistoryRelationResource> StatusHistory { get; set; } = new();
    public List<ContractVehicleTypeRelationResource> VehicleTypes { get; set; } = new();
    public List<ContractDocumentRelationResource> Documents { get; set; } = new();
    public List<ContractExtraFeeRelationResource> ExtraFees { get; set; } = new();
    public List<DestinationRelationResource> Destinations { get; set; } = new();
    public List<ContractSlotRelationResource> Slots { get; set; } = new();
}

public class ContractDestinationResource : DestinationRelationResource
{
    public long ContractId { get; set; }
}

public class ContractApprovalRelationResource
{
    public long Id { get; set; }
    public long ContractId { get; set; }
    public int Stage { get; set; }
    public string StageName { get; set; } = string.Empty;
    public long? ApproverId { get; set; }
    public string? ApproverName { get; set; }
    public string? ApproverRole { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public DateTime? ActionDate { get; set; }
    public int StageOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ContractStatusHistoryRelationResource
{
    public long Id { get; set; }
    public long ContractId { get; set; }
    public ContractStatus PreviousStatus { get; set; }
    public string PreviousStatusName { get; set; } = string.Empty;
    public ContractStatus NewStatus { get; set; }
    public string NewStatusName { get; set; } = string.Empty;
    public ApprovalStage? ApprovalStage { get; set; }
    public string? ApprovalStageName { get; set; }
    public long? ChangedBy { get; set; }
    public string? ChangedByName { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? AdditionalNotes { get; set; }
}

public class ContractDocumentRelationResource
{
    public long Id { get; set; }
    public long ContractId { get; set; }
    public long DocumentId { get; set; }
    public string? Path { get; set; }
    public string? FileUrl { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DocumentRelationResource? Document { get; set; }
}

public class ContractExtraFeeRelationResource
{
    public long Id { get; set; }
    public long ContractId { get; set; }
    public long ExtraFeeTypeId { get; set; }
    public int? CalculationOption { get; set; }
    public decimal? Value { get; set; }
    public int? Threshold { get; set; }
    public string? Notes { get; set; }
    public bool? AllowOverride { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ExtraFeeTypeRelationResource? ExtraFeeType { get; set; }
}

public class ContractSlotRelationResource
{
    public long Id { get; set; }
    public long ContractId { get; set; }
    public long VehicleTypeId { get; set; }
    public long? VehicleId { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
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
    public long? ReplacedSlotId { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public VehicleTypeRelationResource? VehicleType { get; set; }
    public VehicleRelationResource? Vehicle { get; set; }
}

