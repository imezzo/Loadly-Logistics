using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilityVisits;

namespace LoadlyAggregator.API.Requests.Admin.Vehicles.VehicleFacilityVisits;

public class EnterVehicleFacilityVisitRequest
{
    public const string Permission = "enter_vehicle_facility_visit";

    public long VehicleFacilityId { get; set; }
    public DateTime EventDate { get; set; }
    public string? Reason { get; set; }
    public bool? IncludeHeadOrTail { get; set; }
    public bool ClearSector { get; set; } = false;
    public bool ClearContract { get; set; } = false;

    public EnterVehicleFacilityVisitDto FromRequest(long vehicleId) => new()
    {
        VehicleId = vehicleId,
        VehicleFacilityId = VehicleFacilityId,
        EventDate = EventDate,
        Reason = Reason,
        IncludeHeadOrTail = IncludeHeadOrTail ?? true,
        ClearSector = ClearSector,
        ClearContract = ClearContract
    };
}

public class EnterVehicleFacilityVisitRequestValidator : AbstractValidator<EnterVehicleFacilityVisitRequest>
{
    public EnterVehicleFacilityVisitRequestValidator()
    {
        RuleFor(x => x.VehicleFacilityId).IsRequiredId("Vehicle facility");
        RuleFor(x => x.EventDate).NotEmpty();
    }
}
