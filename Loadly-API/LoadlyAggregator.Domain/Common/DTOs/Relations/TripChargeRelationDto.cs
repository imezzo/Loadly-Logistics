namespace LoadlyAggregator.Domain.Common.DTOs.Relations;

/// <summary>Trip charge reference (e.g. in invoice trip).</summary>
public class TripChargeRelationDto
{
    public long Id { get; set; }
    public int ChargeType { get; set; }
    public string ChargeTypeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int? ExtraFeeTypeId { get; set; }
}
