using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Colors;

public class UpdateColorAction
{
    private readonly IColorRepository _colorRepository;

    public UpdateColorAction(IColorRepository colorRepository)
    {
        _colorRepository = colorRepository;
    }

    public async Task ExecuteAsync(UpdateColorDto dto)
    {
        var color = await _colorRepository.GetByIdOrThrowAsync(dto.Id);
        color.NameAr = dto.NameAr;
        color.NameEn = dto.NameEn;
        color.ColorHash = dto.ColorHash;
        await _colorRepository.UpdateAsync(color);
        await _colorRepository.SaveChangesAsync();
    }
}
