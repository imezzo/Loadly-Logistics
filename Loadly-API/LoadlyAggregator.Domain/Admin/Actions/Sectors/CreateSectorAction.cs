using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Sectors;

public class CreateSectorAction
{
    private readonly ISectorRepository _sectorRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateSectorAction(
        ISectorRepository sectorRepository,
        ICurrentUserService currentUserService)
    {
        _sectorRepository = sectorRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateSectorDto dto)
    {
        await _sectorRepository.AddAsync(new Sector
        {
            NameEn = dto.NameEn,
            NameAr = dto.NameAr,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _sectorRepository.SaveChangesAsync();
    }
}
