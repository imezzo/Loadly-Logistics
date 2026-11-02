using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilities;

namespace LoadlyAggregator.API.Requests.Admin.Settings.VehicleFacilities;

public class DeleteVehicleFacilityRequest
{
    public const string Permission = "delete_vehicle_facility";

    public long Id { get; set; }

    public DeleteVehicleFacilityDto FromRequest() => new() { Id = Id };
}

public class DeleteVehicleFacilityRequestValidator : AbstractValidator<DeleteVehicleFacilityRequest>
{
    public DeleteVehicleFacilityRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle facility ID");
    }
}
