using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Collector.Actions.Auth;




public class CollectorEnableMfaAction : IAction<CollectorEnableMfaDto, bool>
{
    private readonly ICollectorRepository _collectorRepository;

    public CollectorEnableMfaAction(ICollectorRepository collectorRepository)
    {
        _collectorRepository = collectorRepository;
    }

    public async Task<bool> ExecuteAsync(CollectorEnableMfaDto dto)
    {
        var collector = await _collectorRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(collector, "Collector");

        UserValidator.ValidateChannelVerified(dto.Channel, collector!.EmailVerified, collector.PhoneVerified);

        collector.MfaChannel = dto.Channel;
        await _collectorRepository.UpdateAsync(collector);
        await _collectorRepository.SaveChangesAsync();

        return true;
    }
}

