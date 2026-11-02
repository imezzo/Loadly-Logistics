using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace LoadlyAggregator.Infrastructure.Services.Auth;



public class ClaimsBuilder
{
    private readonly List<Claim> _claims = new();
    public ClaimsBuilder AddUserClaims(long userId, string email, string userType, long? companyId = null)
    {
        _claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
        _claims.Add(new Claim(ClaimTypes.Email, email));
        _claims.Add(new Claim("userType", userType));
        _claims.Add(new Claim("companyId", companyId?.ToString() ?? string.Empty));
        _claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        return this;
    }

    public ClaimsBuilder AddTokenFamilyId(string tokenFamilyId)
    {
        if (!string.IsNullOrWhiteSpace(tokenFamilyId))
        {
            _claims.Add(new Claim("tokenFamilyId", tokenFamilyId));
        }
        return this;
    }
    public ClaimsBuilder AddRoles(IEnumerable<string>? roles)
    {
        if (roles != null)
        {
            foreach (var role in roles)
            {
                _claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }
        return this;
    }
    public ClaimsBuilder AddPermissions(IEnumerable<string>? permissions)
    {
        if (permissions != null)
        {
            foreach (var permission in permissions)
            {
                _claims.Add(new Claim("permission", permission));
            }
        }
        return this;
    }
    public List<Claim> Build()
    {
        return _claims;
    }
}
