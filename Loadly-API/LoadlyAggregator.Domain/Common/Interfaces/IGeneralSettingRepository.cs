using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IGeneralSettingRepository : IRepository<GeneralSetting>
{
    Task<GeneralSetting?> GetByKeyAsync(string key);
    Task<string?> GetValueByKeyAsync(string key);
    Task<Dictionary<string, string>> GetByPrefixAsync(string keyPrefix);
}
