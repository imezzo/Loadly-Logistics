using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Ports;

public class DeletePortAction
{
    private readonly IPortRepository _portRepository;

    public DeletePortAction(IPortRepository portRepository)
    {
        _portRepository = portRepository;
    }

    public async Task ExecuteAsync(DeletePortDto dto)
    {
        Common.Entities.Port port = await _portRepository.GetByIdOrThrowAsync(dto.Id);
        _portRepository.Remove(port);
        await _portRepository.SaveChangesAsync();
    }
}
