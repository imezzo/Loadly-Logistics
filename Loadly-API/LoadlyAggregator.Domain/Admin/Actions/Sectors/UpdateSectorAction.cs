using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Sectors;

public class UpdateSectorAction
{
    private readonly ISectorRepository _sectorRepository;

    public UpdateSectorAction(ISectorRepository sectorRepository)
    {
        _sectorRepository = sectorRepository;
    }

    public async Task ExecuteAsync(UpdateSectorDto dto)
    {
        var sector = await _sectorRepository.GetByIdOrThrowAsync(dto.Id);
        sector.NameEn = dto.NameEn;
        sector.NameAr = dto.NameAr;
        await _sectorRepository.UpdateAsync(sector);
        await _sectorRepository.SaveChangesAsync();
    }
}
