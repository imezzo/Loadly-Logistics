namespace LoadlyAggregator.API.Resources.Admin.Trips;

public class TripVehicleAssignmentSnapshotResource
{
    public string HeadPlateNo { get; set; } = string.Empty;
    public string HeadPlateLetters { get; set; } = string.Empty;
    public string? TailPlateNo { get; set; }
    public string? TailPlateLetters { get; set; }
}
