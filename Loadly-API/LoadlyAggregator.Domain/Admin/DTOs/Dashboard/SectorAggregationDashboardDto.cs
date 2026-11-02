namespace LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

public class SectorAggregationDashboardDto
{
    public SectorAggregationTotalTripsDto TotalTrips { get; set; } = new();
    public IReadOnlyList<SectorAggregationItemDto> Sectors { get; set; } = Array.Empty<SectorAggregationItemDto>();
}

public class SectorAggregationTotalTripsDto
{
    public int Total { get; set; }
    public PeriodComparisonDto VsYesterday { get; set; } = new();
}

public class SectorAggregationItemDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public int DoneTrips { get; set; }
    public int TotalVehicles { get; set; }
    public int ActiveVehicles { get; set; }
    public decimal OverallProgress { get; set; }
    public SectorAggregationTripStatusesDto TripStatuses { get; set; } = new();
}

public class SectorAggregationTripStatusesDto
{
    public int InProgress { get; set; }
    public int Scheduled { get; set; }
    public int Pending { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
}
