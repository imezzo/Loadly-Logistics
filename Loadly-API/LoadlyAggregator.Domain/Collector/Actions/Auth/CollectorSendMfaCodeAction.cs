using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Mfa;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.Domain.Collector.Actions.Auth;





public class CollectorSendMfaCodeAction : IAction<CollectorSendMfaCodeDto, object>
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly MfaMethodStrategyFactory _strategyFactory;
    private readonly IMfaCacheService _mfaCacheService;

    public CollectorSendMfaCodeAction(
        ICollectorRepository collectorRepository,
        MfaMethodStrategyFactory strategyFactory,
        IMfaCacheService mfaCacheService)
    {
        _collectorRepository = collectorRepository;
        _strategyFactory = strategyFactory;
        _mfaCacheService = mfaCacheService;
    }

    public async Task<object> ExecuteAsync(CollectorSendMfaCodeDto dto)
    {
        var collector = await _collectorRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(collector, "Collector");
        var channel = CommunicationChannel.Sms;
        var code = CodeGenerator.GenerateNumericCode(6);
        await _mfaCacheService.StoreMfaCodeAsync(
            userId: collector!.Id,
            userType: "collector",
            code: code,
            channel: channel);
        var channelString = ChannelHelper.ToChannelString(channel);
        await _strategyFactory.GetStrategy(channelString)
            .SendCodeAsync(
                MfaRecipientResolver.GetRecipient(channelString, collector.Email, collector.Phone),
                collector.FullName,
                code,
                "collector",
                _mfaCacheService.MfaCodeExpirationMinutes,
                collector.Id,
                "Collector");
        return new { };
    }
}

