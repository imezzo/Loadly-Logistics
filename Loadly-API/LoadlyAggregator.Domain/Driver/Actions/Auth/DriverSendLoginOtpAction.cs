using System;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Mfa;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Utilities;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Driver.Actions.Auth;



public class DriverSendLoginOtpAction : IAction<DriverSendLoginOtpDto, object>
{
    private readonly IDriverRepository _driverRepository;
    private readonly MfaMethodStrategyFactory _strategyFactory;
    private readonly IMfaCacheService _mfaCacheService;
    private readonly INotificationTransport _notificationTransport;
    private readonly IJwtTokenService _jwtTokenService;

    public DriverSendLoginOtpAction(
        IDriverRepository driverRepository,
        MfaMethodStrategyFactory strategyFactory,
        IMfaCacheService mfaCacheService,
        INotificationTransport notificationTransport,
        IJwtTokenService jwtTokenService)
    {
        _driverRepository = driverRepository;
        _strategyFactory = strategyFactory;
        _mfaCacheService = mfaCacheService;
        _notificationTransport = notificationTransport;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<object> ExecuteAsync(DriverSendLoginOtpDto dto)
    {
        ValidateRequest(dto);
        var driver = await _driverRepository.GetByLoginAsync((dto.Login ?? string.Empty).Trim().ToLower());
        UserValidator.ValidateUserExists(driver, "Driver");
        UserValidator.ValidateStatus(driver!.Status);
        var channel = DetermineChannel(dto.Login ?? string.Empty, driver);
        _ = await _mfaCacheService.IsRateLimitedAsync(driver.Id, "driver")
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;
        var code = Domain.Common.Utilities.CodeGenerator.GenerateNumericCode(6);
        var token = _jwtTokenService.GenerateRefreshToken();

        await _mfaCacheService.StoreMfaCodeAsync(
            userId: driver.Id,
            userType: "driver",
            code: code,
            channel: channel);

        await _mfaCacheService.StoreLoginOtpTokenAsync(
            userId: driver.Id,
            userType: "driver",
            token: token,
            expirationMinutes: _mfaCacheService.MfaCodeExpirationMinutes);

        if (channel == CommunicationChannel.Sms)
        {
            var sent = await _notificationTransport.SendSmsAsync(driver.Phone, code, NotificationDomain.Driver, driver.Id, "Driver");
            if (!sent)
            {
                throw new LogicalException("auth.unable_to_send_verification_code");
            }
        }
        else
        {
            var channelString = Shared.Utilities.ChannelHelper.ToChannelString(channel);
            await _strategyFactory.GetStrategy(channelString)
                .SendCodeAsync(
                    MfaRecipientResolver.GetRecipient(channelString, driver.Email, driver.Phone),
                    driver.FullName,
                    code,
                    "driver",
                    _mfaCacheService.MfaCodeExpirationMinutes,
                    driver.Id,
                    "Driver");
        }

        return new { Token = token };
    }

    private static CommunicationChannel DetermineChannel(string login, Domain.Common.Entities.Driver driver)
    {
        if (IdentifierValidator.IsValidEmail(login))
        {
            return CommunicationChannel.Email;
        }
        if (IdentifierValidator.IsValidPhone(login))
        {
            return CommunicationChannel.Sms;
        }
        if (!string.IsNullOrWhiteSpace(driver.Email) && login.Equals(driver.Email, StringComparison.OrdinalIgnoreCase))
        {
            return CommunicationChannel.Email;
        }
        if (!string.IsNullOrWhiteSpace(driver.Phone) && login.Equals(driver.Phone, StringComparison.OrdinalIgnoreCase))
        {
            return CommunicationChannel.Sms;
        }
        throw new ValidationException("validation.invalid_login_identifier");
    }

    private static void ValidateRequest(DriverSendLoginOtpDto dto)
    {
        _ = string.IsNullOrWhiteSpace(dto.Login)
            ? throw new ValidationException("validation.login_required")
            : true;
    }
}
