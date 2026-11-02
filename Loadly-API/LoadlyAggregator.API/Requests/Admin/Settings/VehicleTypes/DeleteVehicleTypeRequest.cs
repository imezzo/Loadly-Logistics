using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.VehicleTypes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Requests.Admin.Settings.VehicleTypes;

public class DeleteVehicleTypeRequest
{
    public const string Permission = "delete_vehicle_type";
    public long Id { get; set; }
    public DeleteVehicleTypeDto FromRequest() => new() { Id = Id };
}

public class DeleteVehicleTypeRequestValidator : AbstractValidator<DeleteVehicleTypeRequest>
{
    public DeleteVehicleTypeRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle type ID");
    }
}
