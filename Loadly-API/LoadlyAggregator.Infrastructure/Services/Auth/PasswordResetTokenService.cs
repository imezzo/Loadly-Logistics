using System.Globalization;
using LoadlyAggregator.Shared.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Services.Auth;





public class PasswordResetTokenService : Domain.Common.Interfaces.IPasswordResetTokenService
{
    private readonly IConfiguration _configuration;
    private readonly TimeProvider _timeProvider;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _passwordResetHours;

    public PasswordResetTokenService(IConfiguration configuration, TimeProvider timeProvider)
    {
        _configuration = configuration;
        _timeProvider = timeProvider;
        _secretKey = configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey not configured");
        _issuer = configuration["JwtSettings:Issuer"] ?? "LoadlyAggregator";
        _audience = configuration["JwtSettings:Audience"] ?? "LoadlyAggregator";
        _passwordResetHours = int.Parse(configuration["AuthSettings:PasswordResetHours"] ?? "2");
    }

    private DateTime NowUtc => _timeProvider.GetUtcNow().UtcDateTime;

    public string GenerateResetToken(
        long userId,
        string userType,
        CommunicationChannel channel = CommunicationChannel.Email,
        int expirationHours = 0)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


        var expiration = expirationHours > 0 ? expirationHours : _passwordResetHours;

        var channelString = channel switch
        {
            CommunicationChannel.Email => "email",
            CommunicationChannel.Sms => "sms",
            _ => "email"
        };

        var claims = new List<Claim>
        {
            new Claim("userId", userId.ToString()),
            new Claim("userType", userType),
            new Claim("channel", channelString),
            new Claim("issuedAt", NowUtc.ToString("o")),
            new Claim("purpose", "password_reset")
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: NowUtc.AddHours(expiration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public PasswordResetTokenData? ValidateResetToken(string token)
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


            var purpose = principal.FindFirst("purpose")?.Value;
            if (purpose != "password_reset")
            {
                return null;
            }

            var userIdClaim = principal.FindFirst("userId")?.Value;
            var userTypeClaim = principal.FindFirst("userType")?.Value;
            var channelClaim = principal.FindFirst("channel")?.Value;
            var issuedAtClaim = principal.FindFirst("issuedAt")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userTypeClaim))
            {
                return null;
            }

            if (!long.TryParse(userIdClaim, out var userId))
            {
                return null;
            }


            var channel = ParseChannel(channelClaim);


            var issuedAt = NowUtc;
            if (!string.IsNullOrEmpty(issuedAtClaim)
                && DateTime.TryParse(issuedAtClaim, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
                issuedAt = parsed.ToUniversalTime();

            return new PasswordResetTokenData
            {
                UserId = userId,
                UserType = userTypeClaim,
                Channel = channel,
                IssuedAt = issuedAt
            };
        }
        catch
        {
            return null;
        }
    }

    private static CommunicationChannel ParseChannel(string? channel)
    {
        if (string.IsNullOrWhiteSpace(channel))
            return CommunicationChannel.Email;
        return channel.ToLower() switch
        {
            "email" => CommunicationChannel.Email,
            "sms" => CommunicationChannel.Sms,
            _ => CommunicationChannel.Email
        };
    }
}

