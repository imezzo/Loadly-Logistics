using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.CommunicationMethods;

public class UpdateCommunicationMethodAction
{
    private readonly ICommunicationMethodRepository _communicationMethodRepository;

    public UpdateCommunicationMethodAction(ICommunicationMethodRepository communicationMethodRepository)
    {
        _communicationMethodRepository = communicationMethodRepository;
    }

    public async Task ExecuteAsync(UpdateCommunicationMethodDto dto)
    {
        var communicationMethod = await _communicationMethodRepository.GetByIdOrThrowAsync(dto.Id);

        communicationMethod.NameEn = dto.NameEn;
        communicationMethod.NameAr = dto.NameAr;
        await _communicationMethodRepository.UpdateAsync(communicationMethod);
        await _communicationMethodRepository.SaveChangesAsync();
    }
}
