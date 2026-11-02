using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilityVisits;

namespace LoadlyAggregator.API.Requests.Admin.Vehicles.VehicleFacilityVisits;

public class EnterVehicleParkingFacilityRequest
{
    public const string Permission = "enter_vehicle_facility_visit";

    public long VehicleFacilityId { get; set; }
    public DateTime EventDate { get; set; }
    public string? Reason { get; set; }

    public EnterVehicleParkingFacilityDto FromRequest(long vehicleId) => new()
    {
        VehicleId = vehicleId,
        VehicleFacilityId = VehicleFacilityId,
        EventDate = EventDate,
        Reason = Reason
    };
}

public class EnterVehicleParkingFacilityRequestValidator : AbstractValidator<EnterVehicleParkingFacilityRequest>
{
    public EnterVehicleParkingFacilityRequestValidator()
    {
        RuleFor(x => x.VehicleFacilityId).IsRequiredId("Vehicle facility");
        RuleFor(x => x.EventDate).NotEmpty();
    }
}
