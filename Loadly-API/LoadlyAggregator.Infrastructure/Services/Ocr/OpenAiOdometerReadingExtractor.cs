using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Infrastructure.Options;

namespace LoadlyAggregator.Infrastructure.Services.Ocr;

public sealed class OpenAiOdometerReadingExtractor(
    IHttpClientFactory httpClientFactory,
    IOptions<OpenAiOptions> options) : IOdometerReadingExtractor
{
    public string Provider => ProviderName;

    private const string ProviderName = "openai";
    private const string SystemPrompt =
        "Extract odometer reading as JSON: odometer_reading (numeric), unit (km/miles), " +
        "is_digital (boolean), reading_quality (clear/partial/unclear), is_screen (boolean). " +
        "is_screen=true if photo of a screen (phone/TV/monitor), not physical dash. " +
        "Return the main total odometer only, not trip meter. Use null if missing. Return ONLY JSON.";

    private readonly OpenAiOptions _options = options.Value;

    public Task<OdometerReadingExtraction> ExtractAsync(
        byte[] imageBytes,
        string mimeType,
        CancellationToken cancellationToken = default)
    {
        var apiKey = _options.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenAI API key not configured");

        var dataUrl = $"data:{NormalizeMimeType(mimeType)};base64,{Convert.ToBase64String(imageBytes)}";
        return CallOpenAiAsync(apiKey, _options.Organization, _options.Model, dataUrl, cancellationToken);
    }

    private async Task<OdometerReadingExtraction> CallOpenAiAsync(
        string apiKey,
        string? organization,
        string model,
        string dataUrl,
        CancellationToken cancellationToken)
    {
        using var request = CreateRequest(apiKey, organization, model, dataUrl);
        try
        {
            var client = httpClientFactory.CreateClient("openai");
            using var response = await client.SendAsync(request, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"OpenAI failed: {(int)response.StatusCode} {body}");

            return MapResponse(JsonSerializer.Deserialize<OpenAiResponsesResponse>(body), model, body);
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("Request to OpenAI timed out.");
        }
    }

    private static HttpRequestMessage CreateRequest(string apiKey, string? organization, string model, string dataUrl)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "v1/responses")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    model,
                    instructions = SystemPrompt,
                    temperature = 0,
                    max_output_tokens = 150,
                    input = new object[]
                    {
                        new
                        {
                            role = "user",
                            content = new object[]
                            {
                                new { type = "input_text", text = "Extract odometer reading. Return ONLY JSON." },
                                new { type = "input_image", image_url = dataUrl, detail = "auto" }
                            }
                        }
                    }
                }),
                Encoding.UTF8,
                "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        if (!string.IsNullOrWhiteSpace(organization))
            request.Headers.TryAddWithoutValidation("OpenAI-Organization", organization);

        return request;
    }

    private static OdometerReadingExtraction MapResponse(
        OpenAiResponsesResponse? response,
        string model,
        string rawBody)
    {
        var content = ExtractOutputText(response);
        var payload = TryDeserialize(StripFences(content));
        return MapPayload(payload, response?.Usage?.TotalTokens, string.IsNullOrWhiteSpace(content) ? rawBody : content, model);
    }

    private static string ExtractOutputText(OpenAiResponsesResponse? response)
    {
        if (!string.IsNullOrWhiteSpace(response?.OutputText))
            return response.OutputText;

        return response?.Output?
            .Where(item => item.Type == "message")
            .SelectMany(item => item.Content ?? [])
            .Where(part => part.Type == "output_text" && !string.IsNullOrWhiteSpace(part.Text))
            .Select(part => part.Text!)
            .FirstOrDefault() ?? string.Empty;
    }

    private static string StripFences(string content) =>
        Regex.Replace(content, @"```json\s*|\s*```", string.Empty).Trim();

    private static LlmPayload? TryDeserialize(string content)
    {
        try
        {
            return JsonSerializer.Deserialize<LlmPayload>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }

    private static OdometerReadingExtraction MapPayload(LlmPayload? payload, int? totalTokens, string rawContent, string model)
    {
        if (payload is null)
            return new OdometerReadingExtraction(null, null, null, null, false, totalTokens, rawContent, ProviderName, model);

        if (payload.IsScreen == true)
            return new OdometerReadingExtraction(
                null, payload.Unit, payload.IsDigital, payload.ReadingQuality, true, totalTokens, rawContent, ProviderName, model);

        return new OdometerReadingExtraction(
            ToIntegerReading(payload.OdometerReading),
            payload.Unit,
            payload.IsDigital,
            payload.ReadingQuality,
            false,
            totalTokens,
            rawContent,
            ProviderName,
            model);
    }

    private static int? ToIntegerReading(double? reading) =>
        reading switch
        {
            null => null,
            < 0 => null,
            _ => (int)Math.Round(reading.Value, MidpointRounding.AwayFromZero)
        };

    private static string NormalizeMimeType(string mimeType) =>
        string.IsNullOrWhiteSpace(mimeType) ? "image/jpeg" : mimeType;

    private sealed class OpenAiResponsesResponse
    {
        [JsonPropertyName("output_text")] public string? OutputText { get; set; }
        [JsonPropertyName("usage")] public UsageInfo? Usage { get; set; }
        [JsonPropertyName("output")] public List<OutputItem>? Output { get; set; }
    }

    private sealed class OutputItem
    {
        [JsonPropertyName("type")] public string? Type { get; set; }
        [JsonPropertyName("content")] public List<OutputContent>? Content { get; set; }
    }

    private sealed class OutputContent
    {
        [JsonPropertyName("type")] public string? Type { get; set; }
        [JsonPropertyName("text")] public string? Text { get; set; }
    }

    private sealed class UsageInfo
    {
        [JsonPropertyName("total_tokens")] public int TotalTokens { get; set; }
    }

    private sealed class LlmPayload
    {
        [JsonPropertyName("odometer_reading")] public double? OdometerReading { get; set; }
        [JsonPropertyName("unit")] public string? Unit { get; set; }
        [JsonPropertyName("is_digital")] public bool? IsDigital { get; set; }
        [JsonPropertyName("reading_quality")] public string? ReadingQuality { get; set; }
        [JsonPropertyName("is_screen")] public bool? IsScreen { get; set; }
    }
}
