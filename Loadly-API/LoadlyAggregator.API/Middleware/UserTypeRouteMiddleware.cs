using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.API.Middleware;

public class UserTypeRouteMiddleware
{
    private static readonly (string Prefix, string UserType)[] AreaMap =
    [
        ("/api/admin", "admin"),
        ("/api/driver", "driver"),
        ("/api/company", "company"),
        ("/api/collector", "collector"),
        ("/api/contractor", "contractor"),
    ];

    private readonly RequestDelegate _next;

    public UserTypeRouteMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true
            && TryGetRequiredUserType(context.Request.Path, out var requiredUserType))
        {
            var tokenUserType = ClaimsHelper.GetUserTypeOptional(context.User);
            if (!string.Equals(tokenUserType, requiredUserType, StringComparison.OrdinalIgnoreCase))
            {
                throw new ForbiddenException("auth.invalid_user_type");
            }
        }

        await _next(context);
    }

    internal static bool TryGetRequiredUserType(PathString path, out string userType)
    {
        var value = path.Value ?? string.Empty;
        foreach (var (prefix, type) in AreaMap)
        {
            if (value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                && (value.Length == prefix.Length || value[prefix.Length] == '/'))
            {
                userType = type;
                return true;
            }
        }

        userType = string.Empty;
        return false;
    }
}
