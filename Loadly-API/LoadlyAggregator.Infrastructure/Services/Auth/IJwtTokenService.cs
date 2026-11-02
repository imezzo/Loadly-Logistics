using System.Security.Claims;
namespace LoadlyAggregator.Infrastructure.Services.Auth;

public interface IJwtTokenService : Domain.Common.Interfaces.IJwtTokenService
{
    new ClaimsPrincipal? ValidateToken(string token);
}
