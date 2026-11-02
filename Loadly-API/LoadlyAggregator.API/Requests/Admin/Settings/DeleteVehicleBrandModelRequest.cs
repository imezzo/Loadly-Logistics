using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Requests.Admin.Settings;

public class DeleteVehicleBrandModelRequest
{
    public const string Permission = "delete_vehicle_brand_model";

    public long Id { get; set; }

    public DeleteVehicleBrandModelDto FromRequest() => new()
    {
        Id = Id
    };
}

public class DeleteVehicleBrandModelRequestValidator : AbstractValidator<DeleteVehicleBrandModelRequest>
{
    public DeleteVehicleBrandModelRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle brand model ID");
    }
}
