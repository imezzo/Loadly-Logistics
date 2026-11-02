using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Options;

namespace LoadlyAggregator.Infrastructure.Services.Notification;

/// <summary>
/// Facade that delegates to <see cref="IEmailSender"/>, <see cref="ISmsSender"/>, <see cref="IPushSender"/>
/// for transactional, channel-specific sends.
/// </summary>
public class NotificationTransportService : INotificationTransport
{
    private readonly SmtpOptions _smtpOptions;
    private readonly IEmailSender _emailSender;
    private readonly ISmsSender _smsSender;
    private readonly IPushSender _pushSender;

    public NotificationTransportService(
        IOptions<SmtpOptions> smtpOptions,
        IEmailSender emailSender,
        ISmsSender smsSender,
        IPushSender pushSender)
    {
        _smtpOptions = smtpOptions.Value;
        _emailSender = emailSender;
        _smsSender = smsSender;
        _pushSender = pushSender;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null)
    {
        var fromEmail = _smtpOptions.FromEmail ?? throw new InvalidOperationException("SMTP FromEmail not configured");
        return await _emailSender.SendAsync(fromEmail, _smtpOptions.FromName, to, to, subject, body, isHtml, domain, modelId, modelType, eventType);
    }

    public async Task<bool> SendEmailAsync(string to, string toName, string subject, string body, bool isHtml = false, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null)
    {
        var fromEmail = _smtpOptions.FromEmail ?? throw new InvalidOperationException("SMTP FromEmail not configured");
        return await _emailSender.SendAsync(fromEmail, _smtpOptions.FromName, to, toName, subject, body, isHtml, domain, modelId, modelType, eventType);
    }

    public async Task<bool> SendEmailAsync(string from, string fromName, string to, string toName, string subject, string body, bool isHtml = false, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null) =>
        await _emailSender.SendAsync(from, fromName, to, toName, subject, body, isHtml, domain, modelId, modelType, eventType);

    public async Task<bool> SendSmsAsync(string phoneNumber, string message, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null) =>
        await _smsSender.SendAsync(phoneNumber, message, domain, modelId, modelType, eventType);

    public async Task<bool> SendPushNotificationAsync(string deviceToken, string title, string body, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null) =>
        await _pushSender.SendToDeviceAsync(deviceToken, title, body, null, null, null, domain, modelId, modelType, eventType);

    public async Task<bool> SendPushNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string>? data, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null) =>
        await _pushSender.SendToDeviceAsync(deviceToken, title, body, data, null, null, domain, modelId, modelType, eventType);

    public async Task<bool> SendPushNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string>? data, string? imageUrl, string? clickAction, NotificationDomain domain = NotificationDomain.Default, long? modelId = null, string? modelType = null, string? eventType = null) =>
        await _pushSender.SendToDeviceAsync(deviceToken, title, body, data, imageUrl, clickAction, domain, modelId, modelType, eventType);

    public async Task SendPushToUserAsync(long modelId, string modelType, string title, string body, Dictionary<string, string>? data = null, string? clickAction = null, NotificationDomain domain = NotificationDomain.Default, string? eventType = null, long? broadcastNotificationId = null) =>
        await _pushSender.SendToUserAsync(modelId, modelType, title, body, data, clickAction, domain, eventType, broadcastNotificationId);
}
