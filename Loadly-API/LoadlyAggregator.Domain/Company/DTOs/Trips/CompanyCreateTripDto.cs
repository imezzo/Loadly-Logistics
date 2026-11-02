namespace LoadlyAggregator.Domain.Company.DTOs.Trips;

public class CompanyCreateTripDto
{
    public long CompanyId { get; set; }
    public long ContractId { get; set; }
    public long? DestinationId { get; set; }
    public long? VehicleTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal? ActualCargoWeightTons { get; set; }
    public decimal? CargoPrice { get; set; }
    public decimal? SpotPrice { get; set; }
    public decimal? ContractDistanceKm { get; set; }
    public int? HelpersCount { get; set; }
    public bool SubmitRequest { get; set; }
    public string ContactPersonName { get; set; } = string.Empty;
    public string ContactPersonMobile { get; set; } = string.Empty;
    public List<CompanyCreateTripWaypointDto> Waypoints { get; set; } = new();
}
