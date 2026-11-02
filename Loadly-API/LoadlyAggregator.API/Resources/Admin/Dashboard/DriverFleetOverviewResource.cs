using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.API.Resources.Admin.Dashboard;

public class DriverFleetOverviewResource
{
    public int ActiveDriversCount { get; set; }
    public int FreeDriversCount { get; set; }
    public decimal DriverUtilizationRate { get; set; }
    public int FreeVehiclesCount { get; set; }
    public int InRouteVehiclesCount { get; set; }
    public int TotalVehiclesCount { get; set; }

    public static DriverFleetOverviewResource FromDto(DriverFleetOverviewDto dto) => new()
    {
        ActiveDriversCount = dto.ActiveDriversCount,
        FreeDriversCount = dto.FreeDriversCount,
        DriverUtilizationRate = dto.DriverUtilizationRate,
        FreeVehiclesCount = dto.FreeVehiclesCount,
        InRouteVehiclesCount = dto.InRouteVehiclesCount,
        TotalVehiclesCount = dto.TotalVehiclesCount
    };
}
