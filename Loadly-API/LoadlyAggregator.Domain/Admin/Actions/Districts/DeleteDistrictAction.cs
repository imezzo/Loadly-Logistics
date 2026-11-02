using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Districts;

public class DeleteDistrictAction
{
    private readonly IDistrictRepository _districtRepository;

    public DeleteDistrictAction(IDistrictRepository districtRepository)
    {
        _districtRepository = districtRepository;
    }

    public async Task ExecuteAsync(DeleteDistrictDto dto)
    {
        _districtRepository.Remove(await _districtRepository.GetByIdOrThrowAsync(dto.Id));
        await _districtRepository.SaveChangesAsync();
    }
}
