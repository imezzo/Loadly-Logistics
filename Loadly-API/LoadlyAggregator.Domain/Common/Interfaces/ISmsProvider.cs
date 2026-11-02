namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ISmsProvider
{
    Task<bool> SendSmsAsync(string phoneNumber, string message, Action<string?>? captureResponse = null);
    Task<bool> SendSmsAsync(string phoneNumber, string message, string? senderId, Action<string?>? captureResponse = null);
}
