using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.CommunicationMethods;

public class CreateCommunicationMethodAction
{
    private readonly ICommunicationMethodRepository _communicationMethodRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateCommunicationMethodAction(
        ICommunicationMethodRepository communicationMethodRepository,
        ICurrentUserService currentUserService)
    {
        _communicationMethodRepository = communicationMethodRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateCommunicationMethodDto dto)
    {
        await _communicationMethodRepository.AddAsync(new CommunicationMethod
        {
            NameEn = dto.NameEn,
            NameAr = dto.NameAr,
            AddedBy = _currentUserService.GetCurrentUserId()
        });
        await _communicationMethodRepository.SaveChangesAsync();
    }
}
