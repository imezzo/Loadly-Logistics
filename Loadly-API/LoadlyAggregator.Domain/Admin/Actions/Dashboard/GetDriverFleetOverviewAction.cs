using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class GetDriverFleetOverviewAction(
    IDriverRepository driverRepository,
    IVehicleRepository vehicleRepository)
{
    public async Task<DriverFleetOverviewDto> ExecuteAsync()
    {
        var (activeDriversCount, freeDriversCount) = await driverRepository.GetAvailableActiveDriversCountAsync();
        var vehicleStatusCounts = await vehicleRepository.GetVehicleStatusCountsAsync();

        return new()
        {
            ActiveDriversCount = activeDriversCount,
            FreeDriversCount = freeDriversCount,
            DriverUtilizationRate = CalculateUtilizationRate(activeDriversCount, freeDriversCount),
            FreeVehiclesCount = vehicleStatusCounts.GetValueOrDefault(VehicleStatus.Available),
            InRouteVehiclesCount = vehicleStatusCounts.GetValueOrDefault(VehicleStatus.InRoute),
            TotalVehiclesCount = vehicleStatusCounts.Values.Sum()
        };
    }

    private static decimal CalculateUtilizationRate(int activeDriversCount, int freeDriversCount) =>
        activeDriversCount == 0
            ? 0m
            : Math.Round((activeDriversCount - freeDriversCount) * 100m / activeDriversCount, 2, MidpointRounding.AwayFromZero);
}
