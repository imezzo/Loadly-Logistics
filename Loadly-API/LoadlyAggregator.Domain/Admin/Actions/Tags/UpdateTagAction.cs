using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Tags;

public class UpdateTagAction
{
    private readonly ITagRepository _tagRepository;

    public UpdateTagAction(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task ExecuteAsync(UpdateTagDto dto)
    {
        var tag = await _tagRepository.GetByIdOrThrowAsync(dto.Id);

        tag.NameAr = dto.NameAr;
        tag.NameEn = dto.NameEn;
        tag.IsPricing = dto.IsPricing;
        await _tagRepository.UpdateAsync(tag);
        await _tagRepository.SaveChangesAsync();
    }
}
