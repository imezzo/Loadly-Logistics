namespace LoadlyAggregator.Infrastructure.Options;

public class GoogleMapsOptions
{
    public const string SectionName = "GoogleMaps";

    public string ApiKey { get; set; } = string.Empty;

    /// <summary>ISO 3166-1 alpha-2 country code for geocode biasing (e.g. EG).</summary>
    public string DefaultCountryCode { get; set; } = "EG";
}
