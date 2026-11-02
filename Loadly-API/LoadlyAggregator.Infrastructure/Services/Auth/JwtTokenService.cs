using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace LoadlyAggregator.Infrastructure.Services.Auth;





public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly IDatabase _redis;
    private readonly TimeProvider _timeProvider;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationInMinutes;
    private readonly int _mfaTokenMinutes;

    public JwtTokenService(
        IConfiguration configuration,
        IConnectionMultiplexer connectionMultiplexer,
        TimeProvider timeProvider)
    {
        _configuration = configuration;
        _redis = connectionMultiplexer.GetDatabase();
        _timeProvider = timeProvider;
        _secretKey = configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey not configured");
        _issuer = configuration["JwtSettings:Issuer"] ?? "LoadlyAggregator";
        _audience = configuration["JwtSettings:Audience"] ?? "LoadlyAggregator";
        _expirationInMinutes = int.Parse(configuration["JwtSettings:ExpirationInMinutes"] ?? "120");
        _mfaTokenMinutes = int.Parse(configuration["AuthSettings:MfaTokenMinutes"] ?? "60");
    }

    private DateTime NowUtc => _timeProvider.GetUtcNow().UtcDateTime;

    public string GenerateToken(long userId, string userType, string email, List<string>? roles = null, List<string>? permissions = null, string? tokenFamilyId = null, long? companyId = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


        var claimsBuilder = new ClaimsBuilder()
            .AddUserClaims(userId, email, userType, companyId)
            .AddRoles(roles)
            .AddPermissions(permissions);


        if (!string.IsNullOrWhiteSpace(tokenFamilyId))
        {
            claimsBuilder.AddTokenFamilyId(tokenFamilyId);
        }

        var claims = claimsBuilder.Build();

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: NowUtc.AddMinutes(_expirationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateTemporaryMfaToken(long userId, string userType, string email, int expiresInMinutes = 0)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


        var expirationMinutes = expiresInMinutes > 0 ? expiresInMinutes : _mfaTokenMinutes;


        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim("userType", userType),
            new Claim(ClaimTypes.Email, email),
            new Claim("scope", "mfa_only"),
            new Claim("mfa_pending", "true")
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: NowUtc.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    public string HashRefreshToken(string token)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashBytes);
    }

    public string GenerateTokenFamilyId()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }


    public async Task RevokeTokenAsync(string jti, int expirationMinutes = 120)
    {
        if (string.IsNullOrWhiteSpace(jti))
            return;

        var key = GetJtiKey(jti);


        await _redis.StringSetAsync(key, "revoked", TimeSpan.FromMinutes(expirationMinutes));
    }

    public async Task<bool> IsRevokedAsync(string jti)
    {
        if (string.IsNullOrWhiteSpace(jti))
            return false;

        var key = GetJtiKey(jti);
        var exists = await _redis.KeyExistsAsync(key);
        return exists;
    }

    public async Task RevokeAllUserTokensAsync(long userId, string userType, int expirationMinutes = 120)
    {
        var key = GetUserRevocationKey(userId, userType);

        var timestamp = NowUtc.ToString("O");
        await _redis.StringSetAsync(key, timestamp, TimeSpan.FromMinutes(expirationMinutes));
    }

    public async Task<bool> AreAllUserTokensRevokedAsync(long userId, string userType)
    {
        var key = GetUserRevocationKey(userId, userType);
        var exists = await _redis.KeyExistsAsync(key);
        return exists;
    }

    public async Task ClearUserRevocationAsync(long userId, string userType)
    {
        var key = GetUserRevocationKey(userId, userType);
        await _redis.KeyDeleteAsync(key);
    }
    public async Task ClearUserRevocationBulkAsync(IEnumerable<long> userIds, string userType)
    {
        if (userIds == null || !userIds.Any())
        {
            return;
        }


        var keys = userIds
            .Select(userId => (RedisKey)GetUserRevocationKey(userId, userType))
            .ToArray();


        await _redis.KeyDeleteAsync(keys);
    }
    public async Task RevokeTokenFamilyAsync(string tokenFamilyId, int expirationMinutes = 120)
    {
        if (string.IsNullOrWhiteSpace(tokenFamilyId))
            return;

        var key = GetTokenFamilyRevocationKey(tokenFamilyId);


        await _redis.StringSetAsync(key, "revoked", TimeSpan.FromMinutes(expirationMinutes));
    }

    public async Task<bool> IsTokenFamilyRevokedAsync(string tokenFamilyId)
    {
        if (string.IsNullOrWhiteSpace(tokenFamilyId))
            return false;

        var key = GetTokenFamilyRevocationKey(tokenFamilyId);
        var exists = await _redis.KeyExistsAsync(key);
        return exists;
    }

    private static string GetJtiKey(string jti)
    {
        return $"jwt:blacklist:jti:{jti}";
    }

    private static string GetUserRevocationKey(long userId, string userType)
    {
        return $"jwt:blacklist:user:{userType.ToLowerInvariant()}:{userId}";
    }

    private static string GetTokenFamilyRevocationKey(string tokenFamilyId)
    {
        return $"jwt:blacklist:family:{tokenFamilyId}";
    }
}

