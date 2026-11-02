using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class ApproveTripBookingRequestRequest
{
    public const string Permission = "edit_trip";

    public ApproveTripBookingRequestDto FromRequest(long tripId, long requestId) => new()
    {
        TripId = tripId,
        RequestId = requestId
    };
}

public class ApproveTripBookingRequestRequestValidator : AbstractValidator<ApproveTripBookingRequestRequest>
{
    public ApproveTripBookingRequestRequestValidator()
    {
    }
}
