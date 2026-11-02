using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Settings;

public class ListGeneralSettingsAction
{
    private readonly IGeneralSettingRepository _generalSettingRepository;

    public ListGeneralSettingsAction(IGeneralSettingRepository generalSettingRepository)
    {
        _generalSettingRepository = generalSettingRepository;
    }

    public async Task<Dictionary<string, string>> ExecuteAsync()
    {
        var all = await _generalSettingRepository.GetAllAsync();
        return all.ToDictionary(gs => gs.Key, gs => gs.Value ?? string.Empty);
    }
}
