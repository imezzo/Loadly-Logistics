namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class VehiclePlateChangeHistoryDto
{
    public string PlateNo { get; set; } = string.Empty;
    public string PlateLetters { get; set; } = string.Empty;
    public DateTime SupersededAt { get; set; }
    public long? AddedBy { get; set; }
}
