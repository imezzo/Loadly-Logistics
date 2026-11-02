namespace LoadlyAggregator.API.Resources.Common;

public class TripDocumentDownloadResource
{
    public string Url { get; set; } = string.Empty;

    public static TripDocumentDownloadResource FromUrl(string url) => new()
    {
        Url = url
    };
}
