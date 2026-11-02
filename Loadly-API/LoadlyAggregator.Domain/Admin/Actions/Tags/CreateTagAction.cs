using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Tags;

public class CreateTagAction
{
    private readonly ITagRepository _tagRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateTagAction(
        ITagRepository tagRepository,
        ICurrentUserService currentUserService)
    {
        _tagRepository = tagRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateTagDto dto)
    {
        await _tagRepository.AddAsync(new Tag
        {
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            IsPricing = dto.IsPricing,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _tagRepository.SaveChangesAsync();
    }
}
