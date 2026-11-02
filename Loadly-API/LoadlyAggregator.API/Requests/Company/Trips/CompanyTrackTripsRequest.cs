using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Company.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Company.Trips;

public class CompanyTrackTripsRequest
{
    public const string Permission = "view_trip";

    public string? ContractReferenceNumber { get; set; }

    public CompanyTrackTripsDto FromRequest(long companyId) => new()
    {
        CompanyId = companyId,
        ContractReferenceNumber = ContractReferenceNumber
    };
}

public class CompanyTrackTripsRequestValidator : AbstractValidator<CompanyTrackTripsRequest>
{
    public CompanyTrackTripsRequestValidator()
    {
        RuleFor(x => x.ContractReferenceNumber)
            .MaximumLength(100).WithMessage("validation.contract_reference_max_length|100")
            .When(x => !string.IsNullOrWhiteSpace(x.ContractReferenceNumber));
    }
}
