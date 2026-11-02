using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Colors;

public class CreateColorAction
{
    private readonly IColorRepository _colorRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateColorAction(
        IColorRepository colorRepository,
        ICurrentUserService currentUserService)
    {
        _colorRepository = colorRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateColorDto dto)
    {
        await _colorRepository.AddAsync(new Color
        {
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            ColorHash = dto.ColorHash,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _colorRepository.SaveChangesAsync();
    }
}
