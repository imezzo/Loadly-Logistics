using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilities;

public class CreateVehicleFacilityDto
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? GoogleMapsUrl { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public long? CompanyId { get; set; }
    public VehicleFacilityStatus Status { get; set; } = VehicleFacilityStatus.ACTIVE;
    public VehicleFacilityType FacilityType { get; set; }
    public List<ContactDataDto> Contacts { get; set; } = new();
}
