using LoadlyAggregator.Domain.Common.DTOs.Contacts;
using LoadlyAggregator.Domain.Common.DTOs.Relations;

namespace LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilities;

public class VehicleFacilityDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? GoogleMapsUrl { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public long AddedBy { get; set; }
    public long? CompanyId { get; set; }
    public CompanyRelationDto? Company { get; set; }
    public int Status { get; set; }
    public int FacilityType { get; set; }
    public List<ContactDto> Contacts { get; set; } = new();
    public List<VehicleFacilityVisitRelationDto> VehicleFacilityVisits { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
