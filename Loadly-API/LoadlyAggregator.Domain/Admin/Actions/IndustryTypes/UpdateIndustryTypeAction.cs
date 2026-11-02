using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.IndustryTypes;

public class UpdateIndustryTypeAction
{
    private readonly IIndustryTypeRepository _industryTypeRepository;

    public UpdateIndustryTypeAction(IIndustryTypeRepository industryTypeRepository)
    {
        _industryTypeRepository = industryTypeRepository;
    }

    public async Task ExecuteAsync(UpdateIndustryTypeDto dto)
    {
        var industryType = await _industryTypeRepository.GetByIdOrThrowAsync(dto.Id);

        industryType.NameEn = dto.NameEn;
        industryType.NameAr = dto.NameAr;
        await _industryTypeRepository.UpdateAsync(industryType);
        await _industryTypeRepository.SaveChangesAsync();
    }
}
