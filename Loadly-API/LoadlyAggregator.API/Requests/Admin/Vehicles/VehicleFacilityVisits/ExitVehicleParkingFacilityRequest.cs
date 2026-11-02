using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilityVisits;

namespace LoadlyAggregator.API.Requests.Admin.Vehicles.VehicleFacilityVisits;

public class ExitVehicleParkingFacilityRequest
{
    public const string Permission = "exit_vehicle_facility_visit";

    public long? VehicleFacilityId { get; set; }
    public DateTime EventDate { get; set; }
    public string? Reason { get; set; }

    public ExitVehicleParkingFacilityDto FromRequest(long vehicleId) => new()
    {
        VehicleId = vehicleId,
        VehicleFacilityId = VehicleFacilityId,
        EventDate = EventDate,
        Reason = Reason
    };
}

public class ExitVehicleParkingFacilityRequestValidator : AbstractValidator<ExitVehicleParkingFacilityRequest>
{
    public ExitVehicleParkingFacilityRequestValidator()
    {
        RuleFor(x => x.EventDate).NotEmpty();
        RuleFor(x => x.VehicleFacilityId).IsOptionalId("Vehicle facility");
    }
}
