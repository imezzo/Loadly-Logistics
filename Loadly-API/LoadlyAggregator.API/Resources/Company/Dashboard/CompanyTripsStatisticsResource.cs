using LoadlyAggregator.Domain.Company.DTOs.Dashboard;

namespace LoadlyAggregator.API.Resources.Company.Dashboard;

public class CompanyTripsStatisticsResource
{
    public int TotalTrips { get; set; }
    public int ScheduledTrips { get; set; }
    public int InProgressTrips { get; set; }
    public int CanceledTrips { get; set; }
    public int CompletedTrips { get; set; }
    public int OtherTrips { get; set; }

    public static CompanyTripsStatisticsResource FromDto(CompanyTripsStatisticsDto dto) => new()
    {
        TotalTrips = dto.TotalTrips,
        ScheduledTrips = dto.ScheduledTrips,
        InProgressTrips = dto.InProgressTrips,
        CanceledTrips = dto.CanceledTrips,
        CompletedTrips = dto.CompletedTrips,
        OtherTrips = dto.OtherTrips
    };
}
