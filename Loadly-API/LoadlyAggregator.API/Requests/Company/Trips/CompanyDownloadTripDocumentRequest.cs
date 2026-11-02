using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Company.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Company.Trips;

public class CompanyDownloadTripDocumentRequest
{
    public long TripId { get; set; }
    public string Path { get; set; } = string.Empty;

    public CompanyDownloadTripDocumentDto FromRequest(long companyId) => new()
    {
        TripId = TripId,
        Path = Path,
        CompanyId = companyId
    };
}

public class CompanyDownloadTripDocumentRequestValidator : AbstractValidator<CompanyDownloadTripDocumentRequest>
{
    public CompanyDownloadTripDocumentRequestValidator()
    {
        RuleFor(x => x.TripId)
            .IsRequiredId("Trip ID");
        RuleFor(x => x.Path)
            .NotEmpty().WithMessage("trips.document_path_required");
    }
}
