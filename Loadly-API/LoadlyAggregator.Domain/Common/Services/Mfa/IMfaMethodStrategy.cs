namespace LoadlyAggregator.Domain.Common.Services.Mfa;

public interface IMfaMethodStrategy
{
    string Method { get; }
    Task SendCodeAsync(string recipient, string recipientName, string code, string domain = "default", int expirationMinutes = 30, long? modelId = null, string? modelType = null);
}
