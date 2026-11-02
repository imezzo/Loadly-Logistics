using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Company.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Requests.Company.Trips;

public class CompanyCreateTripRequest
{
    public const string Permission = "create_trip";

    public long ContractId { get; set; }
    public long? DestinationId { get; set; }
    public long? VehicleTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal? ActualCargoWeightTons { get; set; }
    public decimal? CargoPrice { get; set; }
    public decimal? SpotPrice { get; set; }
    public decimal? ContractDistanceKm { get; set; }
    public int? HelpersCount { get; set; }
    public bool SubmitRequest { get; set; }
    public string ContactPersonName { get; set; } = string.Empty;
    public string ContactPersonMobile { get; set; } = string.Empty;
    public List<CompanyCreateTripWaypointRequest> Waypoints { get; set; } = new();

    public CompanyCreateTripDto FromRequest(long companyId) => new()
    {
        CompanyId = companyId,
        ContractId = ContractId,
        DestinationId = DestinationId,
        VehicleTypeId = VehicleTypeId,
        StartDate = StartDate,
        EndDate = EndDate,
        ActualCargoWeightTons = ActualCargoWeightTons,
        CargoPrice = CargoPrice,
        SpotPrice = SpotPrice,
        ContractDistanceKm = ContractDistanceKm,
        HelpersCount = HelpersCount,
        SubmitRequest = SubmitRequest,
        ContactPersonName = ContactPersonName,
        ContactPersonMobile = ContactPersonMobile,
        Waypoints = (Waypoints ?? new List<CompanyCreateTripWaypointRequest>())
            .Select(w => new CompanyCreateTripWaypointDto
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

public class CompanyCreateTripWaypointRequest
{
    public int Sequence { get; set; }
    public TripWaypointStopType StopType { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? AddressName { get; set; }
    public string? GooglePlaceId { get; set; }
    public string? TaskDetails { get; set; }
}

public class CompanyCreateTripRequestValidator : AbstractValidator<CompanyCreateTripRequest>
{
    public CompanyCreateTripRequestValidator()
    {
        RuleFor(x => x.ContractId)
            .IsRequiredId("Contract ID");

        RuleFor(x => x.VehicleTypeId)
            .IsOptionalId("Vehicle type ID");

        RuleFor(x => x.DestinationId)
            .IsOptionalId("Destination ID");

        RuleFor(x => x.StartDate)
            .IsRequiredDate("Start date")
            .IsFutureDate("Start date");

        RuleFor(x => x.EndDate)
            .IsRequiredDate("End date")
            .IsAfterDate(x => x.StartDate, "End date", "Start date");

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

        When(x => x.SpotPrice.HasValue, () =>
        {
            RuleFor(x => x.SpotPrice)
                .GreaterThan(0).WithMessage("trips.spot_price_invalid");
        });

        When(x => x.HelpersCount.HasValue, () =>
        {
            RuleFor(x => x.HelpersCount)
                .GreaterThanOrEqualTo(1).WithMessage(TripMessageKeys.HelpersCountMustBeAtLeastOne)
                .LessThanOrEqualTo(10).WithMessage(TripMessageKeys.HelpersCountMustNotExceed10);
        });

        RuleFor(x => x.Waypoints)
            .NotEmpty().WithMessage("validation.at_least_one_waypoint_required");

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

        RuleFor(x => x.ContactPersonName)
            .NotEmpty().WithMessage("validation.contact_person_name_required")
            .MaximumLength(255).WithMessage("validation.must_not_exceed_characters|255");

        RuleFor(x => x.ContactPersonMobile)
            .IsRequiredPhone(20);
    }
}
