using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.IndustryTypes;

public class CreateIndustryTypeAction
{
    private readonly IIndustryTypeRepository _industryTypeRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateIndustryTypeAction(
        IIndustryTypeRepository industryTypeRepository,
        ICurrentUserService currentUserService)
    {
        _industryTypeRepository = industryTypeRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateIndustryTypeDto dto)
    {
        await _industryTypeRepository.AddAsync(new IndustryType
        {
            NameEn = dto.NameEn,
            NameAr = dto.NameAr,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _industryTypeRepository.SaveChangesAsync();
    }
}
