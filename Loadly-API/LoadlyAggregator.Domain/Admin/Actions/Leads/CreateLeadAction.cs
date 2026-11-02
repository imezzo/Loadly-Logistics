using LoadlyAggregator.Domain.Admin.DTOs.Leads;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Leads;

public class CreateLeadAction
{
    private readonly ILeadRepository _leadRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateLeadAction(ILeadRepository leadRepository, ICurrentUserService currentUserService)
    {
        _leadRepository = leadRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateLeadDto dto)
    {
        await _leadRepository.AddAsync(new Lead
        {
            FullName = dto.FullName,
            Phone = dto.Phone,
            Email = dto.Email,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _leadRepository.SaveChangesAsync();
    }
}
