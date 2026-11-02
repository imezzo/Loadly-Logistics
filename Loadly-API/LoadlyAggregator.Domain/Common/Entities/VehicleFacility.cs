using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class VehicleFacility : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? GoogleMapsUrl { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public long AddedBy { get; set; }
    public long? CompanyId { get; set; }
    public VehicleFacilityStatus Status { get; set; } = VehicleFacilityStatus.ACTIVE;
    public VehicleFacilityType FacilityType { get; set; }
    public virtual Company? Company { get; set; }
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public virtual ICollection<VehicleFacilityVisit> VehicleFacilityVisits { get; set; } = new List<VehicleFacilityVisit>();
}
