using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.ServiceRequirements;

public class UpdateServiceRequirementAction
{
    private readonly IServiceRequirementRepository _serviceRequirementRepository;

    public UpdateServiceRequirementAction(IServiceRequirementRepository serviceRequirementRepository)
    {
        _serviceRequirementRepository = serviceRequirementRepository;
    }

    public async Task ExecuteAsync(UpdateServiceRequirementDto dto)
    {
        var serviceRequirement = await _serviceRequirementRepository.GetByIdOrThrowAsync(dto.Id);

        serviceRequirement.NameEn = dto.NameEn;
        serviceRequirement.NameAr = dto.NameAr;
        await _serviceRequirementRepository.UpdateAsync(serviceRequirement);
        await _serviceRequirementRepository.SaveChangesAsync();
    }
}
