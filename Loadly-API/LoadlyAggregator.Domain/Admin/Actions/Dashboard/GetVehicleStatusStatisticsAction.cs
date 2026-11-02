using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class GetVehicleStatusStatisticsAction(IVehicleRepository vehicleRepository)
{
    public async Task<VehicleStatusStatisticsDto> ExecuteAsync()
    {
        var vehicleStatusCounts = await vehicleRepository.GetVehicleStatusCountsAsync();
        var parkedFacilityTypeCounts = await vehicleRepository.GetParkedVehicleFacilityTypeCountsAsync();

        return new VehicleStatusStatisticsDto
        {
            AvailableVehiclesCount = vehicleStatusCounts.GetValueOrDefault(VehicleStatus.Available),
            InRouteVehiclesCount = vehicleStatusCounts.GetValueOrDefault(VehicleStatus.InRoute),
            OutOfServiceVehiclesCount = vehicleStatusCounts.GetValueOrDefault(VehicleStatus.OutOfService),
            GarageVehiclesCount = parkedFacilityTypeCounts.GetValueOrDefault(VehicleFacilityType.Garage),
            AgencyVehiclesCount = parkedFacilityTypeCounts.GetValueOrDefault(VehicleFacilityType.Agency),
            WorkshopVehiclesCount = parkedFacilityTypeCounts.GetValueOrDefault(VehicleFacilityType.Workshop)
        };
    }
}
