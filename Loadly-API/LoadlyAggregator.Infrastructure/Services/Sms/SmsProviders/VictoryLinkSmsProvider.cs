using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Services.Sms.SmsProviders;





public class VictoryLinkSmsProvider : ISmsProvider
{
    #region Default values (used when appsettings Sms:VictoryLink is missing or keys are not set)

    private const string DefaultApiUrl = "https://app.community-ads.com/SendSMSAPI/api/SMSSender/SendSMS";
    private const string DefaultSenderName = "fuelin";
    private const string DefaultUsername = "Fuelin";
    private const string DefaultPassword = "zUbH^A-^C{";

    #endregion

    private readonly HttpClient _httpClient;
    private readonly ILogger<VictoryLinkSmsProvider> _logger;
    private readonly string _username;
    private readonly string _password;
    private readonly string _senderName;
    private readonly string _apiUrl;
    private readonly string _endpointPath;

    public VictoryLinkSmsProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<VictoryLinkSmsProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        var section = configuration.GetSection("Sms:VictoryLink");

        _username = GetOrDefault(section["Username"], DefaultUsername);
        _password = GetOrDefault(section["Password"], DefaultPassword);
        _senderName = GetOrDefault(section["SenderName"], DefaultSenderName);
        _apiUrl = GetOrDefault(section["ApiUrl"], DefaultApiUrl);

        // Extract base URL and endpoint path
        var uri = new Uri(_apiUrl);
        var baseUrl = $"{uri.Scheme}://{uri.Host}{(uri.Port != 80 && uri.Port != 443 && uri.Port != -1 ? $":{uri.Port}" : "")}";
        _endpointPath = uri.AbsolutePath;

        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    private static string GetOrDefault(string? value, string defaultValue) =>
        string.IsNullOrWhiteSpace(value) ? defaultValue : value;

    public async Task<bool> SendSmsAsync(string phoneNumber, string message, Action<string?>? captureResponse = null)
    {
        return await SendSmsAsync(phoneNumber, message, null, captureResponse);
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message, string? senderId, Action<string?>? captureResponse = null)
    {
        try
        {
            var normalizedPhone = NormalizePhoneNumber(phoneNumber);
            var smsId = Guid.NewGuid().ToString();
            var campaignId = $"indrive_fuel_{DateTime.UtcNow:yyyyMMdd}";

            var payload = new Dictionary<string, string>
            {
                { "UserName", _username },
                { "Password", _password },
                { "SMSText", message },
                { "SMSLang", "a" },
                { "SMSSender", senderId ?? _senderName },
                { "SMSReceiver", normalizedPhone },
                { "SMSID", smsId },
                { "CampaignID", campaignId }
            };

            var formContent = new FormUrlEncodedContent(payload);
            var response = await _httpClient.PostAsync(_endpointPath, formContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            captureResponse?.Invoke(BuildResponseJson((int)response.StatusCode, responseContent));

            if (response.IsSuccessStatusCode)
            {
                if (int.TryParse(responseContent.Trim(), out var statusCode))
                {
                    if (statusCode == 0)
                    {
                        _logger.LogInformation(
                            "SMS sent successfully via VictoryLink to {PhoneNumber}. SMSID: {SmsId}",
                            normalizedPhone,
                            smsId);
                        return true;
                    }
                    else
                    {
                        var errorMessage = GetErrorMessage(statusCode);
                        _logger.LogError(
                            "VictoryLink SMS failed for {PhoneNumber}. Status Code: {StatusCode}, Error: {ErrorMessage}",
                            normalizedPhone,
                            statusCode,
                            errorMessage);
                        return false;
                    }
                }
                else
                {
                    _logger.LogError(
                        "Invalid response from VictoryLink for {PhoneNumber}. Response: {Response}",
                        normalizedPhone,
                        responseContent);
                    return false;
                }
            }
            else
            {
                _logger.LogError(
                    "HTTP error from VictoryLink for {PhoneNumber}. Status: {Status}, Response: {Response}",
                    normalizedPhone,
                    response.StatusCode,
                    responseContent);
                return false;
            }
        }
        catch (Exception ex)
        {
            captureResponse?.Invoke(JsonSerializer.Serialize(new { error = ex.Message, type = ex.GetType().Name }));
            _logger.LogError(ex, "Exception occurred while sending SMS via VictoryLink to {PhoneNumber}", phoneNumber);
            return false;
        }
    }

    private static string BuildResponseJson(int statusCode, string body) =>
        JsonSerializer.Serialize(new { statusCode, body });






    private static string NormalizePhoneNumber(string phoneNumber)
    {

        var normalized = new StringBuilder();
        foreach (var c in phoneNumber)
        {
            if (char.IsDigit(c))
            {
                normalized.Append(c);
            }
        }

        var result = normalized.ToString();


        result = result.TrimStart('0');

        return result;
    }




    private static string GetErrorMessage(int statusCode)
    {
        return statusCode switch
        {
            0 => "Success",
            -1 => "Invalid credentials",
            -2 => "Invalid account IP",
            -3 => "Number is blacklisted",
            -5 => "Out of credit",
            -12 => "SMS text is empty",
            -13 => "Invalid sender with connection",
            -18 => "Invalid phone number",
            -29 => "Empty username or password",
            -30 => "Invalid sender name",
            _ => $"Unknown error code: {statusCode}"
        };
    }
}

