using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Mfa;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Company.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.Domain.Company.Actions.Auth;

public class CompanySendLoginOtpAction : IAction<CompanySendLoginOtpDto, object>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly MfaMethodStrategyFactory _strategyFactory;
    private readonly IMfaCacheService _mfaCacheService;
    private readonly INotificationTransport _notificationTransport;
    private readonly IJwtTokenService _jwtTokenService;

    public CompanySendLoginOtpAction(
        ICompanyRepository companyRepository,
        MfaMethodStrategyFactory strategyFactory,
        IMfaCacheService mfaCacheService,
        INotificationTransport notificationTransport,
        IJwtTokenService jwtTokenService)
    {
        _companyRepository = companyRepository;
        _strategyFactory = strategyFactory;
        _mfaCacheService = mfaCacheService;
        _notificationTransport = notificationTransport;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<object> ExecuteAsync(CompanySendLoginOtpDto dto)
    {
        ValidateRequest(dto);
        var company = await _companyRepository.GetByLoginAsync((dto.Login ?? string.Empty).Trim());
        UserValidator.ValidateUserExists(company, "Company");
        UserValidator.ValidateStatus(company!.Status);
        var channel = DetermineChannel(dto.Login ?? string.Empty, company);
        _ = await _mfaCacheService.IsRateLimitedAsync(company.Id, "company")
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;
        var code = Domain.Common.Utilities.CodeGenerator.GenerateNumericCode(6);
        var token = _jwtTokenService.GenerateRefreshToken();

        await _mfaCacheService.StoreMfaCodeAsync(
            userId: company.Id,
            userType: "company",
            code: code,
            channel: channel);

        await _mfaCacheService.StoreLoginOtpTokenAsync(
            userId: company.Id,
            userType: "company",
            token: token,
            expirationMinutes: _mfaCacheService.MfaCodeExpirationMinutes);

        if (channel == CommunicationChannel.Sms)
        {
            var sent = await _notificationTransport.SendSmsAsync(company.Phone, code, NotificationDomain.Company, company.Id, "Company");
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
                    MfaRecipientResolver.GetRecipient(channelString, company.Email, company.Phone),
                    company.FullName,
                    code,
                    "company",
                    _mfaCacheService.MfaCodeExpirationMinutes,
                    company.Id,
                    "Company");
        }

        return new { Token = token };
    }

    private static CommunicationChannel DetermineChannel(string login, Domain.Common.Entities.Company company)
    {
        if (IdentifierValidator.IsValidEmail(login))
        {
            return CommunicationChannel.Email;
        }
        if (IdentifierValidator.IsValidPhone(login))
        {
            return CommunicationChannel.Sms;
        }
        if (!string.IsNullOrWhiteSpace(company.Email) && login.Equals(company.Email, StringComparison.OrdinalIgnoreCase))
        {
            return CommunicationChannel.Email;
        }
        if (!string.IsNullOrWhiteSpace(company.Phone) && login.Equals(company.Phone, StringComparison.OrdinalIgnoreCase))
        {
            return CommunicationChannel.Sms;
        }
        throw new ValidationException("validation.invalid_login_identifier");
    }

    private static void ValidateRequest(CompanySendLoginOtpDto dto)
    {
        _ = string.IsNullOrWhiteSpace(dto.Login)
            ? throw new ValidationException("validation.login_required")
            : true;
    }
}
