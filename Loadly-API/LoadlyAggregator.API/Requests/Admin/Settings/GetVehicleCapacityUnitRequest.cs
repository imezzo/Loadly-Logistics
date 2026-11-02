using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Requests.Admin.Settings;

public class GetVehicleCapacityUnitRequest
{
    public const string Permission = "view_vehicle_capacity_unit";

    public long Id { get; set; }

    public GetVehicleCapacityUnitDto FromRequest() => new()
    {
        Id = Id
    };
}

public class GetVehicleCapacityUnitRequestValidator : AbstractValidator<GetVehicleCapacityUnitRequest>
{
    public GetVehicleCapacityUnitRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle capacity unit ID");
    }
}
