using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Requests.Admin.Vehicles;

public class AssignVehicleDriverRequest
{
    public const string Permission = "assign_driver";

    public long? DriverId { get; set; }

    public AssignVehicleDriverDto FromRequest(long vehicleId) => new()
    {
        VehicleId = vehicleId,
        DriverId = DriverId
    };
}

public class AssignVehicleDriverRequestValidator : AbstractValidator<AssignVehicleDriverRequest>
{
    public AssignVehicleDriverRequestValidator()
    {
        RuleFor(x => x.DriverId)
            .IsOptionalId("Driver ID")
            .When(x => x.DriverId.HasValue);
    }
}
