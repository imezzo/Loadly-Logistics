using LoadlyAggregator.Domain.Driver.DTOs.Trips;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class TripRouteSyncPayloadValidator
{
    public const int CoordinateScale = 100_000;

    public static void Validate(TripRouteSyncPayloadDto route)
    {
        if (route.PointCount <= 0)
        {
            throw new ValidationException("validation.must_be_greater_than_zero");
        }

        if (route.DeltaTimeSeconds.Count != route.PointCount
            || route.LatitudeScaled.Count != route.PointCount
            || route.LongitudeScaled.Count != route.PointCount
            || route.Speed.Count != route.PointCount
            || route.HeadingDegrees.Count != route.PointCount
            || route.AccuracyCentimeters.Count != route.PointCount
            || route.DistanceSinceLastMeters.Count != route.PointCount)
        {
            throw new ValidationException("validation.route_payload_array_length_mismatch");
        }

        if (route.DeltaTimeSeconds.Any(d => d < 0) || route.DistanceSinceLastMeters.Any(d => d < 0))
        {
            throw new ValidationException("validation.must_be_greater_than_or_equal_to_zero");
        }

        var lat = route.LatitudeScaled[0];
        var lng = route.LongitudeScaled[0];
        ValidateCoordinate(lat, lng);

        var speed = route.Speed[0];
        if (speed < 0)
        {
            throw new ValidationException("validation.must_be_greater_than_or_equal_to_zero");
        }

        if (route.HeadingDegrees[0] is < 0 or > 360)
        {
            throw new ValidationException("validation.must_be_between|0|360");
        }

        var accuracy = route.AccuracyCentimeters[0];
        if (accuracy < 0)
        {
            throw new ValidationException("validation.must_be_greater_than_or_equal_to_zero");
        }

        for (var i = 1; i < route.PointCount; i++)
        {
            lat += route.LatitudeScaled[i];
            lng += route.LongitudeScaled[i];
            ValidateCoordinate(lat, lng);

            speed += route.Speed[i];
            if (speed < 0)
            {
                throw new ValidationException("validation.must_be_greater_than_or_equal_to_zero");
            }

            accuracy += route.AccuracyCentimeters[i];
            if (accuracy < 0)
            {
                throw new ValidationException("validation.must_be_greater_than_or_equal_to_zero");
            }
        }
    }

    private static void ValidateCoordinate(int latScaled, int lngScaled)
    {
        var latitude = latScaled / (decimal)CoordinateScale;
        var longitude = lngScaled / (decimal)CoordinateScale;

        if (latitude is < -90 or > 90)
        {
            throw new ValidationException("validation.latitude_between");
        }

        if (longitude is < -180 or > 180)
        {
            throw new ValidationException("validation.longitude_between");
        }
    }
}
