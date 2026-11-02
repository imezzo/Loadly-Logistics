using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Mfa;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Utilities;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.Domain.Contractor.Actions.Auth;





public class ContractorSendMfaCodeAction : IAction<ContractorSendMfaCodeDto, object>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly MfaMethodStrategyFactory _strategyFactory;
    private readonly IMfaCacheService _mfaCacheService;

    public ContractorSendMfaCodeAction(
        IContractorRepository contractorRepository,
        MfaMethodStrategyFactory strategyFactory,
        IMfaCacheService mfaCacheService)
    {
        _contractorRepository = contractorRepository;
        _strategyFactory = strategyFactory;
        _mfaCacheService = mfaCacheService;
    }

    public async Task<object> ExecuteAsync(ContractorSendMfaCodeDto dto)
    {
        var contractor = await _contractorRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(contractor, "Contractor");
        UserValidator.ValidateMfaChannel(dto.Channel);
        var code = CodeGenerator.GenerateNumericCode(6);
        await _mfaCacheService.StoreMfaCodeAsync(
            userId: contractor!.Id,
            userType: "contractor",
            code: code,
            channel: dto.Channel);
        var channelString = ChannelHelper.ToChannelString(dto.Channel);
        await _strategyFactory.GetStrategy(channelString)
            .SendCodeAsync(
                MfaRecipientResolver.GetRecipient(channelString, contractor.Email, contractor.Phone),
                contractor.FullName,
                code,
                "contractor",
                _mfaCacheService.MfaCodeExpirationMinutes,
                contractor.Id,
                "contractor");
        return new { };
    }
}

