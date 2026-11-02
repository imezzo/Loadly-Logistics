using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Enums;
namespace LoadlyAggregator.Domain.Collector.Actions.Auth;



public class CollectorDisableMfaAction : IAction<CollectorDisableMfaDto, bool>
{
    private readonly ICollectorRepository _collectorRepository;
    public CollectorDisableMfaAction(ICollectorRepository collectorRepository)
    {
        _collectorRepository = collectorRepository;
    }
    public async Task<bool> ExecuteAsync(CollectorDisableMfaDto dto)
    {
        var collector = await _collectorRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(collector, "Collector");

        collector!.MfaChannel = CommunicationChannel.None;
        await _collectorRepository.UpdateAsync(collector);
        await _collectorRepository.SaveChangesAsync();
        return true;
    }
}
