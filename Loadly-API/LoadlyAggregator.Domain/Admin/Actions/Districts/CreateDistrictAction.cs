using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Districts;

public class CreateDistrictAction
{
    private readonly IDistrictRepository _districtRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateDistrictAction(
        IDistrictRepository districtRepository,
        ICurrentUserService currentUserService)
    {
        _districtRepository = districtRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateDistrictDto dto)
    {
        await _districtRepository.AddAsync(new District
        {
            NameEn = dto.NameEn,
            NameAr = dto.NameAr,
            CityId = dto.CityId,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _districtRepository.SaveChangesAsync();
    }
}
