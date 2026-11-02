using LoadlyAggregator.API.Resources.Admin;
using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilities;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

public class VehicleFacilityResource
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? GoogleMapsUrl { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public long AddedBy { get; set; }
    public long? CompanyId { get; set; }
    public CompanyRelationResource? Company { get; set; }
    public int Status { get; set; }
    public int FacilityType { get; set; }
    public List<ContactResource> Contacts { get; set; } = new();
    public List<VehicleFacilityVisitRelationResource> VehicleFacilityVisits { get; set; } = new();

    public static VehicleFacilityResource FromDto(VehicleFacilityDto dto) =>
        new VehicleFacilityResource
        {
            Id = dto.Id,
            Name = dto.Name,
            Address = dto.Address,
            GoogleMapsUrl = dto.GoogleMapsUrl,
            Lat = dto.Lat,
            Lng = dto.Lng,
            AddedBy = dto.AddedBy,
            CompanyId = dto.CompanyId,
            Company = dto.Company == null ? null : new CompanyRelationResource
            {
                Id = dto.Company.Id,
                NameEn = dto.Company.NameEn,
                NameAr = dto.Company.NameAr,
                Phone = dto.Company.Phone,
                Email = dto.Company.Email,
                RegistrationNumber = dto.Company.RegistrationNumber,
                FullName = dto.Company.FullName
            },
            Status = dto.Status,
            FacilityType = dto.FacilityType,
            Contacts = dto.Contacts.Select(c => ContactResource.FromDto(c)).ToList(),
            VehicleFacilityVisits = dto.VehicleFacilityVisits
                .Select(VehicleFacilityVisitRelationResource.FromDto)
                .ToList()
        };
}
