using LoadlyAggregator.Domain.Admin.DTOs.Drivers;

namespace LoadlyAggregator.API.Resources.Admin.Drivers;

public class DriversWorkingStatisticsResource
{
    public int TotalDrivers { get; set; }
    public int Available { get; set; }
    public int Busy { get; set; }
    public int OnBreak { get; set; }
    public int Offline { get; set; }
    public int OffDuty { get; set; }

    public static DriversWorkingStatisticsResource FromDto(DriversWorkingStatisticsDto dto) => new()
    {
        TotalDrivers = dto.TotalDrivers,
        Available = dto.Available,
        Busy = dto.Busy,
        OnBreak = dto.OnBreak,
        Offline = dto.Offline,
        OffDuty = dto.OffDuty
    };
}
