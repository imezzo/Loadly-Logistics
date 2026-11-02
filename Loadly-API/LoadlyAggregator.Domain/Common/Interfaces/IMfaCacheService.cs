using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
namespace LoadlyAggregator.Domain.Common.Interfaces;




public interface IMfaCacheService
{



    int MfaCodeExpirationMinutes { get; }



    Task StoreMfaCodeAsync(
        long userId,
        string userType,
        string code,
        CommunicationChannel channel,
        int expirationMinutes = 30);



    Task<MfaCodeAttempt?> GetMfaCodeAsync(long userId, string userType);



    Task DeleteMfaCodeAsync(long userId, string userType);



    Task IncrementFailedAttemptsAsync(long userId, string userType);



    Task<bool> IsRateLimitedAsync(long userId, string userType);



    Task StoreLoginOtpTokenAsync(
        long userId,
        string userType,
        string token,
        int expirationMinutes = 30);



    Task<string?> GetLoginOtpTokenAsync(long userId, string userType);



    Task DeleteLoginOtpTokenAsync(long userId, string userType);
}



public class MfaCodeAttempt
{
    public long UserId { get; set; }
    public string UserType { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public CommunicationChannel Channel { get; set; }
    public DateTime CreatedAt { get; set; }
    public int FailedAttempts { get; set; }
}
