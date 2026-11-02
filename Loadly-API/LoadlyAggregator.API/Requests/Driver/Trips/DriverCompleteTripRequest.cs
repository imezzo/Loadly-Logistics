using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Requests.Driver.Trips;

public class DriverCompleteTripRequest
{
    public int Odometer { get; set; }
    public decimal? TollgatesTotalAmount { get; set; }
    public decimal? TollgatesWeightTotalAmount { get; set; }
    public FileUpload? OdometerImage { get; set; }
    public string? Note { get; set; }
    public List<FileUpload>? TollgateImages { get; set; }
    public List<FileUpload>? TollgateWeightImages { get; set; }
    public FileUpload? DeliveryNoteImage { get; set; }

    public DriverCompleteTripDto FromRequest(long tripId, long driverId) => new()
    {
        TripId = tripId,
        DriverId = driverId,
        Odometer = Odometer,
        TollgatesTotalAmount = TollgatesTotalAmount,
        TollgatesWeightTotalAmount = TollgatesWeightTotalAmount,
        OdometerImage = OdometerImage,
        Note = Note,
        TollgateImages = TollgateImages,
        TollgateWeightImages = TollgateWeightImages,
        DeliveryNoteImage = DeliveryNoteImage
    };
}

public class DriverCompleteTripRequestValidator : AbstractValidator<DriverCompleteTripRequest>
{
    public DriverCompleteTripRequestValidator()
    {
        RuleFor(x => x.Odometer)
            .GreaterThanOrEqualTo(0)
            .WithMessage(TripMessageKeys.OdometerRequired);

        RuleFor(x => x.TollgatesTotalAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage(TripMessageKeys.TollgatesTotalAmountRequired)
            .When(x => x.TollgatesTotalAmount.HasValue);

        RuleFor(x => x.TollgatesWeightTotalAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage(TripMessageKeys.TollgatesWeightTotalAmountInvalid)
            .When(x => x.TollgatesWeightTotalAmount.HasValue);

        RuleFor(x => x.OdometerImage)
            .RequireOdometerImage();

        RuleFor(x => x.TollgateImages)
            .Must(images => images == null || images.Count <= TripImageValidationRules.MaxTollgateImages)
            .WithMessage(TripMessageKeys.TollgateImagesMax20)
            .When(x => x.TollgateImages != null);

        RuleForEach(x => x.TollgateImages)
            .Must(image => image.Length <= TripImageValidationRules.MaxImageSizeBytes)
            .WithMessage(TripMessageKeys.TollgateImageSizeExceeded)
            .Must(image => TripImageValidationRules.AllowedImageTypes.Contains(image.ContentType.ToLower()))
            .WithMessage(TripMessageKeys.TollgateImageType)
            .When(x => x.TollgateImages != null);

        RuleFor(x => x.TollgateWeightImages)
            .Must(images => images == null || images.Count <= TripImageValidationRules.MaxTollgateImages)
            .WithMessage(TripMessageKeys.TollgateWeightImagesMax20)
            .When(x => x.TollgateWeightImages != null);

        RuleForEach(x => x.TollgateWeightImages)
            .Must(image => image.Length <= TripImageValidationRules.MaxImageSizeBytes)
            .WithMessage(TripMessageKeys.TollgateWeightImageSizeExceeded)
            .Must(image => TripImageValidationRules.AllowedImageTypes.Contains(image.ContentType.ToLower()))
            .WithMessage(TripMessageKeys.TollgateWeightImageType)
            .When(x => x.TollgateWeightImages != null);

        RuleFor(x => x.DeliveryNoteImage)
            .ValidateOptionalDeliveryNoteImage()
            .When(x => x.DeliveryNoteImage != null && x.DeliveryNoteImage.Length > 0);
    }
}
