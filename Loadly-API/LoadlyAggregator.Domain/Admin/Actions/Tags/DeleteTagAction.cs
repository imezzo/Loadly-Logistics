using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Tags;

public class DeleteTagAction
{
    private readonly ITagRepository _tagRepository;
    private readonly IVehicleTypeRepository _vehicleTypeRepository;

    public DeleteTagAction(
        ITagRepository tagRepository,
        IVehicleTypeRepository vehicleTypeRepository)
    {
        _tagRepository = tagRepository;
        _vehicleTypeRepository = vehicleTypeRepository;
    }

    public async Task ExecuteAsync(DeleteTagDto dto)
    {
        var tag = await _tagRepository.GetByIdOrThrowAsync(dto.Id);
        var links = await _vehicleTypeRepository.GetLinksByTagIdAsync(dto.Id);
        if (links.Count > 0)
        {
            _vehicleTypeRepository.RemoveVehicleTypeTags(links);
        }

        _tagRepository.Remove(tag);
        await _tagRepository.SaveChangesAsync();
    }
}
