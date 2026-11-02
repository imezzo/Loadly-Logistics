using FluentValidation;
using LoadlyAggregator.Domain.Company.DTOs.Maps;

namespace LoadlyAggregator.API.Requests.Company.Maps;

public class ExtractPlaceFromGoogleMapsRequest
{
    public string Url { get; set; } = string.Empty;

    public ExtractPlaceFromGoogleMapsDto FromRequest() => new()
    {
        GoogleMapsUrl = Url
    };
}

public class ExtractPlaceFromGoogleMapsRequestValidator : AbstractValidator<ExtractPlaceFromGoogleMapsRequest>
{
    public ExtractPlaceFromGoogleMapsRequestValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("maps.google_maps_url_required")
            .Must(BeValidGoogleMapsUrl).WithMessage("maps.invalid_google_maps_url");
    }

    private static bool BeValidGoogleMapsUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        var u = url.Trim();
        return u.StartsWith("https://www.google.com/maps", StringComparison.OrdinalIgnoreCase)
            || u.StartsWith("https://maps.google.com", StringComparison.OrdinalIgnoreCase)
            || u.StartsWith("https://goo.gl/maps", StringComparison.OrdinalIgnoreCase)
            || u.StartsWith("https://maps.app.goo.gl", StringComparison.OrdinalIgnoreCase);
    }
}
