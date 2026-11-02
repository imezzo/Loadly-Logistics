using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Containers;

public class DeleteContainerAction
{
    private readonly IContainerRepository _containerRepository;

    public DeleteContainerAction(IContainerRepository containerRepository)
    {
        _containerRepository = containerRepository;
    }

    public async Task ExecuteAsync(DeleteContainerDto dto)
    {
        Common.Entities.Container container = await _containerRepository.GetByIdOrThrowAsync(dto.Id);
        _containerRepository.Remove(container);
        await _containerRepository.SaveChangesAsync();
    }
}
