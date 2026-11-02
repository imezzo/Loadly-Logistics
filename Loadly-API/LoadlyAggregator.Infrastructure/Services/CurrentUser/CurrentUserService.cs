using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.Infrastructure.Services.CurrentUser;




public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public long? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }
    public string? GetCurrentUserType()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("userType")?.Value;
    }
    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
