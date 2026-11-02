using System.Security.Claims;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.API.Helpers;




public static class ClaimsHelper
{
    public static long GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedException("auth.user_not_authenticated");
        return long.TryParse(userIdClaim.Value, out var userId)
            ? userId
            : throw new UnauthorizedException("auth.user_not_authenticated");
    }

    public static long GetCompanyId(ClaimsPrincipal user)
    {
        var companyIdClaim = user.FindFirst("companyId")
            ?? throw new UnauthorizedException("auth.user_not_authenticated");

        return long.TryParse(companyIdClaim.Value, out var companyId)
            ? companyId
            : throw new UnauthorizedException("auth.user_not_authenticated");
    }

    public static long? GetUserIdOptional(ClaimsPrincipal? user)
    {
        if (user == null)
            return null;
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return null;
        return long.TryParse(userIdClaim.Value, out var userId) ? userId : null;
    }



    public static string? GetUserTypeOptional(ClaimsPrincipal? user)
    {
        if (user == null)
            return null;
        return user.FindFirst("userType")?.Value;
    }
}
