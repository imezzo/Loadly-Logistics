using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Resources.Admin.Vehicles;

public class VehicleAssignmentHistoryResource
{
    public long Id { get; set; }
    public long VehicleId { get; set; }
    public int Source { get; set; }
    public long? PrevDriverId { get; set; }
    public long? NewDriverId { get; set; }
    public long? PrevTailVehicleId { get; set; }
    public long? NewTailVehicleId { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DriverRelationResource? PrevDriver { get; set; }
    public DriverRelationResource? NewDriver { get; set; }
    public VehicleRelationResource? PrevTailVehicle { get; set; }
    public VehicleRelationResource? NewTailVehicle { get; set; }
    public AdminRelationResource? AddedByAdmin { get; set; }

    public static VehicleAssignmentHistoryResource FromDto(VehicleAssignmentHistoryDto dto) =>
        new()
        {
            Id = dto.Id,
            VehicleId = dto.VehicleId,
            Source = (int)dto.Source,
            PrevDriverId = dto.PrevDriverId,
            NewDriverId = dto.NewDriverId,
            PrevTailVehicleId = dto.PrevTailVehicleId,
            NewTailVehicleId = dto.NewTailVehicleId,
            AddedBy = dto.AddedBy,
            CreatedAt = dto.CreatedAt,
            PrevDriver = MapDriver(dto.PrevDriver),
            NewDriver = MapDriver(dto.NewDriver),
            PrevTailVehicle = MapVehicle(dto.PrevTailVehicle),
            NewTailVehicle = MapVehicle(dto.NewTailVehicle),
            AddedByAdmin = dto.AddedByAdmin == null ? null : new AdminRelationResource
            {
                Id = dto.AddedByAdmin.Id,
                FullName = dto.AddedByAdmin.FullName,
                UserName = dto.AddedByAdmin.UserName,
                Email = dto.AddedByAdmin.Email
            }
        };

    private static DriverRelationResource? MapDriver(Domain.Common.DTOs.Relations.DriverRelationDto? driver) =>
        driver == null ? null : new DriverRelationResource
        {
            Id = driver.Id,
            FullName = driver.FullName,
            FullNameAr = driver.FullNameAr,
            FullNameEn = driver.FullNameEn,
            Phone = driver.Phone,
            Email = driver.Email,
            NationalId = driver.NationalId
        };

    private static VehicleRelationResource? MapVehicle(Domain.Common.DTOs.Relations.VehicleRelationDto? vehicle) =>
        vehicle == null ? null : new VehicleRelationResource
        {
            Id = vehicle.Id,
            VehicleTypeId = vehicle.VehicleTypeId,
            PlateNo = vehicle.PlateNo,
            PlateLetters = vehicle.PlateLetters,
            ChassisNumber = vehicle.ChassisNumber,
            BrandName = vehicle.BrandName,
            ModelName = vehicle.ModelName,
            Year = vehicle.Year,
            SectorId = vehicle.SectorId
        };
}
