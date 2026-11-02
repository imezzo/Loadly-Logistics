using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Governerates;

public class DeleteGovernerateAction
{
    private readonly IGovernerateRepository _governerateRepository;

    public DeleteGovernerateAction(IGovernerateRepository governerateRepository)
    {
        _governerateRepository = governerateRepository;
    }

    public async Task ExecuteAsync(DeleteGovernerateDto dto)
    {
        _governerateRepository.Remove(await _governerateRepository.GetByIdOrThrowAsync(dto.Id));
        await _governerateRepository.SaveChangesAsync();
    }
}
