using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class PushTripToContractorsRequest
{
    public const string Permission = "edit_trip";

    public long VehicleTypeId { get; set; }
    public decimal? SpotPrice { get; set; }
    public decimal? ContractorPrice { get; set; }
    public bool SendToAllContractors { get; set; }
    public List<long>? ContractorIds { get; set; }

    public PushTripToContractorsDto FromRequest(long tripId) => new()
    {
        TripId = tripId,
        VehicleTypeId = VehicleTypeId,
        SpotPrice = SpotPrice,
        ContractorPrice = ContractorPrice,
        SendToAllContractors = SendToAllContractors,
        ContractorIds = ContractorIds
    };
}

public class PushTripToContractorsRequestValidator : AbstractValidator<PushTripToContractorsRequest>
{
    public PushTripToContractorsRequestValidator()
    {
        RuleFor(x => x.VehicleTypeId)
            .IsRequiredId("Vehicle type ID");

        When(x => x.SpotPrice.HasValue, () =>
        {
            RuleFor(x => x.SpotPrice)
                .GreaterThan(0).WithMessage("trips.spot_price_invalid");
        });

        RuleFor(x => x.ContractorPrice)
            .NotNull().WithMessage(TripMessageKeys.ContractorPriceRequired);

        RuleFor(x => x.ContractorPrice)
            .GreaterThan(0).WithMessage("trips.contractor_price_invalid")
            .When(x => x.ContractorPrice.HasValue);

        When(x => !x.SendToAllContractors, () =>
        {
            RuleFor(x => x.ContractorIds)
                .NotEmpty().WithMessage(TripMessageKeys.PushAudienceRequired);

            RuleForEach(x => x.ContractorIds!)
                .GreaterThan(0).WithMessage("validation.entity_must_be_greater_than_zero|Contractor ID");
        });
    }
}
