using FluentValidation;
using LoadlyAggregator.Domain.Driver.DTOs.Location;

namespace LoadlyAggregator.API.Requests.Driver.Location;

public class DriverGoOnlineRequest
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public DriverGoOnlineDto FromRequest(long driverId) => new()
    {
        DriverId = driverId,
        Latitude = Latitude,
        Longitude = Longitude
    };
}

public class DriverGoOnlineRequestValidator : AbstractValidator<DriverGoOnlineRequest>
{
    public DriverGoOnlineRequestValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("validation.latitude_between");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("validation.longitude_between");
    }
}
