namespace LoadlyAggregator.API.Resources.RelationResources;

public class DestinationRelationResource
{
    public long Id { get; set; }
    public WaypointRelationResource Start { get; set; } = null!;
    public WaypointRelationResource End { get; set; } = null!;
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
    public int? Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
