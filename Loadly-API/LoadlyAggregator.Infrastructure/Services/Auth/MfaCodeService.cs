using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Services.Auth;





public class MfaCodeService : IMfaCodeService
{
    private readonly IConfiguration _configuration;
    private readonly TimeProvider _timeProvider;
    private readonly string _secretKey;
    private const int TimeWindowMinutes = 10;

    public MfaCodeService(IConfiguration configuration, TimeProvider timeProvider)
    {
        _configuration = configuration;
        _timeProvider = timeProvider;
        _secretKey = configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey not configured");
    }

    private DateTime NowUtc => _timeProvider.GetUtcNow().UtcDateTime;

    public string GenerateCode(long userId, string userType)
    {

        var timeWindow = NowUtc;
        var minutes = timeWindow.Minute / TimeWindowMinutes * TimeWindowMinutes;
        var windowTime = new DateTime(timeWindow.Year, timeWindow.Month, timeWindow.Day, timeWindow.Hour, minutes, 0, DateTimeKind.Utc);


        var input = $"{userId}:{userType}:{windowTime:yyyy-MM-dd HH:mm}";


        var keyBytes = Encoding.UTF8.GetBytes(_secretKey);
        var inputBytes = Encoding.UTF8.GetBytes(input);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(inputBytes);


        var code = (BitConverter.ToUInt32(hashBytes, 0) % 1000000).ToString("D6");

        return code;
    }

    public bool VerifyCode(long userId, string userType, string code)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length != 6)
            return false;


        var currentCode = GenerateCode(userId, userType);


        var previousCode = GenerateCodeForPreviousWindow(userId, userType);

        return currentCode.Equals(code, StringComparison.OrdinalIgnoreCase) ||
               previousCode.Equals(code, StringComparison.OrdinalIgnoreCase);
    }

    private string GenerateCodeForPreviousWindow(long userId, string userType)
    {

        var timeWindow = NowUtc.AddMinutes(-TimeWindowMinutes);
        var minutes = timeWindow.Minute / TimeWindowMinutes * TimeWindowMinutes;
        var windowTime = new DateTime(timeWindow.Year, timeWindow.Month, timeWindow.Day, timeWindow.Hour, minutes, 0, DateTimeKind.Utc);


        var input = $"{userId}:{userType}:{windowTime:yyyy-MM-dd HH:mm}";


        var keyBytes = Encoding.UTF8.GetBytes(_secretKey);
        var inputBytes = Encoding.UTF8.GetBytes(input);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(inputBytes);


        var code = (BitConverter.ToUInt32(hashBytes, 0) % 1000000).ToString("D6");

        return code;
    }
}

