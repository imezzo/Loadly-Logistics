using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Utilities;
using LoadlyAggregator.Domain.Company.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Auth;






public class CompanyForgetPasswordAction : IAction<CompanyForgetPasswordDto, bool>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly IPasswordResetCacheService _passwordResetCacheService;
    private readonly IResetPasswordUrlService _resetPasswordUrlService;
    private readonly INotificationTransport _notificationTransport;

    public CompanyForgetPasswordAction(
        ICompanyRepository companyRepository,
        IPasswordResetTokenService passwordResetTokenService,
        IPasswordResetCacheService passwordResetCacheService,
        IResetPasswordUrlService resetPasswordUrlService,
        INotificationTransport notificationTransport)
    {
        _companyRepository = companyRepository;
        _passwordResetTokenService = passwordResetTokenService;
        _passwordResetCacheService = passwordResetCacheService;
        _resetPasswordUrlService = resetPasswordUrlService;
        _notificationTransport = notificationTransport;
    }

    public async Task<bool> ExecuteAsync(CompanyForgetPasswordDto dto)
    {
        var normalizedIdentifier = dto.Identifier.Trim().ToLower();
        _ = await _passwordResetCacheService.IsRateLimitedAsync("company", normalizedIdentifier)
            ? throw new TooManyRequestsException("auth.too_many_password_reset_requests")
            : true;
        var company = dto.Channel == CommunicationChannel.Email
            ? await _companyRepository.GetByEmailAsync(normalizedIdentifier)
            : await _companyRepository.GetByPhoneAsync(dto.Identifier);
        return company == null
            ? true
            : await SendPasswordResetAsync(company, dto);
    }

    private async Task<bool> SendPasswordResetAsync(Domain.Common.Entities.Company company, CompanyForgetPasswordDto dto)
    {
        var resetCode = CodeGenerator.GenerateAlphanumericCode(6);
        var resetToken = _passwordResetTokenService.GenerateResetToken(company.Id, "company", channel: dto.Channel);
        await _passwordResetCacheService.StoreResetAttemptAsync(
            userType: "company",
            identifier: dto.Identifier,
            userId: company.Id,
            code: resetCode,
            channel: dto.Channel);
        if (dto.Channel == CommunicationChannel.Email)
        {
            await _notificationTransport.SendEmailAsync(
                company.Email,
                company.FullName,
                "Password Reset Request",
                FormatEmailMessage(resetCode, resetToken),
                isHtml: true,
                domain: NotificationDomain.Company,
                modelId: company.Id,
                modelType: "Company");
        }
        else
        {
            await _notificationTransport.SendSmsAsync(company.Phone, FormatSmsMessage(resetCode), NotificationDomain.Company, company.Id, "Company");
        }
        return true;
    }


    private string FormatEmailMessage(string resetCode, string resetToken)
    {
        var resetUrl = _resetPasswordUrlService.GenerateResetPasswordUrl("company", resetToken);
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

