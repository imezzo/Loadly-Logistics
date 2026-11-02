using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Containers;

public class CreateContainerAction
{
    private readonly IContainerRepository _containerRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateContainerAction(
        IContainerRepository containerRepository,
        ICurrentUserService currentUserService)
    {
        _containerRepository = containerRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateContainerDto dto)
    {
        await _containerRepository.AddAsync(new Container
        {
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            Status = dto.Status,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _containerRepository.SaveChangesAsync();
    }
}
