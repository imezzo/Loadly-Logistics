using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.VehicleBrands;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Requests.Admin.Settings.VehicleBrands;

public class GetVehicleBrandRequest
{
    public const string Permission = "view_vehicle_brand";
    public long Id { get; set; }
    public GetVehicleBrandDto FromRequest() => new() { Id = Id };
}

public class GetVehicleBrandRequestValidator : AbstractValidator<GetVehicleBrandRequest>
{
    public GetVehicleBrandRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle brand ID");
    }
}
