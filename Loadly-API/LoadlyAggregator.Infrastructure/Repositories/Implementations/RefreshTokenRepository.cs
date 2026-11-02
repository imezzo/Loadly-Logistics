using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
    {
        return _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
    }

    public Task<RefreshToken?> GetByModelIdAndModelTypeAsync(long modelId, string modelType) =>
        _dbSet
            .Where(rt => rt.ModelId == modelId
                && rt.ModelType == modelType
                && rt.RevokedAt == null
                && rt.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(rt => rt.CreatedAt)
            .FirstOrDefaultAsync();

    public Task<List<RefreshToken>> GetByTokenFamilyAsync(string tokenFamilyId) =>
        _dbSet.Where(rt => rt.TokenFamilyId == tokenFamilyId).ToListAsync();

    public Task<List<RefreshToken>> GetActiveSessionsByModelAsync(long modelId, string modelType) =>
        _dbSet
            .Where(rt => rt.ModelId == modelId
                && rt.ModelType == modelType
                && rt.RevokedAt == null
                && rt.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync();
    public override Task<RefreshToken?> GetByIdAsync(long id) =>
        _dbSet.FirstOrDefaultAsync(rt => rt.Id == id);

    public override async Task AddAsync(RefreshToken entity)
    {
        await base.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public override async Task UpdateAsync(RefreshToken token)
    {
        _dbSet.Update(token);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateLastUsedAsync(long id) =>
        await (await GetByIdAsync(id) is { } token ? UpdateTokenLastUsed(token) : Task.CompletedTask);

    private async Task UpdateTokenLastUsed(RefreshToken token)
    {
        token.LastUsedAt = DateTime.UtcNow;
        await UpdateAsync(token);
    }

    public async Task RevokeTokenAsync(string tokenHash) =>
        await (await GetByTokenHashAsync(tokenHash) is { } token && token.RevokedAt == null ? RevokeTokenInternal(token) : Task.CompletedTask);

    private async Task RevokeTokenInternal(RefreshToken token)
    {
        token.RevokedAt = DateTime.UtcNow;
        await UpdateAsync(token);
    }
    public async Task RevokeModelTokensAsync(long modelId, string modelType)
    {
        var tokens = await _dbSet
            .Where(rt => rt.ModelId == modelId
                && rt.ModelType == modelType
                && rt.RevokedAt == null
                && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
        var now = DateTime.UtcNow;
        tokens.ForEach(t => t.RevokedAt = now);
        await (tokens.Count != 0 ? _context.SaveChangesAsync() : Task.CompletedTask);
    }
    public Task RevokeModelTokensBulkAsync(List<long> modelIds, string modelType) =>
        modelIds is not { Count: > 0 } ? Task.CompletedTask : RevokeTokensInternal(modelIds, modelType);

    private async Task RevokeTokensInternal(List<long> modelIds, string modelType)
    {
        var tokens = await _dbSet
            .Where(rt => modelIds.Contains(rt.ModelId)
                && rt.ModelType == modelType
                && rt.RevokedAt == null
                && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
        var now = DateTime.UtcNow;
        tokens.ForEach(t => t.RevokedAt = now);
        await (tokens.Count != 0 ? _context.SaveChangesAsync() : Task.CompletedTask);
    }
    public async Task RevokeTokenFamilyAsync(string tokenFamilyId)
    {
        var tokens = await GetByTokenFamilyAsync(tokenFamilyId);
        tokens.Where(t => t.RevokedAt == null).ToList().ForEach(t => t.RevokedAt = DateTime.UtcNow);
        await (tokens.Count != 0 ? _context.SaveChangesAsync() : Task.CompletedTask);
    }
    public async Task DeleteExpiredTokensAsync()
    {
        var expiredTokens = await _dbSet
            .Where(rt => rt.ExpiresAt < DateTime.UtcNow.AddDays(-7))
            .ToListAsync();
        await (expiredTokens.Count != 0 ? DeleteTokensInternal(expiredTokens) : Task.CompletedTask);
    }

    private async Task DeleteTokensInternal(List<RefreshToken> tokens)
    {
        _dbSet.RemoveRange(tokens);
        await _context.SaveChangesAsync();
    }





    public Task<int> DeleteExpiredTokensAsync(int olderThanDays, int batchSize, CancellationToken cancellationToken = default) =>
        _context.RefreshTokens
            .Where(rt => rt.ExpiresAt < DateTime.UtcNow.AddDays(-olderThanDays))
            .ExecuteDeleteAsync(cancellationToken);





    public Task<int> DeleteOldRevokedTokensAsync(int olderThanDays, int batchSize, CancellationToken cancellationToken = default) =>
        _dbSet
            .Where(rt => rt.RevokedAt != null && rt.RevokedAt < DateTime.UtcNow.AddDays(-olderThanDays))
            .ExecuteDeleteAsync(cancellationToken);
}
