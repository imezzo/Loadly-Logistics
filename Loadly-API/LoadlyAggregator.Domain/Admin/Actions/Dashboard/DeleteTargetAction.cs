using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class DeleteTargetAction
{
    private readonly ITargetRepository _targetRepository;

    public DeleteTargetAction(ITargetRepository targetRepository)
    {
        _targetRepository = targetRepository;
    }

    public async Task ExecuteAsync(DeleteTargetDto dto)
    {
        var target = await _targetRepository.GetByIdOrThrowAsync(dto.Id);
        _targetRepository.Remove(target);
        await _targetRepository.SaveChangesAsync();
    }
}
