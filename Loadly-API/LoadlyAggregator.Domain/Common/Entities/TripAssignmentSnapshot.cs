using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

/// <summary>Frozen head/tail plates at trip assignment time (immutable for completed trips).</summary>
public class TripVehicleAssignmentSnapshot : BaseEntity
{
    public long TripId { get; set; }

    public string HeadPlateNo { get; set; } = string.Empty;
    public string HeadPlateLetters { get; set; } = string.Empty;
    public string? TailPlateNo { get; set; }
    public string? TailPlateLetters { get; set; }

    public long? AddedBy { get; set; }

    public virtual Trip? Trip { get; set; }
    public virtual Admin? AddedByAdmin { get; set; }
}
