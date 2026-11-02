using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.DTOs.Relations;

/// <summary>Trip reference (e.g. in invoice).</summary>
public class TripRelationDto
{
    public long Id { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public TripStatus Status { get; set; }
    public decimal? TotalTripCost { get; set; }
    public decimal? TotalAddonsCost { get; set; }
    public decimal? ActualDistanceKm { get; set; }
    public DriverRelationDto? Driver { get; set; }
    public List<TripChargeRelationDto> TripCharges { get; set; } = new();
}
