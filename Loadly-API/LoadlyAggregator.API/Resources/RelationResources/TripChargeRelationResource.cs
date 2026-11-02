namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Trip charge reference (e.g. in invoice trip).</summary>
public class TripChargeRelationResource
{
    public long Id { get; set; }
    public int ChargeType { get; set; }
    public string ChargeTypeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int? ExtraFeeTypeId { get; set; }
}
