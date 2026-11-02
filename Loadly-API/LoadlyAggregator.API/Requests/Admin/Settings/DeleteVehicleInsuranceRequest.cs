using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Requests.Admin.Settings;

public class DeleteVehicleInsuranceRequest
{
    public const string Permission = "delete_vehicle_insurance";

    public long Id { get; set; }

    public DeleteVehicleInsuranceDto FromRequest() => new()
    {
        Id = Id
    };
}

public class DeleteVehicleInsuranceRequestValidator : AbstractValidator<DeleteVehicleInsuranceRequest>
{
    public DeleteVehicleInsuranceRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle insurance ID");
    }
}
