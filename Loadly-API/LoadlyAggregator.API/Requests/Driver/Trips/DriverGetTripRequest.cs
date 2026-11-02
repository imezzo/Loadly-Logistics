using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;

namespace LoadlyAggregator.API.Requests.Driver.Trips;

public class DriverGetTripRequest
{
    public long Id { get; set; }

    public Domain.Driver.DTOs.Trips.DriverGetTripDto FromRequest(long driverId) => new()
    {
        Id = Id,
        DriverId = driverId
    };
}

public class DriverGetTripRequestValidator : AbstractValidator<DriverGetTripRequest>
{
    public DriverGetTripRequestValidator()
    {
        RuleFor(x => x.Id)
            .IsRequiredId("Trip ID");
    }
}
