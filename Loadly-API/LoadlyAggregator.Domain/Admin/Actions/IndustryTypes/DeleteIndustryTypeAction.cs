using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.IndustryTypes;

public class DeleteIndustryTypeAction
{
    private readonly IIndustryTypeRepository _industryTypeRepository;

    public DeleteIndustryTypeAction(IIndustryTypeRepository industryTypeRepository)
    {
        _industryTypeRepository = industryTypeRepository;
    }

    public async Task ExecuteAsync(DeleteIndustryTypeDto dto)
    {
        var industryType = await _industryTypeRepository.GetByIdOrThrowAsync(dto.Id);
        _industryTypeRepository.Remove(industryType);
        await _industryTypeRepository.SaveChangesAsync();
    }
}
