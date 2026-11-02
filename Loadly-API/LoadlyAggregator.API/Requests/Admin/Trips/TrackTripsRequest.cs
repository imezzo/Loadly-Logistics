using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class TrackTripsRequest
{
    public const string Permission = "view_trip";

    public long? CompanyId { get; set; }
    public string? ContractReferenceNumber { get; set; }
    public long? DriverId { get; set; }
    public long? VehicleId { get; set; }

    public TrackTripsDto FromRequest() => new()
    {
        CompanyId = CompanyId,
        ContractReferenceNumber = ContractReferenceNumber,
        DriverId = DriverId,
        VehicleId = VehicleId
    };
}

public class TrackTripsRequestValidator : AbstractValidator<TrackTripsRequest>
{
    public TrackTripsRequestValidator()
    {
        RuleFor(x => x.CompanyId)
            .IsOptionalId("Company ID")
            .When(x => x.CompanyId.HasValue);

        RuleFor(x => x.ContractReferenceNumber)
            .MaximumLength(100).WithMessage("validation.contract_reference_max_length|100")
            .When(x => !string.IsNullOrWhiteSpace(x.ContractReferenceNumber));

        RuleFor(x => x.DriverId)
            .IsOptionalId("Driver ID")
            .When(x => x.DriverId.HasValue);

        RuleFor(x => x.VehicleId)
            .IsOptionalId("Vehicle ID")
            .When(x => x.VehicleId.HasValue);
    }
}
