using LoadlyAggregator.Domain.Common.DTOs.Relations;

namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Vehicle in a head/tail coupling (head, tail, or rigid).</summary>
public class CoupledVehicleRelationResource
{
    public long Id { get; set; }
    public long VehicleTypeId { get; set; }
    public string PlateNo { get; set; } = string.Empty;
    public string PlateLetters { get; set; } = string.Empty;
    public string? ChassisNumber { get; set; }
    public int? Year { get; set; }
    public string? BrandName { get; set; }
    public string? ModelName { get; set; }
    public long? DriverId { get; set; }
    public int Status { get; set; }
    public VehicleTypeRelationResource? VehicleType { get; set; }

    public static CoupledVehicleRelationResource? FromDto(CoupledVehicleRelationDto? dto) =>
        dto == null ? null : new CoupledVehicleRelationResource
        {
            Id = dto.Id,
            VehicleTypeId = dto.VehicleTypeId,
            PlateNo = dto.PlateNo,
            PlateLetters = dto.PlateLetters,
            ChassisNumber = dto.ChassisNumber,
            Year = dto.Year,
            BrandName = dto.BrandName,
            ModelName = dto.ModelName,
            DriverId = dto.DriverId,
            Status = dto.Status,
            VehicleType = VehicleTypeRelationResource.FromDto(dto.VehicleType)
        };
}
