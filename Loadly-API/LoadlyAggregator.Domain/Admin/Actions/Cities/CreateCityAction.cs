using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Cities;

public class CreateCityAction
{
    private readonly ICityRepository _cityRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateCityAction(
        ICityRepository cityRepository,
        ICurrentUserService currentUserService)
    {
        _cityRepository = cityRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateCityDto dto)
    {
        await _cityRepository.AddAsync(new City
        {
            NameEn = dto.NameEn,
            NameAr = dto.NameAr,
            GovernerateId = dto.GovernerateId,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _cityRepository.SaveChangesAsync();
    }
}
