using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilities;

namespace LoadlyAggregator.API.Requests.Admin.Settings.VehicleFacilities;

public class GetVehicleFacilityRequest
{
    public const string Permission = "view_vehicle_facility";

    public long Id { get; set; }

    public GetVehicleFacilityDto FromRequest() => new() { Id = Id };
}

public class GetVehicleFacilityRequestValidator : AbstractValidator<GetVehicleFacilityRequest>
{
    public GetVehicleFacilityRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle facility ID");
    }
}
