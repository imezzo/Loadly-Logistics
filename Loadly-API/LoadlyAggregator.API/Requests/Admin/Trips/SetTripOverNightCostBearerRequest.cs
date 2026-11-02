using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class SetTripOverNightCostBearerRequest
{
    public const string Permission = "edit_trip";

    public OverNightCostBearer OverNightCostBearer { get; set; }

    public SetTripOverNightCostBearerDto FromRequest(long id) => new()
    {
        Id = id,
        OverNightCostBearer = OverNightCostBearer
    };
}

public class SetTripOverNightCostBearerRequestValidator : AbstractValidator<SetTripOverNightCostBearerRequest>
{
    public SetTripOverNightCostBearerRequestValidator()
    {
        RuleFor(x => x.OverNightCostBearer)
            .IsInEnum().WithMessage("validation.trip_status_enum_invalid");
    }
}
