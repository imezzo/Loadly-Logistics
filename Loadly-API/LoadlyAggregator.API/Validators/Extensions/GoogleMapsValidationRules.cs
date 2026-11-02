using FluentValidation;

namespace LoadlyAggregator.API.Validators.Extensions;

public static class GoogleMapsValidationRules
{
    public static IRuleBuilderOptions<T, string?> IsOptionalGoogleMapsUrl<T>(this IRuleBuilder<T, string?> ruleBuilder) =>
        ruleBuilder
            .Must(IsValidOrEmpty)
            .WithMessage("maps.invalid_google_maps_url");

    public static bool IsValidGoogleMapsUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        var u = url.Trim();
        return u.StartsWith("https://www.google.com/maps", StringComparison.OrdinalIgnoreCase)
            || u.StartsWith("https://maps.google.com", StringComparison.OrdinalIgnoreCase)
            || u.StartsWith("https://goo.gl/maps", StringComparison.OrdinalIgnoreCase)
            || u.StartsWith("https://maps.app.goo.gl", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsValidOrEmpty(string? url) =>
        string.IsNullOrWhiteSpace(url) || IsValidGoogleMapsUrl(url);
}
