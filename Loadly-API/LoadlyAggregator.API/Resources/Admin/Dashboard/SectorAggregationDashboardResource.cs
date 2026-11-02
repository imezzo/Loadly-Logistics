using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.API.Resources.Admin.Dashboard;

public class SectorAggregationDashboardResource
{
    public SectorAggregationTotalTripsResource TotalTrips { get; set; } = new();
    public IReadOnlyList<SectorAggregationItemResource> Sectors { get; set; } = Array.Empty<SectorAggregationItemResource>();

    public static SectorAggregationDashboardResource FromDto(SectorAggregationDashboardDto dto) => new()
    {
        TotalTrips = SectorAggregationTotalTripsResource.FromDto(dto.TotalTrips),
        Sectors = dto.Sectors.Select(SectorAggregationItemResource.FromDto).ToList()
    };
}

public class SectorAggregationTotalTripsResource
{
    public int Total { get; set; }
    public PeriodComparisonResource VsYesterday { get; set; } = new();

    public static SectorAggregationTotalTripsResource FromDto(SectorAggregationTotalTripsDto dto) => new()
    {
        Total = dto.Total,
        VsYesterday = PeriodComparisonResource.FromDto(dto.VsYesterday)
    };
}

public class SectorAggregationItemResource
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public int DoneTrips { get; set; }
    public int TotalVehicles { get; set; }
    public int ActiveVehicles { get; set; }
    public decimal OverallProgress { get; set; }
    public SectorAggregationTripStatusesResource TripStatuses { get; set; } = new();

    public static SectorAggregationItemResource FromDto(SectorAggregationItemDto dto) => new()
    {
        Id = dto.Id,
        NameEn = dto.NameEn,
        NameAr = dto.NameAr,
        DoneTrips = dto.DoneTrips,
        TotalVehicles = dto.TotalVehicles,
        ActiveVehicles = dto.ActiveVehicles,
        OverallProgress = dto.OverallProgress,
        TripStatuses = SectorAggregationTripStatusesResource.FromDto(dto.TripStatuses)
    };
}

public class SectorAggregationTripStatusesResource
{
    public int InProgress { get; set; }
    public int Scheduled { get; set; }
    public int Pending { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }

    public static SectorAggregationTripStatusesResource FromDto(SectorAggregationTripStatusesDto dto) => new()
    {
        InProgress = dto.InProgress,
        Scheduled = dto.Scheduled,
        Pending = dto.Pending,
        Completed = dto.Completed,
        Cancelled = dto.Cancelled
    };
}
