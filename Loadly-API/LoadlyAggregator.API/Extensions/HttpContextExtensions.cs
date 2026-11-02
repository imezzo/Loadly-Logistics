using LoadlyAggregator.API.Middleware;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.API.Extensions;

public static class HttpContextExtensions
{
    public static DriverLocationData? GetDriverLocation(this HttpContext context)
    {
        return context.Items[DriverLocationMiddleware.DriverLocationKey] as DriverLocationData;
    }

    public static string? GetCorrelationId(this HttpContext context) =>
        context.Items.TryGetValue(CorrelationIdConstants.HttpContextItemKey, out var value)
            ? value as string
            : null;
}
