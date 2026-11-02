using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Dashboard;

namespace LoadlyAggregator.Domain.Company.Actions.Dashboard;

public class CompanyGetTripsStatisticsAction(ITripRepository tripRepository)
{
    private readonly ITripRepository _tripRepository = tripRepository;

    public async Task<CompanyTripsStatisticsDto> ExecuteAsync(CompanyGetDashboardTripsStatisticsDto request)
    {
        ResolveDateRange(request, out DateTime? from, out DateTime? to);
        var statusCounts = await _tripRepository.GetTripStatusCountsForCompanyAsync(request.CompanyId, from, to);

        return new CompanyTripsStatisticsDto
        {
            TotalTrips = statusCounts.Values.Sum(),
            ScheduledTrips = statusCounts.GetValueOrDefault(TripStatus.Scheduled),
            InProgressTrips = statusCounts.GetValueOrDefault(TripStatus.InProgress),
            CanceledTrips = statusCounts.GetValueOrDefault(TripStatus.Cancelled),
            CompletedTrips = statusCounts.GetValueOrDefault(TripStatus.Completed),
            OtherTrips = statusCounts.GetValueOrDefault(TripStatus.Pending)
        };
    }

    private static void ResolveDateRange(CompanyGetDashboardTripsStatisticsDto request, out DateTime? from, out DateTime? to) =>
        CompanyDashboardDateRangeResolver.Resolve(request.Year, request.LastMonth, request.LastWeek, out from, out to);
}
