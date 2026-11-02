using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Utilities;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Contractor.Actions.Auth;






public class ContractorForgetPasswordAction : IAction<ContractorForgetPasswordDto, bool>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly IPasswordResetCacheService _passwordResetCacheService;
    private readonly IResetPasswordUrlService _resetPasswordUrlService;
    private readonly INotificationTransport _notificationTransport;

    public ContractorForgetPasswordAction(
        IContractorRepository contractorRepository,
        IPasswordResetTokenService passwordResetTokenService,
        IPasswordResetCacheService passwordResetCacheService,
        IResetPasswordUrlService resetPasswordUrlService,
        INotificationTransport notificationTransport)
    {
        _contractorRepository = contractorRepository;
        _passwordResetTokenService = passwordResetTokenService;
        _passwordResetCacheService = passwordResetCacheService;
        _resetPasswordUrlService = resetPasswordUrlService;
        _notificationTransport = notificationTransport;
    }

    public async Task<bool> ExecuteAsync(ContractorForgetPasswordDto dto)
    {
        var normalizedIdentifier = dto.Identifier.Trim().ToLower();
        _ = await _passwordResetCacheService.IsRateLimitedAsync("contractor", normalizedIdentifier)
            ? throw new TooManyRequestsException("auth.too_many_password_reset_requests")
            : true;
        var contractor = dto.Channel == CommunicationChannel.Email
            ? await _contractorRepository.GetByEmailAsync(normalizedIdentifier)
            : await _contractorRepository.GetByPhoneAsync(dto.Identifier);
        return contractor == null
            ? true
            : await SendPasswordResetAsync(contractor, dto);
    }

    private async Task<bool> SendPasswordResetAsync(Domain.Common.Entities.Contractor contractor, ContractorForgetPasswordDto dto)
    {
        var resetCode = CodeGenerator.GenerateAlphanumericCode(6);
        var resetToken = _passwordResetTokenService.GenerateResetToken(contractor.Id, "contractor", channel: dto.Channel);
        await _passwordResetCacheService.StoreResetAttemptAsync(
            userType: "contractor",
            identifier: dto.Identifier,
            userId: contractor.Id,
            code: resetCode,
            channel: dto.Channel);
        if (dto.Channel == CommunicationChannel.Email)
        {
            await _notificationTransport.SendEmailAsync(
                contractor.Email,
                contractor.FullName,
                "Password Reset Request",
                FormatEmailMessage(resetCode, resetToken),
                isHtml: true,
                domain: NotificationDomain.Contractor,
                modelId: contractor.Id,
                modelType: "Contractor");
        }
        else
        {
            await _notificationTransport.SendSmsAsync(contractor.Phone, FormatSmsMessage(resetCode), NotificationDomain.Contractor, contractor.Id, "Contractor");
        }
        return true;
    }


    private string FormatEmailMessage(string resetCode, string resetToken)
    {
        var resetUrl = _resetPasswordUrlService.GenerateResetPasswordUrl("contractor", resetToken);
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

    private string FormatSmsMessage(string resetCode)
    {


        return $"Your verification code is {resetCode}";
    }
}

