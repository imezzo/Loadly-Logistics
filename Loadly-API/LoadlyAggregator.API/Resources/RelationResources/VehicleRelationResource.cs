using LoadlyAggregator.Domain.Common.DTOs.Relations;

namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Vehicle reference (plate, driver, tail vehicles). Used in Contract and Trip.</summary>
public class VehicleRelationResource
{
    public long Id { get; set; }
    public long VehicleTypeId { get; set; }
    public string PlateNo { get; set; } = string.Empty;
    public string PlateLetters { get; set; } = string.Empty;
    public string? ChassisNumber { get; set; }
    public string? BrandName { get; set; }
    public string? ModelName { get; set; }
    public int? Year { get; set; }
    public long? SectorId { get; set; }
    public VehicleTypeRelationResource? VehicleType { get; set; }
    public DriverRelationResource? Driver { get; set; }
    public SectorRelationResource? Sector { get; set; }
    public List<CoupledVehicleRelationResource> TailVehicles { get; set; } = new();

    public static VehicleRelationResource? FromDto(VehicleRelationDto? dto) =>
        dto == null ? null : new VehicleRelationResource
        {
            Id = dto.Id,
            VehicleTypeId = dto.VehicleTypeId,
            PlateNo = dto.PlateNo,
            PlateLetters = dto.PlateLetters,
            ChassisNumber = dto.ChassisNumber,
            BrandName = dto.BrandName,
            ModelName = dto.ModelName,
            Year = dto.Year,
            SectorId = dto.SectorId,
            VehicleType = VehicleTypeRelationResource.FromDto(dto.VehicleType),
            Sector = dto.Sector == null ? null : new SectorRelationResource
            {
                Id = dto.Sector.Id,
                Name = dto.Sector.NameEn,
                NameEn = dto.Sector.NameEn,
                NameAr = dto.Sector.NameAr
            },
            TailVehicles = dto.TailVehicles
                .Select(CoupledVehicleRelationResource.FromDto)
                .Where(t => t != null)
                .Cast<CoupledVehicleRelationResource>()
                .ToList()
        };
}
