using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Settings;

public class GetGeneralSettingAction
{
    private readonly IGeneralSettingRepository _generalSettingRepository;

    public GetGeneralSettingAction(IGeneralSettingRepository generalSettingRepository)
    {
        _generalSettingRepository = generalSettingRepository;
    }

    public Task<string?> ExecuteAsync(string key) => _generalSettingRepository.GetValueByKeyAsync(key);
}
