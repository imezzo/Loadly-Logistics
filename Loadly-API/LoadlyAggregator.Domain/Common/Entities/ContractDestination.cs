using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class ContractDestination : BaseEntity
{
    public long ContractId { get; set; }
    public long StartWaypointId { get; set; }
    public long EndWaypointId { get; set; }
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
    public long? AddedBy { get; set; }

    public virtual Contract Contract { get; set; } = null!;
    public virtual Waypoint StartWaypointNavigation { get; set; } = null!;
    public virtual Waypoint EndWaypointNavigation { get; set; } = null!;
}
