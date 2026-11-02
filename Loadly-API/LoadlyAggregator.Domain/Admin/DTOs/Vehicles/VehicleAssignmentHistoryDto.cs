using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class VehicleAssignmentHistoryDto
{
    public long Id { get; set; }
    public long VehicleId { get; set; }
    public VehicleAssignmentHistorySource Source { get; set; }
    public long? PrevDriverId { get; set; }
    public long? NewDriverId { get; set; }
    public long? PrevTailVehicleId { get; set; }
    public long? NewTailVehicleId { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DriverRelationDto? PrevDriver { get; set; }
    public DriverRelationDto? NewDriver { get; set; }
    public VehicleRelationDto? PrevTailVehicle { get; set; }
    public VehicleRelationDto? NewTailVehicle { get; set; }
    public AdminRelationDto? AddedByAdmin { get; set; }

    public static VehicleAssignmentHistoryDto FromEntity(VehicleAssignmentHistory history) =>
        new()
        {
            Id = history.Id,
            VehicleId = history.VehicleId,
            Source = history.Source,
            PrevDriverId = history.PrevDriverId,
            NewDriverId = history.NewDriverId,
            PrevTailVehicleId = history.PrevTailVehicleId,
            NewTailVehicleId = history.NewTailVehicleId,
            AddedBy = history.AddedBy,
            CreatedAt = history.CreatedAt,
            PrevDriver = history.PrevDriver == null ? null : MapDriver(history.PrevDriver),
            NewDriver = history.NewDriver == null ? null : MapDriver(history.NewDriver),
            PrevTailVehicle = history.PrevTailVehicle == null ? null : MapVehicle(history.PrevTailVehicle),
            NewTailVehicle = history.NewTailVehicle == null ? null : MapVehicle(history.NewTailVehicle),
            AddedByAdmin = history.AddedByAdmin == null ? null : new AdminRelationDto
            {
                Id = history.AddedByAdmin.Id,
                FullName = history.AddedByAdmin.FullName,
                UserName = history.AddedByAdmin.UserName,
                Email = history.AddedByAdmin.Email
            }
        };

    private static DriverRelationDto MapDriver(Common.Entities.Driver driver) =>
        new()
        {
            Id = driver.Id,
            FullName = driver.FullName,
            FullNameAr = driver.FullNameAr,
            Phone = driver.Phone,
            Email = driver.Email,
            NationalId = driver.NationalId
        };

    private static VehicleRelationDto MapVehicle(Common.Entities.Vehicle vehicle) =>
        new()
        {
            Id = vehicle.Id,
            VehicleTypeId = vehicle.VehicleTypeId,
            PlateNo = vehicle.PlateNo,
            PlateLetters = vehicle.PlateLetters,
            ChassisNumber = vehicle.VinNo,
            Year = vehicle.MakeYear,
            SectorId = vehicle.SectorId
        };
}
