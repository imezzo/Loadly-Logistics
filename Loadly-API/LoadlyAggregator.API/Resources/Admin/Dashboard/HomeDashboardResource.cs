using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Resources.Admin.Dashboard;

public class HomeDashboardResource
{
    public HomeTotalTripsResource TotalTrips { get; set; } = new();
    public HomeTripPerformanceResource TripPerformance { get; set; } = new();
    public DriverFleetOverviewResource DriverFleetOverview { get; set; } = new();
    public HomeContractorActivityResource ContractorActivity { get; set; } = new();

    public static HomeDashboardResource FromDto(HomeDashboardDto dto) => new()
    {
        TotalTrips = HomeTotalTripsResource.FromDto(dto.TotalTrips),
        TripPerformance = HomeTripPerformanceResource.FromDto(dto.TripPerformance),
        DriverFleetOverview = DriverFleetOverviewResource.FromDto(dto.DriverFleetOverview),
        ContractorActivity = HomeContractorActivityResource.FromDto(dto.ContractorActivity)
    };
}

public class HomeTotalTripsResource
{
    public int Total { get; set; }
    public int Completed { get; set; }
    public int InProgress { get; set; }
    public int Pending { get; set; }
    public int Cancelled { get; set; }
    public PeriodComparisonResource VsYesterday { get; set; } = new();
    public decimal SevenDayAverage { get; set; }

    public static HomeTotalTripsResource FromDto(HomeTotalTripsDto dto) => new()
    {
        Total = dto.Total,
        Completed = dto.Completed,
        InProgress = dto.InProgress,
        Pending = dto.Pending,
        Cancelled = dto.Cancelled,
        VsYesterday = PeriodComparisonResource.FromDto(dto.VsYesterday),
        SevenDayAverage = dto.SevenDayAverage
    };
}

public class HomeTripPerformanceResource
{
    public decimal ScheduledRate { get; set; }
    public PeriodComparisonResource ScheduledVsLastWeek { get; set; } = new();
    public decimal InProgressRate { get; set; }
    public decimal CompletedRate { get; set; }
    public PeriodComparisonResource CompletedVsLastWeek { get; set; } = new();
    public decimal AverageTripDurationMinutes { get; set; }
    public int PendingTripsCount { get; set; }
    public decimal CancellationRate { get; set; }

    public static HomeTripPerformanceResource FromDto(HomeTripPerformanceDto dto) => new()
    {
        ScheduledRate = dto.ScheduledRate,
        ScheduledVsLastWeek = PeriodComparisonResource.FromDto(dto.ScheduledVsLastWeek),
        InProgressRate = dto.InProgressRate,
        CompletedRate = dto.CompletedRate,
        CompletedVsLastWeek = PeriodComparisonResource.FromDto(dto.CompletedVsLastWeek),
        AverageTripDurationMinutes = dto.AverageTripDurationMinutes,
        PendingTripsCount = dto.PendingTripsCount,
        CancellationRate = dto.CancellationRate
    };
}

public class HomeContractorActivityResource
{
    public int ActiveExternalContractorTrips { get; set; }
    public PeriodComparisonResource VsWeeklyAverage { get; set; } = new();
    public IReadOnlyList<int> Trend { get; set; } = Array.Empty<int>();

    public static HomeContractorActivityResource FromDto(HomeContractorActivityDto dto) => new()
    {
        ActiveExternalContractorTrips = dto.ActiveExternalContractorTrips,
        VsWeeklyAverage = PeriodComparisonResource.FromDto(dto.VsWeeklyAverage),
        Trend = dto.Trend
    };
}

public class PeriodComparisonResource
{
    public decimal Current { get; set; }
    public decimal Previous { get; set; }
    public decimal Delta { get; set; }
    public decimal PercentChange { get; set; }
    public string Direction { get; set; } = TrendDirection.Flat.ToString().ToLowerInvariant();

    public static PeriodComparisonResource FromDto(PeriodComparisonDto dto) => new()
    {
        Current = dto.Current,
        Previous = dto.Previous,
        Delta = dto.Delta,
        PercentChange = dto.PercentChange,
        Direction = dto.Direction.ToString().ToLowerInvariant()
    };
}
