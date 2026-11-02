using System.Linq;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class GetMonthlyTripsAvailableDriversAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IDriverRepository _driverRepository;

    public GetMonthlyTripsAvailableDriversAction(
        ITripRepository tripRepository,
        IDriverRepository driverRepository)
    {
        _tripRepository = tripRepository;
        _driverRepository = driverRepository;
    }

    public async Task<IReadOnlyList<MonthlyTripsAvailableDriversDto>> ExecuteAsync(GetMonthlyTripsAvailableDriversDto request)
    {
        var monthlyTrips = await _tripRepository.GetMonthlyCompletedTripAggregatesAsync(request.Year, request.Month);
        var createdInYearByMonth = (await _driverRepository.GetMonthlyAvailableDriversCountAsync(request.Year))
            .ToDictionary(x => x.Month, x => x.DriversCount);
        int runningAvailableDrivers = await _driverRepository.GetDriversCreatedCountBeforeAsync(new DateTime(request.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var tripsLookup = monthlyTrips.ToDictionary(x => x.Month, x => x.TripsCount);
        var availableDriversLookup = Enumerable.Range(1, 12)
            .Select(m => (Month: m, AvailableDriversCount: runningAvailableDrivers += createdInYearByMonth.GetValueOrDefault(m)))
            .ToDictionary(x => x.Month, x => x.AvailableDriversCount);

        IEnumerable<int> months = request.Month.HasValue ? new[] { request.Month.Value } : Enumerable.Range(1, 12);
        return months.Select(m => new MonthlyTripsAvailableDriversDto
        {
            Month = m,
            TripsCount = tripsLookup.TryGetValue(m, out int tripsCount) ? tripsCount : 0,
            AvailableDriversCount = availableDriversLookup[m]
        }).ToList();
    }
}
