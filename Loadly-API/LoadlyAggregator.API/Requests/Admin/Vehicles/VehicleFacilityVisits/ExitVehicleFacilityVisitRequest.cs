using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilityVisits;

namespace LoadlyAggregator.API.Requests.Admin.Vehicles.VehicleFacilityVisits;

public class ExitVehicleFacilityVisitRequest
{
    public const string Permission = "exit_vehicle_facility_visit";

    public long VehicleFacilityId { get; set; }
    public DateTime EventDate { get; set; }
    public long DriverId { get; set; }
    public long SectorId { get; set; }
    public string? Reason { get; set; }
    public bool? IncludeHeadOrTail { get; set; }

    public ExitVehicleFacilityVisitDto FromRequest(long vehicleId) => new()
    {
        VehicleId = vehicleId,
        VehicleFacilityId = VehicleFacilityId,
        EventDate = EventDate,
        DriverId = DriverId,
        SectorId = SectorId,
        Reason = Reason,
        IncludeHeadOrTail = IncludeHeadOrTail ?? true
    };
}

public class ExitVehicleFacilityVisitRequestValidator : AbstractValidator<ExitVehicleFacilityVisitRequest>
{
    public ExitVehicleFacilityVisitRequestValidator()
    {
        RuleFor(x => x.VehicleFacilityId).IsRequiredId("Vehicle facility");
        RuleFor(x => x.EventDate).NotEmpty();
        RuleFor(x => x.DriverId).IsRequiredId("Driver");
        RuleFor(x => x.SectorId).IsRequiredId("Sector");
    }
}
