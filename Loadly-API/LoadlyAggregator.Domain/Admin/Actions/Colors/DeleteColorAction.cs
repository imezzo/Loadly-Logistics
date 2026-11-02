using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Colors;

public class DeleteColorAction
{
    private readonly IColorRepository _colorRepository;

    public DeleteColorAction(IColorRepository colorRepository)
    {
        _colorRepository = colorRepository;
    }

    public async Task ExecuteAsync(DeleteColorDto dto)
    {
        _colorRepository.Remove(await _colorRepository.GetByIdOrThrowAsync(dto.Id));
        await _colorRepository.SaveChangesAsync();
    }
}
