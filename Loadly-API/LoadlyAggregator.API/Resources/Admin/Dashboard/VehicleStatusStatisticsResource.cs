using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.API.Resources.Admin.Dashboard;

public class VehicleStatusStatisticsResource
{
    public int AvailableVehiclesCount { get; set; }
    public int InRouteVehiclesCount { get; set; }
    public int OutOfServiceVehiclesCount { get; set; }
    public int GarageVehiclesCount { get; set; }
    public int AgencyVehiclesCount { get; set; }
    public int WorkshopVehiclesCount { get; set; }

    public static VehicleStatusStatisticsResource FromDto(VehicleStatusStatisticsDto dto) => new()
    {
        AvailableVehiclesCount = dto.AvailableVehiclesCount,
        InRouteVehiclesCount = dto.InRouteVehiclesCount,
        OutOfServiceVehiclesCount = dto.OutOfServiceVehiclesCount,
        GarageVehiclesCount = dto.GarageVehiclesCount,
        AgencyVehiclesCount = dto.AgencyVehiclesCount,
        WorkshopVehiclesCount = dto.WorkshopVehiclesCount
    };
}
