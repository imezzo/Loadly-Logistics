using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Districts;

public class UpdateDistrictAction
{
    private readonly IDistrictRepository _districtRepository;

    public UpdateDistrictAction(IDistrictRepository districtRepository)
    {
        _districtRepository = districtRepository;
    }

    public async Task ExecuteAsync(UpdateDistrictDto dto)
    {
        var district = await _districtRepository.GetByIdOrThrowAsync(dto.Id);
        district.NameEn = dto.NameEn;
        district.NameAr = dto.NameAr;
        district.CityId = dto.CityId;
        await _districtRepository.UpdateAsync(district);
        await _districtRepository.SaveChangesAsync();
    }
}
