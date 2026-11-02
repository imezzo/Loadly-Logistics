namespace LoadlyAggregator.Infrastructure.Options;

public class OpenAiOptions
{
    public const string SectionName = "OpenAI";

    public string ApiKey { get; set; } = string.Empty;
    public string? Organization { get; set; }
    public string Model { get; set; } = "gpt-4o";
}
