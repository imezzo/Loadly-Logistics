using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Vehicles;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Requests.Admin.Vehicles;

public class DeleteVehicleRequest
{
    public const string Permission = "delete_vehicle";

    public long Id { get; set; }

    public DeleteVehicleDto FromRequest() => new()
    {
        Id = Id
    };
}

public class DeleteVehicleRequestValidator : AbstractValidator<DeleteVehicleRequest>
{
    public DeleteVehicleRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle ID");
    }
}
