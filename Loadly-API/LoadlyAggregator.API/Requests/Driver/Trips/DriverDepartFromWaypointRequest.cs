using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Requests.Driver.Trips;

public class DriverDepartFromWaypointRequest
{
    public string? Note { get; set; }
    public FileUpload? DeliveryNoteImage { get; set; }
    public int? Odometer { get; set; }
    public FileUpload? OdometerImage { get; set; }

    public DriverDepartFromWaypointDto FromRequest(long tripId, long waypointId, long driverId) => new()
    {
        TripId = tripId,
        WaypointId = waypointId,
        DriverId = driverId,
        Note = Note,
        DeliveryNoteImage = DeliveryNoteImage,
        Odometer = Odometer,
        OdometerImage = OdometerImage
    };
}

public class DriverDepartFromWaypointRequestValidator : AbstractValidator<DriverDepartFromWaypointRequest>
{
    public DriverDepartFromWaypointRequestValidator()
    {
        RuleFor(x => x.DeliveryNoteImage)
            .ValidateOptionalDeliveryNoteImage()
            .When(x => x.DeliveryNoteImage != null && x.DeliveryNoteImage.Length > 0);

        RuleFor(x => x.Odometer)
            .GreaterThanOrEqualTo(0)
            .WithMessage(TripMessageKeys.OdometerRequired)
            .When(x => x.Odometer.HasValue);

        RuleFor(x => x.OdometerImage)
            .ValidateOptionalOdometerImage()
            .When(x => x.OdometerImage != null && x.OdometerImage.Length > 0);
    }
}
