using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Vehicles;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Requests.Admin.Vehicles;

public class GetVehicleRequest
{
    public const string Permission = "view_vehicle";

    public long Id { get; set; }

    public GetVehicleDto FromRequest() => new()
    {
        Id = Id
    };
}

public class GetVehicleRequestValidator : AbstractValidator<GetVehicleRequest>
{
    public GetVehicleRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle ID");
    }
}
