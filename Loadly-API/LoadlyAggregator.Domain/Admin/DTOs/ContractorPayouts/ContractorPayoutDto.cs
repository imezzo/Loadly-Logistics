using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.ContractorPayouts;

public class ContractorPayoutDto
{
    public long Id { get; set; }
    public string PayoutNumber { get; set; } = string.Empty;
    public int? Sequence { get; set; }
    public long ContractorId { get; set; }
    public decimal TotalAmount { get; set; }
    public ContractorPayoutStatus Status { get; set; }
    public long? AddedBy { get; set; }
    public long? CancelledBy { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public ContractorRelationDto? Contractor { get; set; }
    public List<ContractorPayoutTripLineDto> Trips { get; set; } = [];
    public int TripCount { get; set; }
    public decimal TotalTripCostAmount { get; set; }
    public decimal TotalContractorPriceAmount { get; set; }
    public decimal LoadlyCommissionAmount { get; set; }
}

public class ContractorPayoutTripLineDto
{
    public long TripId { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public decimal? ContractorPrice { get; set; }
}
