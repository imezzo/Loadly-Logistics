using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class CreateHistoricalTripRequest
{
    public const string Permission = "create_historical_trip";

    public long? ContractId { get; set; }
    public bool IsExternalTrip { get; set; }
    public long? LeadId { get; set; }
    public long? DestinationId { get; set; }
    public long? ReturnDestinationId { get; set; }
    public TripType Type { get; set; }
    public long? VehicleTypeId { get; set; }
    public long VehicleId { get; set; }
    public long? TailVehicleId { get; set; }
    public long DriverId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateOnly? FinancialDate { get; set; }
    public decimal? ContractDistanceKm { get; set; }
    public decimal? ActualCargoWeightTons { get; set; }
    public decimal? CargoPrice { get; set; }
    public decimal? SpotPrice { get; set; }
    public decimal? ContractorPrice { get; set; }
    public int? HelpersCount { get; set; }
    public string? ContactPersonName { get; set; }
    public string? ContactPersonMobile { get; set; }
    public string? DispatcherNotes { get; set; }
    public bool HasCustoms { get; set; } = false;
    public List<WaypointItemRequest>? Waypoints { get; set; }

    public CreateHistoricalTripDto FromRequest() => new()
    {
        ContractId = ContractId,
        IsExternalTrip = IsExternalTrip,
        LeadId = LeadId,
        DestinationId = DestinationId,
        ReturnDestinationId = ReturnDestinationId,
        Type = Type,
        VehicleTypeId = VehicleTypeId,
        VehicleId = VehicleId,
        TailVehicleId = TailVehicleId,
        DriverId = DriverId,
        StartDate = StartDate,
        EndDate = EndDate,
        FinancialDate = FinancialDate,
        ContractDistanceKm = ContractDistanceKm,
        ActualCargoWeightTons = ActualCargoWeightTons,
        CargoPrice = CargoPrice,
        SpotPrice = SpotPrice,
        ContractorPrice = ContractorPrice,
        HelpersCount = HelpersCount,
        ContactPersonName = ContactPersonName,
        ContactPersonMobile = ContactPersonMobile,
        DispatcherNotes = DispatcherNotes,
        HasCustoms = HasCustoms,
        Waypoints = (Waypoints ?? new List<WaypointItemRequest>())
            .Select(w => new TripWaypointDto
            {
                Sequence = w.Sequence,
                StopType = w.StopType,
                Latitude = w.Latitude,
                Longitude = w.Longitude,
                AddressName = w.AddressName,
                GooglePlaceId = w.GooglePlaceId,
                TaskDetails = w.TaskDetails
            }).ToList()
    };
}

