using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
namespace LoadlyAggregator.Domain.Common.Interfaces;




public interface IPasswordResetCacheService
{



    Task StoreResetAttemptAsync(
        string userType,
        string identifier,
        long userId,
        string code,
        CommunicationChannel channel,
        int expirationMinutes = 60);



    Task<PasswordResetAttempt?> GetResetAttemptAsync(string userType, string identifier);



    Task DeleteResetAttemptAsync(string userType, string identifier);



    Task IncrementFailedAttemptsAsync(string userType, string identifier);



    Task<bool> IsRateLimitedAsync(string userType, string identifier);
}



public class PasswordResetAttempt
{
    public long UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public CommunicationChannel Channel { get; set; }
    public DateTime CreatedAt { get; set; }
    public int FailedAttempts { get; set; }
}
