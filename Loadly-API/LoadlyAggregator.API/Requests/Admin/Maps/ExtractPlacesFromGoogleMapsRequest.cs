using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Maps;

namespace LoadlyAggregator.API.Requests.Admin.Maps;

public class ExtractPlacesFromGoogleMapsRequest
{
    public List<string> Urls { get; set; } = [];

    public ExtractPlacesFromGoogleMapsDto FromRequest() => new()
    {
        GoogleMapsUrls = Urls
    };
}

public class ExtractPlacesFromGoogleMapsRequestValidator : AbstractValidator<ExtractPlacesFromGoogleMapsRequest>
{
    public ExtractPlacesFromGoogleMapsRequestValidator()
    {
        RuleFor(x => x.Urls)
            .IsRequiredNonEmptyCollection("Urls");

        RuleForEach(x => x.Urls)
            .NotEmpty().WithMessage("maps.google_maps_url_required")
            .Must(GoogleMapsValidationRules.IsValidGoogleMapsUrl)
            .WithMessage("maps.invalid_google_maps_url");
    }
}
