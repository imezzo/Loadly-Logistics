using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class GeneralSettingRepository : Repository<GeneralSetting>, IGeneralSettingRepository
{
    public GeneralSettingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<GeneralSetting?> GetByKeyAsync(string key) =>
        _dbSet.FirstOrDefaultAsync(gs => gs.Key == key);

    public async Task<string?> GetValueByKeyAsync(string key)
    {
        var setting = await GetByKeyAsync(key);
        return setting?.Value;
    }

    public async Task<Dictionary<string, string>> GetByPrefixAsync(string keyPrefix)
    {
        var settings = await _dbSet
            .Where(gs => gs.Key.StartsWith(keyPrefix))
            .ToListAsync();
        return settings.ToDictionary(gs => gs.Key, gs => gs.Value ?? string.Empty);
    }
}
