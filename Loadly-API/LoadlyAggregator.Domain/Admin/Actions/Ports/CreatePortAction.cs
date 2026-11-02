using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Ports;

public class CreatePortAction
{
    private readonly IPortRepository _portRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreatePortAction(
        IPortRepository portRepository,
        ICurrentUserService currentUserService)
    {
        _portRepository = portRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreatePortDto dto)
    {
        await _portRepository.AddAsync(new Port
        {
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            Country = dto.Country,
            PortCode = dto.PortCode,
            Type = dto.Type,
            Status = dto.Status,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _portRepository.SaveChangesAsync();
    }
}
