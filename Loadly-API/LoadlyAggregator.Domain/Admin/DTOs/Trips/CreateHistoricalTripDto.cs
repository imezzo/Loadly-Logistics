using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class CreateHistoricalTripDto
{
    public long? ContractId { get; set; }
    public bool IsExternalTrip { get; set; }
    public long? LeadId { get; set; }
    public long? DestinationId { get; set; }
    public long? ReturnDestinationId { get; set; }
    public TripType Type { get; set; }
    public long? VehicleTypeId { get; set; }
    public long VehicleId { get; set; }
    public long? TailVehicleId { get; set; }
    public long DriverId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateOnly? FinancialDate { get; set; }
    public decimal? ContractDistanceKm { get; set; }
    public decimal? ActualCargoWeightTons { get; set; }
    public decimal? CargoPrice { get; set; }
    public decimal? SpotPrice { get; set; }
    public decimal? ContractorPrice { get; set; }
    public int? HelpersCount { get; set; }
    public string? ContactPersonName { get; set; }
    public string? ContactPersonMobile { get; set; }
    public string? DispatcherNotes { get; set; }
    public bool HasCustoms { get; set; }

    public List<TripWaypointDto> Waypoints { get; set; } = new();
}
