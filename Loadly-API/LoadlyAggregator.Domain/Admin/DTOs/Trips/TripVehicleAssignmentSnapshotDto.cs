namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class TripVehicleAssignmentSnapshotDto
{
    public string HeadPlateNo { get; set; } = string.Empty;
    public string HeadPlateLetters { get; set; } = string.Empty;
    public string? TailPlateNo { get; set; }
    public string? TailPlateLetters { get; set; }
}
