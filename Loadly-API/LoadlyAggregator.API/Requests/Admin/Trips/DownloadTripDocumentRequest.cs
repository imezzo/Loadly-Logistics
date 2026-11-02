using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class DownloadTripDocumentRequest
{
    public const string Permission = "view_trip";

    public long TripId { get; set; }
    public string Path { get; set; } = string.Empty;

    public DownloadTripDocumentDto FromRequest() => new()
    {
        TripId = TripId,
        Path = Path
    };
}

public class DownloadTripDocumentRequestValidator : AbstractValidator<DownloadTripDocumentRequest>
{
    public DownloadTripDocumentRequestValidator()
    {
        RuleFor(x => x.TripId)
            .IsRequiredId("Trip ID");
        RuleFor(x => x.Path)
            .NotEmpty().WithMessage("trips.document_path_required");
    }
}
