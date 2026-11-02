using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.ServiceRequirements;

public class DeleteServiceRequirementAction
{
    private readonly IServiceRequirementRepository _serviceRequirementRepository;

    public DeleteServiceRequirementAction(IServiceRequirementRepository serviceRequirementRepository)
    {
        _serviceRequirementRepository = serviceRequirementRepository;
    }

    public async Task ExecuteAsync(DeleteServiceRequirementDto dto)
    {
        var serviceRequirement = await _serviceRequirementRepository.GetByIdOrThrowAsync(dto.Id);
        _serviceRequirementRepository.Remove(serviceRequirement);
        await _serviceRequirementRepository.SaveChangesAsync();
    }
}
