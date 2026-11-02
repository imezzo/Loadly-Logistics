using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Services.Sms.SmsProviders;






public class UltraMsgSmsProvider : ISmsProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UltraMsgSmsProvider> _logger;
    private readonly string _token;
    private readonly string _instanceId;
    private readonly string _apiUrl;

    public UltraMsgSmsProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<UltraMsgSmsProvider> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        var ultraMsgSection = configuration.GetSection("Sms").GetSection("UltraMsg");
        _token = ultraMsgSection["Token"]
            ?? throw new InvalidOperationException("Sms:UltraMsg:Token not configured. Add Token and InstanceId under Sms:UltraMsg in appsettings.json.");
        _instanceId = ultraMsgSection["InstanceId"]
            ?? throw new InvalidOperationException("Sms:UltraMsg:InstanceId not configured. Add Token and InstanceId under Sms:UltraMsg in appsettings.json.");
        _apiUrl = $"https://api.ultramsg.com/{_instanceId}/messages/chat";

        // Don't set BaseAddress - use full URL directly to match curl behavior
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message, Action<string?>? captureResponse = null)
    {
        return await SendSmsAsync(phoneNumber, message, null, captureResponse);
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message, string? senderId, Action<string?>? captureResponse = null)
    {
        try
        {
            var response = await _httpClient.PostAsync(
                _apiUrl,
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "token", _token },
                    { "to", NormalizePhoneNumber(phoneNumber) },
                    { "body", message },
                    { "priority", "10" }
                }));

            var responseContent = await response.Content.ReadAsStringAsync();
            captureResponse?.Invoke(JsonSerializer.Serialize(new { statusCode = (int)response.StatusCode, body = responseContent }));

            return response.IsSuccessStatusCode
                ? HandleSuccessResponse(responseContent, NormalizePhoneNumber(phoneNumber))
                : HandleErrorResponse(response.StatusCode, responseContent, NormalizePhoneNumber(phoneNumber));
        }
        catch (Exception ex)
        {
            captureResponse?.Invoke(JsonSerializer.Serialize(new { error = ex.Message, type = ex.GetType().Name }));
            _logger.LogError(ex, "Exception occurred while sending WhatsApp via UltraMsg to {PhoneNumber}", phoneNumber);
            return false;
        }
    }

    private bool HandleSuccessResponse(string responseContent, string normalizedPhone)
    {
        var root = JsonDocument.Parse(responseContent).RootElement;

        return root.TryGetProperty("error", out var errorElement)
            ? LogErrorAndReturnFalse(errorElement.GetString() ?? "Unknown error", normalizedPhone)
            : root.TryGetProperty("sent", out var sentElement) && IsSentTrue(sentElement)
                ? LogSuccessAndReturnTrue(root, normalizedPhone)
                : LogWarningAndReturnFalse(responseContent, normalizedPhone);
    }

    private static bool IsSentTrue(JsonElement sentElement) =>
        sentElement.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.String => sentElement.GetString()?.ToLower() == "true",
            _ => false
        };

    private bool LogErrorAndReturnFalse(string errorMessage, string normalizedPhone)
    {
        _logger.LogError("UltraMsg WhatsApp failed for {PhoneNumber}. Error: {ErrorMessage}", normalizedPhone, errorMessage);
        return false;
    }

    private bool LogSuccessAndReturnTrue(JsonElement root, string normalizedPhone)
    {
        var messageId = root.TryGetProperty("id", out var idElement)
            ? idElement.ValueKind switch
            {
                JsonValueKind.String => idElement.GetString() ?? "unknown",
                JsonValueKind.Number => idElement.GetInt64().ToString(),
                _ => "unknown"
            }
            : "unknown";

        _logger.LogInformation("WhatsApp message sent successfully via UltraMsg to {PhoneNumber}. MessageID: {MessageId}", normalizedPhone, messageId);
        return true;
    }

    private bool LogWarningAndReturnFalse(string responseContent, string normalizedPhone)
    {
        _logger.LogWarning("UltraMsg response indicates failure for {PhoneNumber}. Response: {Response}", normalizedPhone, responseContent);
        return false;
    }

    private bool HandleErrorResponse(System.Net.HttpStatusCode statusCode, string responseContent, string normalizedPhone)
    {
        _logger.LogError("HTTP error from UltraMsg for {PhoneNumber}. Status: {Status}, Response: {Response}", normalizedPhone, statusCode, responseContent);
        return false;
    }







    private static string NormalizePhoneNumber(string phoneNumber)
    {
        var result = new string(phoneNumber.Where(c => char.IsDigit(c) || c == '+').ToArray()).TrimStart('0');
        return result.StartsWith("+") ? result : result.StartsWith("20") ? "+" + result : "+20" + result;
    }
}

