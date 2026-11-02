using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Governerates;

public class CreateGovernerateAction
{
    private readonly IGovernerateRepository _governerateRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateGovernerateAction(
        IGovernerateRepository governerateRepository,
        ICurrentUserService currentUserService)
    {
        _governerateRepository = governerateRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateGovernerateDto dto)
    {
        await _governerateRepository.AddAsync(new Governerate
        {
            NameEn = dto.NameEn,
            NameAr = dto.NameAr,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _governerateRepository.SaveChangesAsync();
    }
}
