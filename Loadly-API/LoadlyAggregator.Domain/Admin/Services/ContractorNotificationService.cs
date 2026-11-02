using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Services;

public class ContractorNotificationService : IContractorNotificationService
{
    private readonly INotificationTransport _notificationTransport;

    public ContractorNotificationService(INotificationTransport notificationTransport)
    {
        _notificationTransport = notificationTransport;
    }

    public async Task SendContractorWelcomeEmailIfEmailChannelAsync(Common.Entities.Contractor contractor, string resetPasswordUrl)
    {
        if (string.IsNullOrWhiteSpace(contractor.Email))
        {
            return;
        }

        var subject = "Welcome to Loadly Aggregator - Set Your Contractor Account Password";
        var body = $@"
            <h2>Welcome to Loadly Aggregator!</h2>
            <p>Hello {contractor.FullName},</p>
            <p>Your <strong>Contractor</strong> account has been created successfully.</p>
            <p><strong>Next Steps:</strong></p>
            <p>To activate your account and set your password, please click the button below:</p>
            <p style=""text-align: center; margin: 30px 0;"">
                <a href=""{resetPasswordUrl}"" style=""display: inline-block; padding: 12px 30px; background-color: #007bff; color: #ffffff; text-decoration: none; border-radius: 5px; font-weight: bold;"">Set Your Password</a>
            </p>
            <p><strong>What happens next?</strong></p>
            <ul>
                <li>You'll be redirected to a secure page to set your password</li>
                <li>Your email will be verified automatically</li>
                <li>You can then log in to your contractor account</li>
            </ul>
            <p><strong>Important:</strong> This link will expire in 24 hours. If the link expires, please contact support.</p>
            <p>If you didn't request this account, please contact support immediately.</p>
        ";

        await _notificationTransport.SendEmailAsync(
            contractor.Email,
            contractor.FullName,
            subject,
            body,
            isHtml: true,
            domain: NotificationDomain.Contractor,
            modelId: contractor.Id,
            modelType: "Contractor");
    }

    public async Task SendContractorApprovedSmsAsync(Common.Entities.Contractor contractor)
    {
        var message = "Welcome to Loadly! Your account has been approved. Open the app to set your password or log in with OTP.";

        await _notificationTransport.SendSmsAsync(
            contractor.Phone,
            message,
            NotificationDomain.Contractor,
            contractor.Id,
            "Contractor");
    }
}
