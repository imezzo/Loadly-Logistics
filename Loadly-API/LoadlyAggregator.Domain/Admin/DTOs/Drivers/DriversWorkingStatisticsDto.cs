namespace LoadlyAggregator.Domain.Admin.DTOs.Drivers;

public class DriversWorkingStatisticsDto
{
    public int TotalDrivers { get; set; }
    public int Available { get; set; }
    public int Busy { get; set; }
    public int OnBreak { get; set; }
    public int Offline { get; set; }
    public int OffDuty { get; set; }
}
