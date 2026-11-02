namespace LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

public class VehicleStatusStatisticsDto
{
    public int AvailableVehiclesCount { get; set; }
    public int InRouteVehiclesCount { get; set; }
    public int OutOfServiceVehiclesCount { get; set; }
    public int GarageVehiclesCount { get; set; }
    public int AgencyVehiclesCount { get; set; }
    public int WorkshopVehiclesCount { get; set; }
}
