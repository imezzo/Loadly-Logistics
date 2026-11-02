using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.VehicleBrands;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Requests.Admin.Settings.VehicleBrands;

public class DeleteVehicleBrandRequest
{
    public const string Permission = "delete_vehicle_brand";
    public long Id { get; set; }
    public DeleteVehicleBrandDto FromRequest() => new() { Id = Id };
}

public class DeleteVehicleBrandRequestValidator : AbstractValidator<DeleteVehicleBrandRequest>
{
    public DeleteVehicleBrandRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle brand ID");
    }
}
