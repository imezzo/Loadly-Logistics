using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Containers;

public class UpdateContainerAction
{
    private readonly IContainerRepository _containerRepository;

    public UpdateContainerAction(IContainerRepository containerRepository)
    {
        _containerRepository = containerRepository;
    }

    public async Task ExecuteAsync(UpdateContainerDto dto)
    {
        Common.Entities.Container container = await _containerRepository.GetByIdOrThrowAsync(dto.Id);
        container.NameAr = dto.NameAr;
        container.NameEn = dto.NameEn;
        container.Status = dto.Status;
        await _containerRepository.UpdateAsync(container);
        await _containerRepository.SaveChangesAsync();
    }
}
