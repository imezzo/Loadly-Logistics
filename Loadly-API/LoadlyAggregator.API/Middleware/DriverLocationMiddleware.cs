using System.Globalization;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.API.Middleware;

public class DriverLocationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DriverLocationMiddleware> _logger;
    private const string LatitudeHeader = "X-Latitude";
    private const string LongitudeHeader = "X-Longitude";
    private const string AccuracyHeader = "X-Accuracy";
    private const string HeadingHeader = "X-Heading";
    private const string SpeedHeader = "X-Speed";
    private const string TimestampHeader = "X-Timestamp";
    public const string DriverLocationKey = "DriverLocation";

    public DriverLocationMiddleware(RequestDelegate next, ILogger<DriverLocationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IDriverLocationRepository driverLocationRepository)
    {
        // Only process authenticated driver endpoints (skip auth endpoints like login/register)
        if (context.Request.Path.StartsWithSegments("/api/driver") &&
            !context.Request.Path.StartsWithSegments("/api/driver/auth") &&
            context.User.Identity?.IsAuthenticated == true)
        {
            await ProcessDriverLocationAsync(context, driverLocationRepository);
        }

        await _next(context);
    }

    private async Task ProcessDriverLocationAsync(HttpContext context, IDriverLocationRepository driverLocationRepository)
    {
        try
        {
            var driverId = ClaimsHelper.GetUserIdOptional(context.User);
            if (!driverId.HasValue)
            {
                return;
            }

            // Extract required headers
            var latitudeHeader = context.Request.Headers[LatitudeHeader].FirstOrDefault();
            var longitudeHeader = context.Request.Headers[LongitudeHeader].FirstOrDefault();

            if (string.IsNullOrEmpty(latitudeHeader) || string.IsNullOrEmpty(longitudeHeader))
            {
                return;
            }

            if (!decimal.TryParse(latitudeHeader, out var latitude) ||
                !decimal.TryParse(longitudeHeader, out var longitude))
            {
                _logger.LogWarning("Invalid latitude/longitude values for driver {DriverId}", driverId.Value);
                return;
            }

            // Extract optional headers
            decimal? accuracy = ExtractOptionalDecimal(context.Request.Headers[AccuracyHeader].FirstOrDefault());
            decimal? heading = ExtractOptionalDecimal(context.Request.Headers[HeadingHeader].FirstOrDefault());
            decimal? speed = ExtractOptionalDecimal(context.Request.Headers[SpeedHeader].FirstOrDefault());
            DateTime? timestamp = ExtractOptionalDateTime(context.Request.Headers[TimestampHeader].FirstOrDefault());
            var recordedAt = timestamp ?? AppUtcTime.UtcNow(TimeProvider.System);

            // Store location in HttpContext.Items for actions to access
            var locationData = new DriverLocationData
            {
                Latitude = latitude,
                Longitude = longitude,
                Accuracy = accuracy,
                Heading = heading,
                Speed = speed,
                RecordedAt = recordedAt
            };

            context.Items[DriverLocationKey] = locationData;

            await driverLocationRepository.UpsertAsync(new Domain.Common.Entities.DriverLocation
            {
                DriverId = driverId.Value,
                Latitude = latitude,
                Longitude = longitude,
                Accuracy = accuracy,
                Heading = heading,
                Speed = speed,
                RecordedAt = recordedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error processing driver location headers");
            // Don't throw - continue with the request
        }
    }

    private static decimal? ExtractOptionalDecimal(string? headerValue)
    {
        if (string.IsNullOrEmpty(headerValue))
            return null;

        return decimal.TryParse(headerValue, out var value) ? value : null;
    }

    private static DateTime? ExtractOptionalDateTime(string? headerValue)
    {
        if (string.IsNullOrEmpty(headerValue))
            return null;

        return DateTime.TryParse(
            headerValue,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out var value)
            ? value
            : null;
    }
}
