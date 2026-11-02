using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.Domain.Collector.Actions.Auth;

public class CollectorForgetPasswordAction : IAction<CollectorForgetPasswordDto, bool>
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly IPasswordResetCacheService _passwordResetCacheService;
    private readonly IResetPasswordUrlService _resetPasswordUrlService;
    private readonly INotificationTransport _notificationTransport;

    public CollectorForgetPasswordAction(
        ICollectorRepository collectorRepository,
        IPasswordResetTokenService passwordResetTokenService,
        IPasswordResetCacheService passwordResetCacheService,
        IResetPasswordUrlService resetPasswordUrlService,
        INotificationTransport notificationTransport)
    {
        _collectorRepository = collectorRepository;
        _passwordResetTokenService = passwordResetTokenService;
        _passwordResetCacheService = passwordResetCacheService;
        _resetPasswordUrlService = resetPasswordUrlService;
        _notificationTransport = notificationTransport;
    }

    public async Task<bool> ExecuteAsync(CollectorForgetPasswordDto dto)
    {
        var phone = dto.Identifier.Trim();
        _ = await _passwordResetCacheService.IsRateLimitedAsync("collector", phone)
            ? throw new TooManyRequestsException("auth.too_many_password_reset_requests")
            : true;

        var collector = await _collectorRepository.GetByPhoneAsync(phone);
        return collector == null
            ? true
            : await SendPasswordResetAsync(collector, dto);
    }

    private async Task<bool> SendPasswordResetAsync(Domain.Common.Entities.Collector collector, CollectorForgetPasswordDto dto)
    {
        var resetCode = CodeGenerator.GenerateAlphanumericCode(6);
        var resetToken = _passwordResetTokenService.GenerateResetToken(collector.Id, "collector", channel: dto.Channel);
        await _passwordResetCacheService.StoreResetAttemptAsync(
            userType: "collector",
            identifier: dto.Identifier,
            userId: collector.Id,
            code: resetCode,
            channel: dto.Channel);

        await _notificationTransport.SendSmsAsync(
            collector.Phone,
            FormatSmsMessage(resetCode, resetToken),
            NotificationDomain.Collector,
            collector.Id,
            "Collector");

        return true;
    }

    private string FormatSmsMessage(string resetCode, string resetToken)
    {
        var resetUrl = _resetPasswordUrlService.GenerateResetPasswordUrl("collector", resetToken);
        return $"Your password reset code is {resetCode}. Reset your password here: {resetUrl}";
    }
}
