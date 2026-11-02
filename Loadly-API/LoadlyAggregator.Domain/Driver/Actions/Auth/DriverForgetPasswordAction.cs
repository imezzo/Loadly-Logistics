using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Utilities;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Driver.Actions.Auth;






public class DriverForgetPasswordAction : IAction<DriverForgetPasswordDto, bool>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly IPasswordResetCacheService _passwordResetCacheService;
    private readonly IResetPasswordUrlService _resetPasswordUrlService;
    private readonly INotificationTransport _notificationTransport;

    public DriverForgetPasswordAction(
        IDriverRepository driverRepository,
        IPasswordResetTokenService passwordResetTokenService,
        IPasswordResetCacheService passwordResetCacheService,
        IResetPasswordUrlService resetPasswordUrlService,
        INotificationTransport notificationTransport)
    {
        _driverRepository = driverRepository;
        _passwordResetTokenService = passwordResetTokenService;
        _passwordResetCacheService = passwordResetCacheService;
        _resetPasswordUrlService = resetPasswordUrlService;
        _notificationTransport = notificationTransport;
    }

    public async Task<bool> ExecuteAsync(DriverForgetPasswordDto dto)
    {
        var normalizedIdentifier = (dto.Identifier ?? string.Empty).Trim().ToLower();
        _ = await _passwordResetCacheService.IsRateLimitedAsync("driver", normalizedIdentifier)
            ? throw new TooManyRequestsException("auth.too_many_password_reset_requests")
            : true;
        var driver = dto.Channel == CommunicationChannel.Email
            ? await _driverRepository.GetByEmailAsync(normalizedIdentifier)
            : await _driverRepository.GetByPhoneAsync(normalizedIdentifier);
        if (driver == null)
        {
            var errorMessage = dto.Channel == CommunicationChannel.Email
                ? "auth.email_not_found"
                : "auth.phone_not_found";
            throw new NotFoundException(errorMessage);
        }
        return await SendPasswordResetAsync(driver, dto);
    }

    private async Task<bool> SendPasswordResetAsync(Domain.Common.Entities.Driver driver, DriverForgetPasswordDto dto)
    {
        var resetCode = CodeGenerator.GenerateNumericCode(6);
        var resetToken = _passwordResetTokenService.GenerateResetToken(driver.Id, "driver", channel: dto.Channel);
        var normalizedIdentifier = (dto.Identifier ?? string.Empty).Trim().ToLower();
        await _passwordResetCacheService.StoreResetAttemptAsync(
            userType: "driver",
            identifier: normalizedIdentifier,
            userId: driver.Id,
            code: resetCode,
            channel: dto.Channel);
        if (dto.Channel == CommunicationChannel.Email)
        {
            await _notificationTransport.SendEmailAsync(
                driver.Email,
                driver.FullName,
                "Password Reset Request",
                FormatEmailMessage(resetCode, resetToken),
                isHtml: true,
                domain: NotificationDomain.Driver,
                modelId: driver.Id,
                modelType: "Driver");
        }
        else
        {
            await _notificationTransport.SendSmsAsync(driver.Phone, FormatSmsMessage(resetCode), NotificationDomain.Driver, driver.Id, "Driver");
        }
        return true;
    }


    private string FormatEmailMessage(string resetCode, string resetToken)
    {
        var resetUrl = _resetPasswordUrlService.GenerateResetPasswordUrl("driver", resetToken);
        return $@"
            <h2>Password Reset Request</h2>
            <p>Hello,</p>
            <p>You have requested to reset your password via EMAIL.</p>
            <p><strong>Reset Code: {resetCode}</strong></p>
            <p>Click the link below to reset your password:</p>
            <p><a href=""{resetUrl}"">Reset Password</a></p>
            <p>Both the code and link will expire in 24 hours.</p>
            <p>If you didn't request this, please ignore this email.</p>
        ";
    }

    private static string FormatSmsMessage(string resetCode) => $"Your verification code is {resetCode}";
}

