using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Addresses;

namespace LoadlyAggregator.API.Resources.Admin;

/// <summary>
/// Resource for Address API responses (both listing and single get)
/// </summary>
public class AddressResource
{
    public long Id { get; set; }
    public string ModelType { get; set; } = string.Empty;
    public long ModelId { get; set; }
    public string StreetName { get; set; } = string.Empty;
    public string? BuildingNumber { get; set; }
    public string? BuildingType { get; set; }
    public long? GovernerateId { get; set; }
    public long? CityId { get; set; }
    public long? DistrictId { get; set; }
    public string? PostalCode { get; set; }
    public int Status { get; set; }
    public GovernerateRelationResource? Governerate { get; set; }
    public CityRelationResource? City { get; set; }
    public DistrictRelationResource? District { get; set; }

    public static AddressResource FromDto(
        AddressDto dto,
        GovernerateDto? governerate = null,
        CityDto? city = null,
        DistrictDto? district = null) =>
        new AddressResource
        {
            Id = dto.Id,
            ModelType = dto.ModelType,
            ModelId = dto.ModelId,
            StreetName = dto.StreetName,
            BuildingNumber = dto.BuildingNumber,
            BuildingType = dto.BuildingType,
            GovernerateId = dto.GovernerateId,
            CityId = dto.CityId,
            DistrictId = dto.DistrictId,
            PostalCode = dto.PostalCode,
            Status = dto.Status,
            Governerate = governerate != null ? new GovernerateRelationResource
            {
                Id = governerate.Id,
                Name = governerate.NameEn,
                NameAr = governerate.NameAr,
                NameEn = governerate.NameEn
            } : null,
            City = city != null ? new CityRelationResource
            {
                Id = city.Id,
                Name = city.NameEn,
                NameAr = city.NameAr,
                NameEn = city.NameEn
            } : null,
            District = district != null ? new DistrictRelationResource
            {
                Id = district.Id,
                Name = district.NameEn,
                NameAr = district.NameAr,
                NameEn = district.NameEn
            } : null
        };
}
