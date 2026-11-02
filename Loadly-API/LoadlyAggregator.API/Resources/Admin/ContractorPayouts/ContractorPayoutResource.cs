using LoadlyAggregator.API.Resources.RelationResources;

namespace LoadlyAggregator.API.Resources.Admin.ContractorPayouts;

public class ContractorPayoutResource
{
    public long Id { get; set; }
    public string PayoutNumber { get; set; } = string.Empty;
    public int? Sequence { get; set; }
    public long ContractorId { get; set; }
    public decimal TotalAmount { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public long? AddedBy { get; set; }
    public long? CancelledBy { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public ContractorRelationResource? Contractor { get; set; }
    public List<ContractorPayoutTripLineResource> Trips { get; set; } = [];
    public int TripCount { get; set; }
    public decimal TotalTripCostAmount { get; set; }
    public decimal TotalContractorPriceAmount { get; set; }
    public decimal LoadlyCommissionAmount { get; set; }
}

public class ContractorPayoutTripLineResource
{
    public long TripId { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public decimal? ContractorPrice { get; set; }
}
