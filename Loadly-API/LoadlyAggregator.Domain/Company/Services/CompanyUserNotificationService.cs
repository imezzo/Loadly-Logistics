using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Company.Services;

public class CompanyUserNotificationService : ICompanyUserNotificationService
{
    private readonly INotificationTransport _notificationTransport;

    public CompanyUserNotificationService(INotificationTransport notificationTransport)
    {
        _notificationTransport = notificationTransport;
    }

    public async Task SendWelcomeSetPasswordEmailAsync(Common.Entities.Company user, string resetPasswordUrl)
    {
        //if (user.MfaChannel != CommunicationChannel.Email)
        //{
        //    return;
        //}

        const string subject = "Welcome to Loadly Aggregator - Set Your Company User Account Password";
        string body = $@"
            <h2>Welcome to Loadly Aggregator!</h2>
            <p>Hello {user.FullName},</p>
            <p>Your <strong>Company User</strong> account has been created successfully.</p>
            <p><strong>Next Steps:</strong></p>
            <p>To activate your account and set your password, please click the button below:</p>
            <p style=""text-align: center; margin: 30px 0;"">
                <a href=""{resetPasswordUrl}"" style=""display: inline-block; padding: 12px 30px; background-color: #007bff; color: #ffffff; text-decoration: none; border-radius: 5px; font-weight: bold;"">Set Your Password</a>
            </p>
            <p><strong>What happens next?</strong></p>
            <ul>
                <li>You'll be redirected to a secure page to set your password</li>
                <li>Your email will be verified automatically</li>
                <li>You can then log in to your company user account</li>
            </ul>
            <p><strong>Important:</strong> This link will expire in 24 hours. If the link expires, please contact support.</p>
            <p>If you didn't request this account, please contact support immediately.</p>
        ";

        await _notificationTransport.SendEmailAsync(
            user.Email,
            user.FullName,
            subject,
            body,
            isHtml: true,
            domain: NotificationDomain.Company,
            modelId: user.Id,
            modelType: "Company",
            eventType: null);
    }
}
