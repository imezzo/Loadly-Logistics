using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.API.Resources.Admin.Dashboard;

public class TripsStatisticsResource
{
    public int TotalTrips { get; set; }
    public int ScheduledTrips { get; set; }
    public int InProgressTrips { get; set; }
    public int CanceledTrips { get; set; }
    public int CompletedTrips { get; set; }
    public int OtherTrips { get; set; }

    public static TripsStatisticsResource FromDto(TripsStatisticsDto dto) => new()
    {
        TotalTrips = dto.TotalTrips,
        ScheduledTrips = dto.ScheduledTrips,
        InProgressTrips = dto.InProgressTrips,
        CanceledTrips = dto.CanceledTrips,
        CompletedTrips = dto.CompletedTrips,
        OtherTrips = dto.OtherTrips
    };
}
