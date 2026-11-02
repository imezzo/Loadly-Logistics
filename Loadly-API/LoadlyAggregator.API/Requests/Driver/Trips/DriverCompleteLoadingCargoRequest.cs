using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Requests.Driver.Trips;

public class DriverCompleteLoadingCargoRequest
{
    public string? Note { get; set; }
    public FileUpload? ReceiptNoteImage { get; set; }
    public List<FileUpload>? ShipmentImages { get; set; }

    public DriverCompleteLoadingCargoDto FromRequest(long tripId, long waypointId, long driverId) => new()
    {
        TripId = tripId,
        WaypointId = waypointId,
        DriverId = driverId,
        Note = Note,
        ReceiptNoteImage = ReceiptNoteImage,
        ShipmentImages = ShipmentImages
    };
}

public class DriverCompleteLoadingCargoRequestValidator : AbstractValidator<DriverCompleteLoadingCargoRequest>
{
    public DriverCompleteLoadingCargoRequestValidator()
    {
        RuleFor(x => x.ReceiptNoteImage)
            .RequireReceiptNoteImage();

        RuleFor(x => x.ShipmentImages)
            .NotNull().WithMessage(TripMessageKeys.ShipmentImagesRequired)
            .Must(images => images != null && images.Any(i => i.Length > 0))
            .WithMessage(TripMessageKeys.ShipmentImagesRequired);

        RuleFor(x => x.ShipmentImages)
            .Must(images => images == null || images.Count <= TripImageValidationRules.MaxShipmentImages)
            .WithMessage(TripMessageKeys.ShipmentImagesMax10)
            .When(x => x.ShipmentImages != null);

        RuleForEach(x => x.ShipmentImages)
            .Must(image => image.Length <= TripImageValidationRules.MaxImageSizeBytes)
            .WithMessage(TripMessageKeys.ShipmentImageSizeExceeded)
            .Must(image => TripImageValidationRules.AllowedImageTypes.Contains(image.ContentType.ToLower()))
            .WithMessage(TripMessageKeys.ShipmentImageType)
            .When(x => x.ShipmentImages != null);
    }
}
