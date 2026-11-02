using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Requests.Driver.Trips;

public class DriverCompleteUnloadingCargoRequest
{
    public string? Note { get; set; }
    public FileUpload? DeliveryNoteImage { get; set; }
    public List<FileUpload>? UnloadingImages { get; set; }
    public int? Odometer { get; set; }
    public FileUpload? OdometerImage { get; set; }

    public DriverCompleteUnloadingCargoDto FromRequest(long tripId, long waypointId, long driverId) => new()
    {
        TripId = tripId,
        WaypointId = waypointId,
        DriverId = driverId,
        Note = Note,
        DeliveryNoteImage = DeliveryNoteImage,
        UnloadingImages = UnloadingImages,
        Odometer = Odometer,
        OdometerImage = OdometerImage
    };
}

public class DriverCompleteUnloadingCargoRequestValidator : AbstractValidator<DriverCompleteUnloadingCargoRequest>
{
    public DriverCompleteUnloadingCargoRequestValidator()
    {
        RuleFor(x => x.DeliveryNoteImage)
            .RequireDeliveryNoteImage();

        RuleFor(x => x.UnloadingImages)
            .NotNull().WithMessage(TripMessageKeys.UnloadingImagesRequired)
            .Must(images => images != null && images.Any(i => i != null && i.Length > 0))
            .WithMessage(TripMessageKeys.UnloadingImagesRequired);

        RuleFor(x => x.UnloadingImages)
            .Must(images => images == null || images.Count <= TripImageValidationRules.MaxUnloadingImages)
            .WithMessage(TripMessageKeys.UnloadingImagesMax10)
            .When(x => x.UnloadingImages != null);

        RuleForEach(x => x.UnloadingImages)
            .Must(image => image.Length <= TripImageValidationRules.MaxImageSizeBytes)
            .WithMessage(TripMessageKeys.UnloadingImageSizeExceeded)
            .Must(image => TripImageValidationRules.AllowedImageTypes.Contains(image.ContentType.ToLower()))
            .WithMessage(TripMessageKeys.UnloadingImageType)
            .When(x => x.UnloadingImages != null);

        RuleFor(x => x.Odometer)
            .GreaterThanOrEqualTo(0)
            .WithMessage(TripMessageKeys.OdometerRequired)
            .When(x => x.Odometer.HasValue);

        RuleFor(x => x.OdometerImage)
            .ValidateOptionalOdometerImage()
            .When(x => x.OdometerImage != null && x.OdometerImage.Length > 0);
    }
}
