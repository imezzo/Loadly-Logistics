using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.CommunicationMethods;

public class DeleteCommunicationMethodAction
{
    private readonly ICommunicationMethodRepository _communicationMethodRepository;

    public DeleteCommunicationMethodAction(ICommunicationMethodRepository communicationMethodRepository)
    {
        _communicationMethodRepository = communicationMethodRepository;
    }

    public async Task ExecuteAsync(DeleteCommunicationMethodDto dto)
    {
        _communicationMethodRepository.Remove(await _communicationMethodRepository.GetByIdOrThrowAsync(dto.Id));
        await _communicationMethodRepository.SaveChangesAsync();
    }
}
