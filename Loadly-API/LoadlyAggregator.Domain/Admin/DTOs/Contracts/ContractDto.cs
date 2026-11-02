using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.Domain.Admin.DTOs.Contracts;

public class ContractDto
{
    public long Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public long CompanyId { get; set; }
    public CompanyRelationDto? Company { get; set; }
    public ContractType ContractType { get; set; }
    public decimal Price { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public short PaymentDueDays { get; set; }
    public ContractBillingCycles BillingCycle { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ContractConsumptionTypes ConsumptionType { get; set; }
    public decimal? CargoCommission { get; set; }
    public int? MonthlyWorkingDays { get; set; }
    public int? VehiclesCount { get; set; }
    public ContractStatus Status { get; set; }
    public string? TermsAndConditions { get; set; }
    public long? AddedBy { get; set; }
    public AdminRelationDto? SalesRep { get; set; }
    public ApprovalStage? CurrentApprovalStage { get; set; }
    public DateTime? DeactivationDate { get; set; }
    public long? RenewedFromContractId { get; set; }
    public bool AutoAssignMode { get; set; }
    public ContractKmOverageMode? KmOverageMode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ContractApprovalDto> Approvals { get; set; } = new();
    public List<ContractStatusHistoryDto> StatusHistory { get; set; } = new();
    public List<ContractVehicleTypeRelationDto> VehicleTypes { get; set; } = new();
    public List<ContractDocumentDto> Documents { get; set; } = new();
    public List<ContractExtraFeeDto> ExtraFees { get; set; } = new();
    public List<DestinationRelationDto> Destinations { get; set; } = new();
    public List<ContractSlotDto> Slots { get; set; } = new();
    public int TotalTrips { get; set; }
    public int TotalUnInvoicedTrips { get; set; }
    public int TotalCompletedTrips { get; set; }
    public decimal TotalTripsCost { get; set; }
    public decimal TotalCompletedTripsCost { get; set; }
}

public class CreateContractDto
{
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
    public decimal? CargoCommission { get; set; }
    public int? MonthlyWorkingDays { get; set; }
    public int? VehiclesCount { get; set; }
    /// <summary>Optional. If true, contract is submitted for approval after creation; otherwise it remains in Draft (default).</summary>
    public bool SubmitForApproval { get; set; } = false;
    public string? TermsAndConditions { get; set; }
    public long? SalesRepId { get; set; }
    /// <summary>Optional. When set, creates a renewal: new contract is linked to this one and the existing contract is marked RENEWED. That contract must be ACTIVE or EXPIRED. The new contract must have the same CompanyId and ContractTypeId as the original.</summary>
    public long? RenewedFromContractId { get; set; }
    public bool AutoAssignMode { get; set; }
    public ContractKmOverageMode? KmOverageMode { get; set; }
    public List<CreateContractVehicleTypeDto>? VehicleTypes { get; set; }
    public List<DocumentItemDto>? Documents { get; set; }
    public List<ExtraFeeItemDto>? ExtraFees { get; set; }
    public List<ContractDestinationItemDto>? Destinations { get; set; }
    public List<ContractSlotItemDto>? Slots { get; set; }
}

public class DocumentItemDto
{
    public long DocumentId { get; set; }
    public FileUpload? File { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DocumentVerificationStatus Status { get; set; } = DocumentVerificationStatus.PENDING;
}

public class ExtraFeeItemDto
{
    public long ExtraFeeTypeId { get; set; }
    public int? CalculationOption { get; set; }
    public decimal? Value { get; set; }
    public int? Threshold { get; set; }
    public string? Notes { get; set; }
    public bool? AllowOverride { get; set; }
}

public class ContractDestinationWaypointItemDto
{
    public long? Id { get; set; }
    public string? GooglePlaceId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string AddressName { get; set; } = string.Empty;
}

public class ContractDestinationItemDto
{
    public ContractDestinationWaypointItemDto Start { get; set; } = null!;
    public ContractDestinationWaypointItemDto End { get; set; } = null!;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public decimal? ExpectedKms { get; set; }
    public decimal? ReturnExpectedKms { get; set; }
    public int? NumberOfDays { get; set; }
    public decimal? PricePerTon { get; set; }
    public decimal? FixedTripPrice { get; set; }
    public decimal? ContractorPricePerTon { get; set; }
    public decimal? ContractorFixedTripPrice { get; set; }
    public string? Description { get; set; }
}

public class ContractSlotItemDto
{
    public long? Id { get; set; }
    public long VehicleTypeId { get; set; }
    public long? VehicleId { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
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
    public long? ReplacedSlotId { get; set; }
}

public class ContractSlotDto
{
    public long Id { get; set; }
    public long ContractId { get; set; }
    public long VehicleTypeId { get; set; }
    public long? VehicleId { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public ContractSlotStatus Status { get; set; }
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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public VehicleTypeRelationDto? VehicleType { get; set; }
    public VehicleRelationDto? Vehicle { get; set; }
}

public class UpdateContractDto
{
    public long Id { get; set; }
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
    public decimal? CargoCommission { get; set; }
    public int? MonthlyWorkingDays { get; set; }
    public int? VehiclesCount { get; set; }
    /// <summary>Optional. If true and contract is DRAFT, submit for approval after update; otherwise leave as DRAFT (default).</summary>
    public bool SubmitForApproval { get; set; } = false;
    public string? TermsAndConditions { get; set; }
    public long? SalesRepId { get; set; }
    public bool AutoAssignMode { get; set; }
    public ContractKmOverageMode? KmOverageMode { get; set; }
    public List<ContractVehicleTypeDto>? VehicleTypes { get; set; }
    public List<DocumentItemDto>? Documents { get; set; }
    public List<ExtraFeeItemDto>? ExtraFees { get; set; }
    public List<ContractDestinationItemDto>? Destinations { get; set; }
    public List<ContractSlotItemDto>? Slots { get; set; }
}

public class GetContractDto
{
    public long Id { get; set; }
}

public class ContractDestinationDto : DestinationRelationDto
{
    public long ContractId { get; set; }
}

public class GetContractDestinationDto
{
    public long Id { get; set; }
}

public class DeleteContractDto
{
    public long Id { get; set; }
}

public class ListContractsDto : BasePagedRequest
{
    public long? ContractId { get; set; }
    public long? CompanyId { get; set; }
    public ContractType? ContractType { get; set; }
    public long? SalesRepId { get; set; }
    public ContractStatus? Status { get; set; }
    public string? ReferenceNumber { get; set; }
}

public class TerminateContractDto
{
    public long ContractId { get; set; }
    public string? Notes { get; set; }
}

public class ReactivateContractDto
{
    public long ContractId { get; set; }
    public string? Notes { get; set; }
}

public class PullBackToDraftContractDto
{
    public long ContractId { get; set; }
}

public class DeactivateContractDto
{
    public long ContractId { get; set; }
    public DeactivationReason Reason { get; set; }
    public DateTime EffectiveDate { get; set; }
    public string? Notes { get; set; }
}
