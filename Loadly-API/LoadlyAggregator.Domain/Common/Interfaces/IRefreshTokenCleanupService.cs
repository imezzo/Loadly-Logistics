namespace LoadlyAggregator.Domain.Common.Interfaces;




public interface IRefreshTokenCleanupService
{





    Task<RefreshTokenCleanupResult> PerformCleanupAsync(CancellationToken cancellationToken = default);
}



public class RefreshTokenCleanupResult
{
    public int ExpiredTokensDeleted { get; set; }
    public int RevokedTokensDeleted { get; set; }
    public int TotalDeleted => ExpiredTokensDeleted + RevokedTokensDeleted;
    public long DurationMs { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
