using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Settings;

public class SetGeneralSettingAction
{
    private readonly IGeneralSettingRepository _generalSettingRepository;

    public SetGeneralSettingAction(IGeneralSettingRepository generalSettingRepository)
    {
        _generalSettingRepository = generalSettingRepository;
    }

    public async Task ExecuteAsync(string key, string value)
    {
        var existing = await _generalSettingRepository.GetByKeyAsync(key);
        if (existing != null)
        {
            existing.Value = value;
            await _generalSettingRepository.UpdateAsync(existing);
        }
        else
        {
            await _generalSettingRepository.AddAsync(new GeneralSetting
            {
                Key = key,
                Value = value
            });
        }
        await _generalSettingRepository.SaveChangesAsync();
    }
}
