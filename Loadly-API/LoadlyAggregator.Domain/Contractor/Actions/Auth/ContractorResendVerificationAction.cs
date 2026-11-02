using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Utilities;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Contractor.Actions.Auth;





public class ContractorResendVerificationAction : IAction<ContractorResendVerificationDto, object>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly INotificationTransport _notificationTransport;
    private readonly IMfaCacheService _mfaCacheService;

    public ContractorResendVerificationAction(
        IContractorRepository contractorRepository,
        INotificationTransport notificationTransport,
        IMfaCacheService mfaCacheService)
    {
        _contractorRepository = contractorRepository;
        _notificationTransport = notificationTransport;
        _mfaCacheService = mfaCacheService;
    }

    public async Task<object> ExecuteAsync(ContractorResendVerificationDto dto)
    {
        var contractor = await _contractorRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(contractor, "Contractor");
        _ = dto.Channel == CommunicationChannel.Email || dto.Channel == CommunicationChannel.Sms
            ? true
            : throw new ValidationException("validation.verification_channel_invalid");
        _ = dto.Channel == CommunicationChannel.Email && contractor!.EmailVerified
            ? throw new ValidationException("validation.email_already_verified")
            : true;
        _ = dto.Channel == CommunicationChannel.Sms && contractor!.PhoneVerified
            ? throw new ValidationException("validation.phone_already_verified")
            : true;
        var code = CodeGenerator.GenerateNumericCode(6);
        await _mfaCacheService.StoreMfaCodeAsync(
            userId: contractor!.Id,
            userType: "contractor",
            code: code,
            channel: dto.Channel);
        if (dto.Channel == CommunicationChannel.Email)
        {
            await _notificationTransport.SendEmailAsync(
                contractor.Email,
                contractor.FullName,
                "Loadly Aggregator - Email Verification Code",
                FormatEmailVerificationMessage(contractor.FullName, code),
                isHtml: true,
                domain: NotificationDomain.Contractor,
                modelId: contractor.Id,
                modelType: "Contractor");
        }
        else
        {
            var smsSent = await _notificationTransport.SendSmsAsync(
                contractor.Phone,
                $"Loadly phone verification code: {code}\nUse this code to verify your phone number.\nValid for 30 minutes.\nDidn't request? Ignore this message.",
                NotificationDomain.Contractor,
                contractor.Id,
                "contractor");
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

