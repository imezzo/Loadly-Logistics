using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.ServiceRequirements;

public class CreateServiceRequirementAction
{
    private readonly IServiceRequirementRepository _serviceRequirementRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateServiceRequirementAction(
        IServiceRequirementRepository serviceRequirementRepository,
        ICurrentUserService currentUserService)
    {
        _serviceRequirementRepository = serviceRequirementRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateServiceRequirementDto dto)
    {
        await _serviceRequirementRepository.AddAsync(new ServiceRequirement
        {
            NameEn = dto.NameEn,
            NameAr = dto.NameAr,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _serviceRequirementRepository.SaveChangesAsync();
    }
}
