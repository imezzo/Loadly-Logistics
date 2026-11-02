using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class PreviewTripContractorPriceRequest
{
    public const string Permission = "view_trip";

    public long ContractId { get; set; }
    public long? TripId { get; set; }
    public long? DestinationId { get; set; }
    public long? VehicleTypeId { get; set; }
    public long? VehicleId { get; set; }
    public decimal? ActualCargoWeightTons { get; set; }

    public PreviewTripContractorPriceDto FromRequest() => new()
    {
        ContractId = ContractId,
        TripId = TripId,
        DestinationId = DestinationId,
        VehicleTypeId = VehicleTypeId,
        VehicleId = VehicleId,
        ActualCargoWeightTons = ActualCargoWeightTons
    };
}

public class PreviewTripContractorPriceRequestValidator : AbstractValidator<PreviewTripContractorPriceRequest>
{
    public PreviewTripContractorPriceRequestValidator()
    {
        RuleFor(x => x.ContractId)
            .IsRequiredId("Contract ID");

        RuleFor(x => x.TripId)
            .IsOptionalId("Trip ID");

        RuleFor(x => x.DestinationId)
            .IsOptionalId("Destination ID");

        RuleFor(x => x.VehicleTypeId)
            .IsOptionalId("Vehicle type ID");

        RuleFor(x => x.VehicleId)
            .IsOptionalId("Vehicle ID");

        When(x => x.ActualCargoWeightTons.HasValue, () =>
        {
            RuleFor(x => x.ActualCargoWeightTons)
                .GreaterThan(0).WithMessage("trips.actual_cargo_weight_tons_invalid");
        });
    }
}
