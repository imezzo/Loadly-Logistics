using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class DeviceTokenRepository : Repository<DeviceToken>, IDeviceTokenRepository
{
    public DeviceTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<DeviceToken?> GetByFcmTokenAsync(string fcmToken) =>
        _dbSet.FirstOrDefaultAsync(dt => dt.FcmToken == fcmToken && dt.IsActive);

    public Task<List<DeviceToken>> GetByUserAsync(long userId, string userType) =>
        _dbSet
            .Where(dt => dt.UserId == userId && dt.UserType == userType)
            .OrderByDescending(dt => dt.CreatedAt)
            .ToListAsync();

    public Task<List<DeviceToken>> GetActiveTokensByUserAsync(long userId, string userType) =>
        _dbSet
            .Where(dt => dt.UserId == userId && dt.UserType == userType && dt.IsActive)
            .OrderByDescending(dt => dt.LastUsedAt ?? dt.CreatedAt)
            .ToListAsync();

    public Task<DeviceToken?> GetByUserAndTokenAsync(long userId, string userType, string fcmToken) =>
        _dbSet.FirstOrDefaultAsync(dt => dt.UserId == userId && dt.UserType == userType && dt.FcmToken == fcmToken);

    public Task<DeviceToken?> GetByUserAndDeviceIdAsync(long userId, string userType, string deviceId) =>
        _dbSet.FirstOrDefaultAsync(dt => dt.UserId == userId && dt.UserType == userType && dt.DeviceId == deviceId);

    public async Task DeactivateTokenAsync(long id) =>
        await (await GetByIdAsync(id) is { } token && token.IsActive ? DeactivateTokenInternal(token) : Task.CompletedTask);

    private async Task DeactivateTokenInternal(DeviceToken token)
    {
        token.IsActive = false;
        await UpdateAsync(token);
    }

    public async Task DeactivateUserTokensAsync(long userId, string userType)
    {
        var tokens = await _dbSet
            .Where(dt => dt.UserId == userId && dt.UserType == userType && dt.IsActive)
            .ToListAsync();

        tokens.ForEach(t => t.IsActive = false);

        await (tokens.Count != 0 ? _context.SaveChangesAsync() : Task.CompletedTask);
    }

    public async Task DeactivateTokenByFcmTokenAsync(string fcmToken) =>
        await (await GetByFcmTokenAsync(fcmToken) is { } token ? DeactivateTokenInternal(token) : Task.CompletedTask);

    public async Task UpdateLastUsedAsync(long id) =>
        await (await GetByIdAsync(id) is { } token ? UpdateTokenLastUsed(token) : Task.CompletedTask);

    private async Task UpdateTokenLastUsed(DeviceToken token)
    {
        token.LastUsedAt = DateTime.UtcNow;
        await UpdateAsync(token);
    }
}
