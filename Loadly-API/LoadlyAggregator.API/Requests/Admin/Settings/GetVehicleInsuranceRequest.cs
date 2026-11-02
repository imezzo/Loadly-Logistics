using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Requests.Admin.Settings;

public class GetVehicleInsuranceRequest
{
    public const string Permission = "view_vehicle_insurance";

    public long Id { get; set; }

    public GetVehicleInsuranceDto FromRequest() => new()
    {
        Id = Id
    };
}

public class GetVehicleInsuranceRequestValidator : AbstractValidator<GetVehicleInsuranceRequest>
{
    public GetVehicleInsuranceRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle insurance ID");
    }
}