public class CreateHistoricalTripRequestValidator : AbstractValidator<CreateHistoricalTripRequest>
{
    public CreateHistoricalTripRequestValidator()
    {
        When(x => x.IsExternalTrip, () =>
        {
            RuleFor(x => x.SpotPrice)
                .NotNull().WithMessage(TripMessageKeys.SpotPriceRequiredForExternalTrip)
                .GreaterThan(0).WithMessage("trips.spot_price_invalid");
        });

        RuleFor(x => x.VehicleId)
            .IsRequiredId("Vehicle ID");

        RuleFor(x => x.DriverId)
            .IsRequiredId("Driver ID");

        RuleFor(x => x.TailVehicleId)
            .IsOptionalId("Tail vehicle ID");

        RuleFor(x => x.VehicleTypeId)
            .IsOptionalId("Vehicle type ID");

        RuleFor(x => x.DestinationId)
            .IsOptionalId("Destination ID");

        RuleFor(x => x.ReturnDestinationId)
            .IsOptionalId("Return destination ID");

        When(x => x.Type == TripType.OpenJaw && x.DestinationId is > 0, () =>
        {
            RuleFor(x => x.ReturnDestinationId)
                .NotNull().WithMessage(TripMessageKeys.OpenJawReturnDestinationRequired)
                .GreaterThan(0).WithMessage(TripMessageKeys.OpenJawReturnDestinationRequired);
        });

        When(x => x.Type != TripType.OpenJaw, () =>
        {
            RuleFor(x => x.ReturnDestinationId)
                .Null().WithMessage(TripMessageKeys.ReturnDestinationNotAllowedForTripType);
        });

        RuleFor(x => x.Type)
            .IsRequiredDefinedEnum(TripMessageKeys.TripTypeRequired, TripMessageKeys.TripTypeInvalid);

        When(x => x.Type == TripType.RoundTrip, () =>
        {
            RuleFor(x => x.Waypoints)
                .NotNull().WithMessage(TripMessageKeys.RoundTripWaypointsRequired)
                .Must(w => w!.Count > 0).WithMessage(TripMessageKeys.RoundTripWaypointsRequired);
        });

        When(x => x.Type == TripType.OpenJaw, () =>
        {
            RuleFor(x => x.Waypoints)
                .NotNull().WithMessage(TripMessageKeys.OpenJawWaypointsRequired)
                .Must(w => w!.Count > 0).WithMessage(TripMessageKeys.OpenJawWaypointsRequired);
        });

        When(x => x.ActualCargoWeightTons.HasValue, () =>
        {
            RuleFor(x => x.ActualCargoWeightTons)
                .GreaterThan(0).WithMessage("trips.actual_cargo_weight_tons_invalid");
        });

        When(x => x.CargoPrice.HasValue, () =>
        {
            RuleFor(x => x.CargoPrice)
                .GreaterThan(0).WithMessage("trips.cargo_price_invalid");
        });

        When(x => !x.IsExternalTrip && x.SpotPrice.HasValue, () =>
        {
            RuleFor(x => x.SpotPrice)
                .GreaterThan(0).WithMessage("trips.spot_price_invalid");
        });

        When(x => x.ContractorPrice.HasValue, () =>
        {
            RuleFor(x => x.ContractorPrice)
                .GreaterThan(0).WithMessage("trips.contractor_price_invalid");
        });

        When(x => x.HelpersCount.HasValue, () =>
        {
            RuleFor(x => x.HelpersCount)
                .GreaterThanOrEqualTo(1).WithMessage(TripMessageKeys.HelpersCountMustBeAtLeastOne)
                .LessThanOrEqualTo(10).WithMessage(TripMessageKeys.HelpersCountMustNotExceed10);
        });

        RuleFor(x => x.StartDate)
            .IsRequiredDate("Start date")
            .IsPastDateTime("Start date");

        RuleFor(x => x.EndDate)
            .IsRequiredDate("End date")
            .IsPastDateTime("End date");

        RuleForEach(x => x.Waypoints)
            .ChildRules(waypoint =>
            {
                waypoint.RuleFor(w => w.Sequence)
                    .GreaterThanOrEqualTo(0).WithMessage("validation.waypoint_sequence_gte_zero");
                waypoint.RuleFor(w => w.StopType)
                    .IsInEnum().WithMessage("validation.waypoint_stop_type_pickup_or_dropoff");
                waypoint.RuleFor(w => w.Latitude)
                    .InclusiveBetween(-90m, 90m).WithMessage("validation.latitude_between");
                waypoint.RuleFor(w => w.Longitude)
                    .InclusiveBetween(-180m, 180m).WithMessage("validation.longitude_between");
                waypoint.When(w => !string.IsNullOrWhiteSpace(w.AddressName!), () =>
                    waypoint.RuleFor(w => w.AddressName).MaximumLength(255).WithMessage("validation.must_not_exceed_characters|255"));
                waypoint.When(w => !string.IsNullOrWhiteSpace(w.GooglePlaceId!), () =>
                    waypoint.RuleFor(w => w.GooglePlaceId).MaximumLength(255).WithMessage("validation.must_not_exceed_characters|255"));
            })
            .When(x => x.Waypoints != null && x.Waypoints.Count > 0);

        When(x => !string.IsNullOrWhiteSpace(x.ContactPersonName), () =>
            RuleFor(x => x.ContactPersonName).MaximumLength(255).WithMessage("validation.must_not_exceed_characters|255"));

        When(x => !string.IsNullOrWhiteSpace(x.ContactPersonMobile), () =>
            RuleFor(x => x.ContactPersonMobile).IsOptionalPhone(20));
    }
}
