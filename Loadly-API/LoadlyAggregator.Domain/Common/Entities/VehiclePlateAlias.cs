using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

/// <summary>Historical plate values superseded on a vehicle; used to resolve imports/search by old plate.</summary>
public class VehiclePlateAlias : BaseEntity
{
    public long VehicleId { get; set; }
    public string PlateNo { get; set; } = string.Empty;
    public string PlateLetters { get; set; } = string.Empty;

    public long? AddedBy { get; set; }

    public virtual Vehicle? Vehicle { get; set; }
    public virtual Admin? AddedByAdmin { get; set; }
}
