using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Contractor.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Contractor.Trips;

public class ContractorDownloadTripDocumentRequest
{
    public long TripId { get; set; }
    public string Path { get; set; } = string.Empty;

    public ContractorDownloadTripDocumentDto FromRequest(long contractorId) => new()
    {
        TripId = TripId,
        Path = Path,
        ContractorId = contractorId
    };
}

public class ContractorDownloadTripDocumentRequestValidator : AbstractValidator<ContractorDownloadTripDocumentRequest>
{
    public ContractorDownloadTripDocumentRequestValidator()
    {
        RuleFor(x => x.TripId).IsRequiredId("Trip ID");
        RuleFor(x => x.Path).NotEmpty().WithMessage("trips.document_path_required");
    }
}
