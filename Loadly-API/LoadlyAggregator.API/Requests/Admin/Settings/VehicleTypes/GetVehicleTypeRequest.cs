using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.VehicleTypes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Requests.Admin.Settings.VehicleTypes;

public class GetVehicleTypeRequest
{
    public const string Permission = "view_vehicle_type";
    public long Id { get; set; }
    public GetVehicleTypeDto FromRequest() => new() { Id = Id };
}

public class GetVehicleTypeRequestValidator : AbstractValidator<GetVehicleTypeRequest>
{
    public GetVehicleTypeRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle type ID");
    }
}
