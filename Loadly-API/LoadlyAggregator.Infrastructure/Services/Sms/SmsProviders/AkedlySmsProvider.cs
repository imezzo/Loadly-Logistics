using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.Infrastructure.Services.Sms.SmsProviders;




public class AkedlySmsProvider : ISmsProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AkedlySmsProvider> _logger;
    private readonly string _apiKey;
    private readonly string _pipelineId;
    private readonly string _apiUrl;
    public AkedlySmsProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<AkedlySmsProvider> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _apiKey = _configuration["Sms:Akedly:ApiKey"]
            ?? throw new InvalidOperationException("Sms:Akedly:ApiKey not configured");
        _pipelineId = _configuration["Sms:Akedly:PipelineId"]
            ?? throw new InvalidOperationException("Sms:Akedly:PipelineId not configured");
        _apiUrl = _configuration["Sms:Akedly:ApiUrl"] ?? "https://api.akedly.io/api/v1";

        _httpClient.BaseAddress = new Uri(_apiUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
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
            var normalizedPhone = NormalizePhoneNumber(phoneNumber);

            var payload = new
            {
                pipelineID = _pipelineId,
                phone = normalizedPhone,
                message = message
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/transactions/initiate", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            captureResponse?.Invoke(JsonSerializer.Serialize(new { statusCode = (int)response.StatusCode, body = responseContent }));

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "SMS sent successfully via Akedly to {PhoneNumber}. Response: {Response}",
                    normalizedPhone,
                    responseContent);
                return true;
            }
            else
            {
                _logger.LogError(
                    "Failed to send SMS via Akedly to {PhoneNumber}. Status: {Status}, Response: {Response}",
                    normalizedPhone,
                    response.StatusCode,
                    responseContent);
                return false;
            }
        }
        catch (Exception ex)
        {
            captureResponse?.Invoke(JsonSerializer.Serialize(new { error = ex.Message, type = ex.GetType().Name }));
            _logger.LogError(ex, "Exception occurred while sending SMS via Akedly to {PhoneNumber}", phoneNumber);
            return false;
        }
    }



    private static string NormalizePhoneNumber(string phoneNumber)
    {

        var normalized = new StringBuilder();
        foreach (var c in phoneNumber)
        {
            if (char.IsDigit(c) || c == '+')
            {
                normalized.Append(c);
            }
        }
        var result = normalized.ToString();


        if (!result.StartsWith("+"))
        {



        }
        return result;
    }
}
