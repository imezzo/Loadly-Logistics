using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Requests.Admin.Settings;

public class GetVehicleBrandModelRequest
{
    public const string Permission = "view_vehicle_brand_model";

    public long Id { get; set; }

    public GetVehicleBrandModelDto FromRequest() => new()
    {
        Id = Id
    };
}

public class GetVehicleBrandModelRequestValidator : AbstractValidator<GetVehicleBrandModelRequest>
{
    public GetVehicleBrandModelRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle brand model ID");
    }
}
