using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Requests.Admin.Settings;

public class DeleteVehicleCapacityUnitRequest
{
    public const string Permission = "delete_vehicle_capacity_unit";

    public long Id { get; set; }

    public DeleteVehicleCapacityUnitDto FromRequest() => new()
    {
        Id = Id
    };
}

public class DeleteVehicleCapacityUnitRequestValidator : AbstractValidator<DeleteVehicleCapacityUnitRequest>
{
    public DeleteVehicleCapacityUnitRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle capacity unit ID");
    }
}
