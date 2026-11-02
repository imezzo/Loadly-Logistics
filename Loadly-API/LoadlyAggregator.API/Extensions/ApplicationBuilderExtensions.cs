using LoadlyAggregator.API.Middleware;
namespace LoadlyAggregator.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
    public static IApplicationBuilder UseLanguageMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LanguageMiddleware>();
    }
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }
    public static IApplicationBuilder UseDriverLocation(this IApplicationBuilder app)
    {
        return app.UseMiddleware<DriverLocationMiddleware>();
    }

    public static IApplicationBuilder UseUserTypeRoute(this IApplicationBuilder app)
    {
        return app.UseMiddleware<UserTypeRouteMiddleware>();
    }
}
