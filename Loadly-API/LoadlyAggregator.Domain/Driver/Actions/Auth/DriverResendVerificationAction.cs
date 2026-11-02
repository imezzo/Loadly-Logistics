using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Utilities;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Driver.Actions.Auth;





public class DriverResendVerificationAction : IAction<DriverResendVerificationDto, object>
{
    private readonly IDriverRepository _driverRepository;
    private readonly INotificationTransport _notificationTransport;
    private readonly IMfaCacheService _mfaCacheService;

    public DriverResendVerificationAction(
        IDriverRepository driverRepository,
        INotificationTransport notificationTransport,
        IMfaCacheService mfaCacheService)
    {
        _driverRepository = driverRepository;
        _notificationTransport = notificationTransport;
        _mfaCacheService = mfaCacheService;
    }

    public async Task<object> ExecuteAsync(DriverResendVerificationDto dto)
    {
        var driver = await _driverRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(driver, "Driver");
        _ = dto.Channel == CommunicationChannel.Email || dto.Channel == CommunicationChannel.Sms
            ? true
            : throw new ValidationException("validation.verification_channel_invalid");
        _ = dto.Channel == CommunicationChannel.Email && string.IsNullOrWhiteSpace(driver!.Email)
            ? throw new ValidationException("auth.no_email_to_verify")
            : true;
        _ = dto.Channel == CommunicationChannel.Email && driver.EmailVerified
            ? throw new ValidationException("validation.email_already_verified")
            : true;
        _ = dto.Channel == CommunicationChannel.Sms && driver.PhoneVerified
            ? throw new ValidationException("validation.phone_already_verified")
            : true;
        var code = CodeGenerator.GenerateNumericCode(6);
        await _mfaCacheService.StoreMfaCodeAsync(
            userId: driver!.Id,
            userType: "driver",
            code: code,
            channel: dto.Channel);
        if (dto.Channel == CommunicationChannel.Email)
        {
            await _notificationTransport.SendEmailAsync(
                driver.Email,
                driver.FullName,
                "Loadly Aggregator - Email Verification Code",
                FormatEmailVerificationMessage(driver.FullName, code),
                isHtml: true,
                domain: NotificationDomain.Driver,
                modelId: driver.Id,
                modelType: "Driver");
        }
        else
        {
            var smsSent = await _notificationTransport.SendSmsAsync(
                driver.Phone,
                $"Loadly phone verification code: {code}\nUse this code to verify your phone number.\nValid for 30 minutes.\nDidn't request? Ignore this message.",
                NotificationDomain.Driver,
                driver.Id,
                "Driver");
            if (!smsSent)
            {
                throw new LogicalException("auth.unable_to_send_verification_code");
            }
        }
        return new { };
    }

    private static string FormatEmailVerificationMessage(string fullName, string code) => $@"
                <h2>Email Verification Code</h2>
                <p>Hello {fullName},</p>
                <p>You requested a code to verify your email address. Use the verification code below:</p>
                <p style=""font-size: 32px; font-weight: bold; letter-spacing: 8px; text-align: center; padding: 20px; background-color: #f0f0f0; border-radius: 5px; margin: 30px 0; font-family: monospace;"">{code}</p>
                <p><strong>What happens next?</strong></p>
                <ul>
                    <li>Enter this code on the email verification page</li>
                    <li>Your email will be verified once the code is confirmed</li>
                    <li>You can then access all features that require email verification</li>
                </ul>
                <p><strong>Important:</strong> This code will expire in 30 minutes.</p>
                <p>If you didn't request this code, please ignore this email or contact support if you're concerned about your account security.</p>
            ";
}

