using Microsoft.Extensions.Caching.Memory;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class GetSectorAggregationDashboardAction(
    ISectorRepository sectorRepository,
    ITripRepository tripRepository,
    IVehicleRepository vehicleRepository,
    IMemoryCache cache)
{
    private const string CacheKeyPrefix = "admin:dashboard:sectors";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(45);

    public async Task<SectorAggregationDashboardDto> ExecuteAsync(GetSectorAggregationDashboardDto request)
    {
        (DateTime rangeStart, DateTime rangeEnd) = ResolveDateRange(request.StartDate);
        string cacheKey = $"{CacheKeyPrefix}:{rangeStart:O}";

        if (cache.TryGetValue(cacheKey, out SectorAggregationDashboardDto? cached) && cached is not null)
        {
            return cached;
        }

        SectorAggregationDashboardDto result = await BuildAsync(rangeStart, rangeEnd);
        cache.Set(cacheKey, result, CacheTtl);
        return result;
    }

    private static (DateTime From, DateTime To) ResolveDateRange(DateTime? startDate)
    {
        DateTime from = startDate ?? AppUtcTime.UtcTodayAtMidnight(TimeProvider.System);
        return (from, from.AddDays(1));
    }

    private async Task<SectorAggregationDashboardDto> BuildAsync(DateTime rangeStart, DateTime rangeEnd)
    {

        (List<Sector> sectors, _) = await sectorRepository.ListAsync(null, 1, 100);
        IReadOnlyList<(long SectorId, TripStatus Status, int Count)> tripCountsBySector =
            await tripRepository.GetTripStatusCountsBySectorAsync(rangeStart, rangeEnd);
        IReadOnlyList<(long SectorId, int Total, int Active)> vehicleCountsBySector =
            await vehicleRepository.GetVehicleCountsBySectorAsync();

        var tripsBySector = tripCountsBySector
            .GroupBy(x => x.SectorId)
            .ToDictionary(
                g => g.Key,
                g => g.ToDictionary(x => x.Status, x => x.Count));

        var vehiclesBySector = vehicleCountsBySector.ToDictionary(x => x.SectorId, x => (x.Total, x.Active));

        int rangeTotal = await tripRepository.GetTripCountInDateRangeAsync(rangeStart, rangeEnd);
        int previousRangeTotal = await tripRepository.GetTripCountInDateRangeAsync(rangeStart.AddDays(-1), rangeStart);

        return new SectorAggregationDashboardDto
        {
            TotalTrips = new SectorAggregationTotalTripsDto
            {
                Total = rangeTotal,
                VsYesterday = ComparisonCalculator.Compare(rangeTotal, previousRangeTotal)
            },
            Sectors = sectors
                .Select(sector => BuildSectorItem(sector, tripsBySector, vehiclesBySector))
                .OrderByDescending(s => s.DoneTrips)
                .ToList()
        };
    }

    private static SectorAggregationItemDto BuildSectorItem(
        Sector sector,
        IReadOnlyDictionary<long, Dictionary<TripStatus, int>> tripsBySector,
        IReadOnlyDictionary<long, (int Total, int Active)> vehiclesBySector)
    {
        tripsBySector.TryGetValue(sector.Id, out Dictionary<TripStatus, int>? statusCounts);
        statusCounts ??= new Dictionary<TripStatus, int>();

        int inProgress = GetStatusCount(statusCounts, TripStatus.InProgress);
        int scheduled = GetStatusCount(statusCounts, TripStatus.Scheduled);
        int pending = GetStatusCount(statusCounts, TripStatus.Pending);
        int completed = GetStatusCount(statusCounts, TripStatus.Completed);
        int cancelled = GetStatusCount(statusCounts, TripStatus.Cancelled);

        vehiclesBySector.TryGetValue(sector.Id, out (int Total, int Active) vehicleCounts);

        int activePipeline = inProgress + scheduled + pending + completed;

        return new SectorAggregationItemDto
        {
            Id = sector.Id,
            NameEn = sector.NameEn,
            NameAr = sector.NameAr,
            DoneTrips = completed,
            TotalVehicles = vehicleCounts.Total,
            ActiveVehicles = vehicleCounts.Active,
            OverallProgress = activePipeline == 0
                ? 0m
                : DashboardTripMetrics.Round(completed * 100m / activePipeline),
            TripStatuses = new SectorAggregationTripStatusesDto
            {
                InProgress = inProgress,
                Scheduled = scheduled,
                Pending = pending,
                Completed = completed,
                Cancelled = cancelled
            }
        };
    }

    private static int GetStatusCount(IReadOnlyDictionary<TripStatus, int> statusCounts, TripStatus status) =>
        statusCounts.GetValueOrDefault(status);
}
