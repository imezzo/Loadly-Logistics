using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class SetTripDispatchingModeRequest
{
    public const string Permission = "edit_trip";

    public TripDispatchingMode DispatchingMode { get; set; }

    public SetTripDispatchingModeDto FromRequest(long id) => new()
    {
        Id = id,
        DispatchingMode = DispatchingMode
    };
}

public class SetTripDispatchingModeRequestValidator : AbstractValidator<SetTripDispatchingModeRequest>
{
    public SetTripDispatchingModeRequestValidator()
    {
        RuleFor(x => x.DispatchingMode)
            .IsInEnum().WithMessage("validation.trip_dispatching_mode_enum_invalid");
    }
}
