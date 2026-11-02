using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Mfa;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Utilities;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;

namespace LoadlyAggregator.Domain.Driver.Actions.Auth;





public class DriverSendMfaCodeAction : IAction<DriverSendMfaCodeDto, object>
{
    private readonly IDriverRepository _driverRepository;
    private readonly MfaMethodStrategyFactory _strategyFactory;
    private readonly IMfaCacheService _mfaCacheService;

    public DriverSendMfaCodeAction(
        IDriverRepository driverRepository,
        MfaMethodStrategyFactory strategyFactory,
        IMfaCacheService mfaCacheService)
    {
        _driverRepository = driverRepository;
        _strategyFactory = strategyFactory;
        _mfaCacheService = mfaCacheService;
    }

    public async Task<object> ExecuteAsync(DriverSendMfaCodeDto dto)
    {
        var driver = await _driverRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(driver, "Driver");
        UserValidator.ValidateMfaChannel(dto.Channel);
        var code = CodeGenerator.GenerateNumericCode(6);
        await _mfaCacheService.StoreMfaCodeAsync(
            userId: driver!.Id,
            userType: "driver",
            code: code,
            channel: dto.Channel);
        var channelString = ChannelHelper.ToChannelString(dto.Channel);
        await _strategyFactory.GetStrategy(channelString)
            .SendCodeAsync(
                MfaRecipientResolver.GetRecipient(channelString, driver.Email, driver.Phone),
                driver.FullName,
                code,
                "driver",
                _mfaCacheService.MfaCodeExpirationMinutes,
                driver.Id,
                "Driver");
        return new { };
    }
}

