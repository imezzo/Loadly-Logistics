namespace LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilityVisits;

public class EnterVehicleFacilityVisitDto
{
    public long VehicleId { get; set; }
    public long VehicleFacilityId { get; set; }
    public DateTime EventDate { get; set; }
    public string? Reason { get; set; }
    public bool? IncludeHeadOrTail { get; set; }
    public bool ClearSector { get; set; } = false;
    public bool ClearContract { get; set; } = false;
}

public class ExitVehicleFacilityVisitDto
{
    public long VehicleId { get; set; }
    public long VehicleFacilityId { get; set; }
    public DateTime EventDate { get; set; }
    public long DriverId { get; set; }
    public long SectorId { get; set; }
    public string? Reason { get; set; }
    public bool? IncludeHeadOrTail { get; set; }
}
