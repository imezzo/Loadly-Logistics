using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Driver.Trips;

public class DriverDownloadTripDocumentRequest
{
    public long TripId { get; set; }
    public string Path { get; set; } = string.Empty;

    public DriverDownloadTripDocumentDto FromRequest(long driverId) => new()
    {
        TripId = TripId,
        Path = Path,
        DriverId = driverId
    };
}

public class DriverDownloadTripDocumentRequestValidator : AbstractValidator<DriverDownloadTripDocumentRequest>
{
    public DriverDownloadTripDocumentRequestValidator()
    {
        RuleFor(x => x.TripId)
            .IsRequiredId("Trip ID");
        RuleFor(x => x.Path)
            .NotEmpty().WithMessage("trips.document_path_required");
    }
}
