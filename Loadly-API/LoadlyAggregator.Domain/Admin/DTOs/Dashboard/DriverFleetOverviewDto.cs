namespace LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

public class DriverFleetOverviewDto
{
    public int ActiveDriversCount { get; set; }
    public int FreeDriversCount { get; set; }
    public decimal DriverUtilizationRate { get; set; }
    public int FreeVehiclesCount { get; set; }
    public int InRouteVehiclesCount { get; set; }
    public int TotalVehiclesCount { get; set; }
}
