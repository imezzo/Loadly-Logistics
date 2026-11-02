using LoadlyAggregator.Domain.Common.Extensions;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.Domain.Common.Services.Mfa;

public class EmailMfaStrategy : IMfaMethodStrategy
{
    private readonly INotificationTransport _notificationTransport;
    public EmailMfaStrategy(INotificationTransport notificationTransport)
    {
        _notificationTransport = notificationTransport;
    }
    public string Method => "email";
    public async Task SendCodeAsync(string recipient, string recipientName, string code, string domain = "default", int expirationMinutes = 30, long? modelId = null, string? modelType = null)
    {
        var emailBody = $@"
            <h2>MFA Login Verification Code</h2>
            <p>Hello {recipientName},</p>
            <p>A login attempt is being made to your account. Use the verification code below to complete your login:</p>
            <p style=""font-size: 32px; font-weight: bold; letter-spacing: 8px; text-align: center; padding: 20px; background-color: #f0f0f0; border-radius: 5px; margin: 30px 0; font-family: monospace;"">{code}</p>
            <p><strong>What happens next?</strong></p>
            <ul>
                <li>Enter this code on the login verification page</li>
                <li>Your login will be completed once verified</li>
                <li>If you didn't attempt to login, please secure your account immediately</li>
            </ul>
            <p><strong>Important:</strong> This code will expire in {expirationMinutes} minutes.</p>
            <p>If you didn't request this code, please change your password immediately and contact support.</p>
        ";
        await _notificationTransport.SendEmailAsync(
            recipient,
            recipientName,
            "Loadly Aggregator - MFA Login Verification Code",
            emailBody,
            isHtml: true,
            domain: domain.ToNotificationDomain(),
            modelId: modelId,
            modelType: modelType);
    }
}
