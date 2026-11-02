using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
    Task<RefreshToken?> GetByModelIdAndModelTypeAsync(long modelId, string modelType);
    Task<List<RefreshToken>> GetByTokenFamilyAsync(string tokenFamilyId);
    Task<List<RefreshToken>> GetActiveSessionsByModelAsync(long modelId, string modelType);
    Task UpdateLastUsedAsync(long id);
    Task RevokeTokenAsync(string tokenHash);
    Task RevokeModelTokensAsync(long modelId, string modelType);
    Task RevokeModelTokensBulkAsync(List<long> modelIds, string modelType);
    Task RevokeTokenFamilyAsync(string tokenFamilyId);
    Task DeleteExpiredTokensAsync();
    Task<int> DeleteExpiredTokensAsync(int olderThanDays, int batchSize, CancellationToken cancellationToken = default);
    Task<int> DeleteOldRevokedTokensAsync(int olderThanDays, int batchSize, CancellationToken cancellationToken = default);
}
