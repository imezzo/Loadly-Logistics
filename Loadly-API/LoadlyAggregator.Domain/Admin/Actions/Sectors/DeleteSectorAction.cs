using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Sectors;

public class DeleteSectorAction
{
    private readonly ISectorRepository _sectorRepository;

    public DeleteSectorAction(ISectorRepository sectorRepository)
    {
        _sectorRepository = sectorRepository;
    }

    public async Task ExecuteAsync(DeleteSectorDto dto)
    {
        _sectorRepository.Remove(await _sectorRepository.GetByIdOrThrowAsync(dto.Id));
        await _sectorRepository.SaveChangesAsync();
    }
}
