using System.Text.Json.Serialization;
using FluentValidation;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Driver.Trips;

public class DriverSyncRouteRequest
{
    public decimal DistanceMeters { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int GpsPacketCount { get; set; }
    public TripRouteSyncPayloadData Route { get; set; } = null!;

    public DriverSyncRouteDto FromRequest(long tripId, long driverId) => new()
    {
        TripId = tripId,
        DriverId = driverId,
        DistanceMeters = DistanceMeters,
        StartTime = StartTime!.Value,
        EndTime = EndTime!.Value,
        GpsPacketCount = GpsPacketCount,
        Route = Route.ToDto()
    };
}

public class TripRouteSyncPayloadData
{
    [JsonPropertyName("n")]
    public int N { get; set; }

    [JsonPropertyName("t0")]
    public long T0 { get; set; }

    [JsonPropertyName("dt")]
    public List<int> Dt { get; set; } = [];

    [JsonPropertyName("lat")]
    public List<int> Lat { get; set; } = [];

    [JsonPropertyName("lng")]
    public List<int> Lng { get; set; } = [];

    [JsonPropertyName("spd")]
    public List<int> Spd { get; set; } = [];

    [JsonPropertyName("hdg")]
    public List<int> Hdg { get; set; } = [];

    [JsonPropertyName("acc")]
    public List<int> Acc { get; set; } = [];

    [JsonPropertyName("dsm")]
    public List<int> Dsm { get; set; } = [];

    public TripRouteSyncPayloadDto ToDto() => new()
    {
        PointCount = N,
        EpochSeconds = T0,
        DeltaTimeSeconds = Dt,
        LatitudeScaled = Lat,
        LongitudeScaled = Lng,
        Speed = Spd,
        HeadingDegrees = Hdg,
        AccuracyCentimeters = Acc,
        DistanceSinceLastMeters = Dsm
    };
}

public class DriverSyncRouteRequestValidator : AbstractValidator<DriverSyncRouteRequest>
{
    public DriverSyncRouteRequestValidator()
    {
        RuleFor(x => x.DistanceMeters)
            .GreaterThanOrEqualTo(0).WithMessage("validation.must_be_greater_than_or_equal_to_zero");

        RuleFor(x => x.StartTime)
            .NotNull().WithMessage("validation.field_is_required|Start time");

        RuleFor(x => x.EndTime)
            .NotNull().WithMessage("validation.field_is_required|End time")
            .GreaterThanOrEqualTo(x => x.StartTime).WithMessage("validation.field_must_be_after_or_equal|End time|Start time")
            .When(x => x.StartTime.HasValue);

        RuleFor(x => x.GpsPacketCount)
            .GreaterThanOrEqualTo(0).WithMessage("validation.must_be_greater_than_or_equal_to_zero");

        RuleFor(x => x.Route)
            .NotNull().WithMessage("validation.field_is_required|Route");

        RuleFor(x => x.Route)
            .SetValidator(new TripRouteSyncPayloadDataValidator()!)
            .When(x => x.Route is not null);
    }
}

public class TripRouteSyncPayloadDataValidator : AbstractValidator<TripRouteSyncPayloadData>
{
    public TripRouteSyncPayloadDataValidator()
    {
        RuleFor(x => x.N)
            .GreaterThan(0).WithMessage("validation.must_be_greater_than_zero");

        RuleFor(x => x.Dt)
            .NotEmpty().WithMessage("validation.field_is_required|dt");

        RuleFor(x => x)
            .Must(p => p.Dt.Count == p.N
                && p.Lat.Count == p.N
                && p.Lng.Count == p.N
                && p.Spd.Count == p.N
                && p.Hdg.Count == p.N
                && p.Acc.Count == p.N
                && p.Dsm.Count == p.N)
            .WithMessage("validation.route_payload_array_length_mismatch");

        RuleForEach(x => x.Dt)
            .GreaterThanOrEqualTo(0).WithMessage("validation.must_be_greater_than_or_equal_to_zero");

        RuleForEach(x => x.Dsm)
            .GreaterThanOrEqualTo(0).WithMessage("validation.must_be_greater_than_or_equal_to_zero");
    }
}
