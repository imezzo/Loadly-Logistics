using Serilog.Context;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Middleware;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrGenerateCorrelationId(context);

        context.Items[CorrelationIdConstants.HttpContextItemKey] = correlationId;
        context.Response.Headers[CorrelationIdConstants.HeaderName] = correlationId;

        using (LogContext.PushProperty(CorrelationIdConstants.SerilogPropertyName, correlationId))
        using (LogContext.PushProperty("RequestPath", context.Request.Path))
        using (LogContext.PushProperty("RequestMethod", context.Request.Method))
        using (LogContext.PushProperty("RequestQueryString", context.Request.QueryString.ToString()))
        using (LogContext.PushProperty("UserAgent", context.Request.Headers.UserAgent.ToString()))
        using (LogContext.PushProperty("RemoteIpAddress", context.Connection.RemoteIpAddress?.ToString() ?? "Unknown"))
        {
            await _next(context);
        }
    }
    private static string GetOrGenerateCorrelationId(HttpContext context)
    {

        if (context.Request.Headers.TryGetValue(CorrelationIdConstants.HeaderName, out var correlationIdHeader))
        {
            var correlationId = correlationIdHeader.ToString();
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                return correlationId;
            }
        }

        return context.TraceIdentifier;
    }
}
