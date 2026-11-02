using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Authentication;





public class JwtBearerEventsHandler
{



    public static JwtBearerEvents CreateEvents()
    {
        return new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var jwtTokenService = context.HttpContext.RequestServices
                    .GetRequiredService<IJwtTokenService>();


                var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                if (!string.IsNullOrWhiteSpace(jti))
                {

                    var isRevoked = await jwtTokenService.IsRevokedAsync(jti);
                    if (isRevoked)
                    {
                        context.Fail("Token has been revoked");
                        return;
                    }


                    var tokenFamilyId = context.Principal?.FindFirst("tokenFamilyId")?.Value;
                    if (!string.IsNullOrWhiteSpace(tokenFamilyId))
                    {
                        var isFamilyRevoked = await jwtTokenService.IsTokenFamilyRevokedAsync(tokenFamilyId);
                        if (isFamilyRevoked)
                        {
                            context.Fail("Token family has been revoked");
                            return;
                        }
                    }


                    var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var userTypeClaim = context.Principal?.FindFirst("userType")?.Value;

                    if (!string.IsNullOrWhiteSpace(userIdClaim)
                        && !string.IsNullOrWhiteSpace(userTypeClaim)
                        && long.TryParse(userIdClaim, out var userId))
                    {
                        var allRevoked = await jwtTokenService.AreAllUserTokensRevokedAsync(userId, userTypeClaim);
                        if (allRevoked)
                        {
                            context.Fail("All user tokens have been revoked");
                            return;
                        }
                    }
                }
            }
        };
    }
}

