using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Mfa;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.Domain.Contractor.Actions.Auth;

public class ContractorSendLoginOtpAction : IAction<ContractorSendLoginOtpDto, object>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly MfaMethodStrategyFactory _strategyFactory;
    private readonly IMfaCacheService _mfaCacheService;
    private readonly INotificationTransport _notificationTransport;
    private readonly IJwtTokenService _jwtTokenService;

    public ContractorSendLoginOtpAction(
        IContractorRepository contractorRepository,
        MfaMethodStrategyFactory strategyFactory,
        IMfaCacheService mfaCacheService,
        INotificationTransport notificationTransport,
        IJwtTokenService jwtTokenService)
    {
        _contractorRepository = contractorRepository;
        _strategyFactory = strategyFactory;
        _mfaCacheService = mfaCacheService;
        _notificationTransport = notificationTransport;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<object> ExecuteAsync(ContractorSendLoginOtpDto dto)
    {
        ValidateRequest(dto);
        var contractor = await _contractorRepository.GetByLoginAsync((dto.Login ?? string.Empty).Trim());
        UserValidator.ValidateUserExists(contractor, "Contractor");
        UserValidator.ValidateStatus(contractor!.Status);
        var channel = DetermineChannel(dto.Login ?? string.Empty, contractor);
        _ = await _mfaCacheService.IsRateLimitedAsync(contractor.Id, "contractor")
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;
        var code = Domain.Common.Utilities.CodeGenerator.GenerateNumericCode(6);
        var token = _jwtTokenService.GenerateRefreshToken();

        await _mfaCacheService.StoreMfaCodeAsync(
            userId: contractor.Id,
            userType: "contractor",
            code: code,
            channel: channel);

        await _mfaCacheService.StoreLoginOtpTokenAsync(
            userId: contractor.Id,
            userType: "contractor",
            token: token,
            expirationMinutes: _mfaCacheService.MfaCodeExpirationMinutes);

        if (channel == CommunicationChannel.Sms)
        {
            var sent = await _notificationTransport.SendSmsAsync(contractor.Phone, code, NotificationDomain.Contractor, contractor.Id, "Contractor");
            if (!sent)
            {
                throw new LogicalException("auth.unable_to_send_verification_code");
            }
        }
        else
        {
            var channelString = ChannelHelper.ToChannelString(channel);
            await _strategyFactory.GetStrategy(channelString)
                .SendCodeAsync(
                    MfaRecipientResolver.GetRecipient(channelString, contractor.Email, contractor.Phone),
                    contractor.FullName,
                    code,
                    "contractor",
                    _mfaCacheService.MfaCodeExpirationMinutes,
                    contractor.Id,
                    "contractor");
        }

        return new { Token = token };
    }

    private static CommunicationChannel DetermineChannel(string login, Domain.Common.Entities.Contractor contractor)
    {
        if (IdentifierValidator.IsValidEmail(login))
        {
            return CommunicationChannel.Email;
        }
        if (IdentifierValidator.IsValidPhone(login))
        {
            return CommunicationChannel.Sms;
        }
        if (!string.IsNullOrWhiteSpace(contractor.Email) && login.Equals(contractor.Email, StringComparison.OrdinalIgnoreCase))
        {
            return CommunicationChannel.Email;
        }
        if (!string.IsNullOrWhiteSpace(contractor.Phone) && login.Equals(contractor.Phone, StringComparison.OrdinalIgnoreCase))
        {
            return CommunicationChannel.Sms;
        }
        throw new ValidationException("validation.invalid_login_identifier");
    }

    private static void ValidateRequest(ContractorSendLoginOtpDto dto)
    {
        _ = string.IsNullOrWhiteSpace(dto.Login)
            ? throw new ValidationException("validation.login_required")
            : true;
    }
}
