using LoadlyAggregator.Domain.Common.Extensions;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Common.Services.Mfa;

public class SmsMfaStrategy : IMfaMethodStrategy
{
    private readonly INotificationTransport _notificationTransport;
    public SmsMfaStrategy(INotificationTransport notificationTransport)
    {
        _notificationTransport = notificationTransport;
    }
    public string Method => "sms";
    public async Task SendCodeAsync(string recipient, string recipientName, string code, string domain = "default", int expirationMinutes = 30, long? modelId = null, string? modelType = null)
    {
        var message = $"Loadly MFA login code: {code}\nUse this code to complete your login.\nValid for {expirationMinutes} minutes.\nDidn't request? Ignore this message.";
        var sent = await _notificationTransport.SendSmsAsync(recipient, message, domain.ToNotificationDomain(), modelId, modelType);
        if (!sent)
        {
            throw new LogicalException("auth.unable_to_send_verification_code");
        }
    }
}
