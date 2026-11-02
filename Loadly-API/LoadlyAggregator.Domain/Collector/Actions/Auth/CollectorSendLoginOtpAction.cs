using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Collector.Actions.Auth;

public class CollectorSendLoginOtpAction : IAction<CollectorSendLoginOtpDto, object>
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly IMfaCacheService _mfaCacheService;
    private readonly INotificationTransport _notificationTransport;
    private readonly IJwtTokenService _jwtTokenService;

    public CollectorSendLoginOtpAction(
        ICollectorRepository collectorRepository,
        IMfaCacheService mfaCacheService,
        INotificationTransport notificationTransport,
        IJwtTokenService jwtTokenService)
    {
        _collectorRepository = collectorRepository;
        _mfaCacheService = mfaCacheService;
        _notificationTransport = notificationTransport;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<object> ExecuteAsync(CollectorSendLoginOtpDto dto)
    {
        ValidateRequest(dto);
        var collector = await _collectorRepository.GetByLoginAsync(dto.Login.Trim());
        UserValidator.ValidateUserExists(collector, "Collector");
        UserValidator.ValidateStatus(collector!.Status);
        ValidateCollectorCanReceiveSms(collector);
        _ = await _mfaCacheService.IsRateLimitedAsync(collector.Id, "collector")
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;

        var code = Domain.Common.Utilities.CodeGenerator.GenerateNumericCode(6);
        var token = _jwtTokenService.GenerateRefreshToken();

        await _mfaCacheService.StoreMfaCodeAsync(
            userId: collector.Id,
            userType: "collector",
            code: code,
            channel: CommunicationChannel.Sms);

        await _mfaCacheService.StoreLoginOtpTokenAsync(
            userId: collector.Id,
            userType: "collector",
            token: token,
            expirationMinutes: _mfaCacheService.MfaCodeExpirationMinutes);

        var sent = await _notificationTransport.SendSmsAsync(
            collector.Phone,
            code,
            NotificationDomain.Collector,
            collector.Id,
            "Collector");
        if (!sent)
        {
            throw new LogicalException("auth.unable_to_send_verification_code");
        }

        return new { Token = token };
    }

    private static void ValidateCollectorCanReceiveSms(Domain.Common.Entities.Collector collector)
    {
        _ = string.IsNullOrWhiteSpace(collector.Phone)
            ? throw new ValidationException("validation.invalid_login_identifier")
            : true;
    }

    private static void ValidateRequest(CollectorSendLoginOtpDto dto)
    {
        _ = string.IsNullOrWhiteSpace(dto.Login)
            ? throw new ValidationException("validation.login_required")
            : true;
    }
}
