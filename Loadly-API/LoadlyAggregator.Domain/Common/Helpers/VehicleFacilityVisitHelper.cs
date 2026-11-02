using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class VehicleFacilityVisitHelper
{
    // Linked navigations don't load VehicleType, so also treat "referenced as a head by a group member" as head.
    public static Vehicle? ResolveHeadOrRigid(List<Vehicle> group) =>
        group.FirstOrDefault(v =>
            v.VehicleType?.VehicleUnitType is VehicleUnitType.Head or VehicleUnitType.RIGID
            || group.Any(x => x.HeadVehicleId == v.Id));

    public static Vehicle? GetCoupledVehicle(Vehicle vehicle) =>
        vehicle.HeadVehicleId.HasValue ? vehicle.HeadVehicle : vehicle.TailVehicles.FirstOrDefault();
}
