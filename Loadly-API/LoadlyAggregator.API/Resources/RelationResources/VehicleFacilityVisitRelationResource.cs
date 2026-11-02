using LoadlyAggregator.Domain.Common.DTOs.Relations;

namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Vehicle facility visit reference (e.g. on facility get/list).</summary>
public class VehicleFacilityVisitRelationResource
{
    public long Id { get; set; }
    public int Event { get; set; }
    public string EventName { get; set; } = string.Empty;
    public int Context { get; set; }
    public string ContextName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    public VehicleFacilityRelationResource? VehicleFacility { get; set; }
    public VehicleRelationResource? Vehicle { get; set; }
    public DriverRelationResource? Driver { get; set; }
    public AdminRelationResource? AddedBy { get; set; }

    public static VehicleFacilityVisitRelationResource FromDto(VehicleFacilityVisitRelationDto dto) =>
        new()
        {
            Id = dto.Id,
            Event = (int)dto.Event,
            EventName = dto.EventName,
            Context = (int)dto.Context,
            ContextName = dto.ContextName,
            EventDate = dto.EventDate,
            Reason = dto.Reason,
            CreatedAt = dto.CreatedAt,
            VehicleFacility = dto.VehicleFacility == null ? null : new VehicleFacilityRelationResource
            {
                Id = dto.VehicleFacility.Id,
                Name = dto.VehicleFacility.Name,
                Address = dto.VehicleFacility.Address,
                GoogleMapsUrl = dto.VehicleFacility.GoogleMapsUrl,
                Lat = dto.VehicleFacility.Lat,
                Lng = dto.VehicleFacility.Lng,
                FacilityType = dto.VehicleFacility.FacilityType,
                CompanyId = dto.VehicleFacility.CompanyId,
                Company = dto.VehicleFacility.Company == null ? null : new CompanyRelationResource
                {
                    Id = dto.VehicleFacility.Company.Id,
                    NameEn = dto.VehicleFacility.Company.NameEn,
                    NameAr = dto.VehicleFacility.Company.NameAr,
                    Phone = dto.VehicleFacility.Company.Phone,
                    Email = dto.VehicleFacility.Company.Email,
                    RegistrationNumber = dto.VehicleFacility.Company.RegistrationNumber,
                    FullName = dto.VehicleFacility.Company.FullName
                }
            },
            Vehicle = dto.Vehicle == null ? null : new VehicleRelationResource
            {
                Id = dto.Vehicle.Id,
                VehicleTypeId = dto.Vehicle.VehicleTypeId,
                PlateNo = dto.Vehicle.PlateNo,
                PlateLetters = dto.Vehicle.PlateLetters,
                ChassisNumber = dto.Vehicle.ChassisNumber,
                BrandName = dto.Vehicle.BrandName,
                ModelName = dto.Vehicle.ModelName,
                Year = dto.Vehicle.Year,
                SectorId = dto.Vehicle.SectorId
            },
            Driver = dto.Driver == null ? null : new DriverRelationResource
            {
                Id = dto.Driver.Id,
                FullName = dto.Driver.FullName,
                FullNameAr = dto.Driver.FullNameAr,
                FullNameEn = dto.Driver.FullNameEn,
                Phone = dto.Driver.Phone,
                Email = dto.Driver.Email,
                NationalId = dto.Driver.NationalId
            },
            AddedBy = dto.AddedBy == null ? null : new AdminRelationResource
            {
                Id = dto.AddedBy.Id,
                FullName = dto.AddedBy.FullName,
                UserName = dto.AddedBy.UserName,
                Email = dto.AddedBy.Email
            }
        };
}
