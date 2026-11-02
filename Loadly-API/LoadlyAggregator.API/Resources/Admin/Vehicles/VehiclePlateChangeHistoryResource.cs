namespace LoadlyAggregator.API.Resources.Admin.Vehicles;

public class VehiclePlateChangeHistoryResource
{
    public string PlateNo { get; set; } = string.Empty;
    public string PlateLetters { get; set; } = string.Empty;
    public DateTime SupersededAt { get; set; }
    public long? AddedBy { get; set; }
}
