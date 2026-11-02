namespace LoadlyAggregator.Infrastructure.Services.Maps;

/// <summary>
/// Resolves Google Maps short URLs (maps.app.goo.gl, goo.gl/maps) to their final destination.
/// </summary>
internal static class ShortUrlResolver
{
    private const int MaxRedirects = 5;
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);
    private static readonly string DesktopUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
    private static readonly string MobileUserAgent = "Mozilla/5.0 (Linux; Android 10; SM-G973F) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Mobile Safari/537.36";

    public static async Task<string?> ResolveAsync(string shortUrl, bool isMobile, CancellationToken cancellationToken = default)
    {
        using var handler = new HttpClientHandler
        {
            AllowAutoRedirect = true,
            MaxAutomaticRedirections = MaxRedirects
        };
        using var client = new HttpClient(handler) { Timeout = Timeout };
        var userAgent = isMobile ? MobileUserAgent : DesktopUserAgent;
        client.DefaultRequestHeaders.Add("User-Agent", userAgent);
        using var request = new HttpRequestMessage(HttpMethod.Get, shortUrl);
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        return response.RequestMessage?.RequestUri?.AbsoluteUri ?? shortUrl;
    }
}
