using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Vehicles.VehicleFacilityVisits;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Requests.Admin.Vehicles;

public class SetVehicleAssignedParkingFacilityRequest
{
    public const string Permission = EnterVehicleFacilityVisitRequest.Permission;

    public long? AssignedParkingFacilityId { get; set; }

    public SetVehicleAssignedParkingFacilityDto FromRequest(long vehicleId) => new()
    {
        VehicleId = vehicleId,
        AssignedParkingFacilityId = AssignedParkingFacilityId
    };
}

public class SetVehicleAssignedParkingFacilityRequestValidator : AbstractValidator<SetVehicleAssignedParkingFacilityRequest>
{
    public SetVehicleAssignedParkingFacilityRequestValidator()
    {
        RuleFor(x => x.AssignedParkingFacilityId)
            .IsOptionalId("Assigned parking facility ID");
    }
}
