using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.DTOs.Relations;

public class DestinationRelationDto
{
    public long Id { get; set; }
    public WaypointRelationDto Start { get; set; } = null!;
    public WaypointRelationDto End { get; set; } = null!;
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
    public ContractDestinationStatus? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
