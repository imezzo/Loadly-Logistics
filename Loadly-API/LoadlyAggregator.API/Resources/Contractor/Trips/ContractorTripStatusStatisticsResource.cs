using LoadlyAggregator.Domain.Contractor.DTOs.Trips;

namespace LoadlyAggregator.API.Resources.Contractor.Trips;

public class ContractorTripStatusStatisticsResource
{
    public int CompletedTrips { get; set; }
    public int ScheduledTrips { get; set; }
    public int InProgressTrips { get; set; }
    public int PendingTrips { get; set; }

    public static ContractorTripStatusStatisticsResource FromDto(ContractorTripStatusStatisticsDto dto) => new()
    {
        CompletedTrips = dto.CompletedTrips,
        ScheduledTrips = dto.ScheduledTrips,
        InProgressTrips = dto.InProgressTrips,
        PendingTrips = dto.PendingTrips
    };
}
