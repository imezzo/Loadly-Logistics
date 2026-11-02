using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Requests.Admin.TripsManagement;

public class CompleteUnloadingAndDepartRequest
{
    public const string Permission = "self_complete_trip";

    public string? Note { get; set; }
    public FileUpload? DeliveryNoteImage { get; set; }
    public List<FileUpload>? UnloadingImages { get; set; }
    public int? Odometer { get; set; }
    public FileUpload? OdometerImage { get; set; }

    public CompleteUnloadingAndDepartDto FromRequest(long tripId, long waypointId) => new()
    {
        TripId = tripId,
        WaypointId = waypointId,
        Note = Note,
        DeliveryNoteImage = DeliveryNoteImage,
        UnloadingImages = UnloadingImages,
        Odometer = Odometer,
        OdometerImage = OdometerImage
    };
}

public class CompleteUnloadingAndDepartRequestValidator : AbstractValidator<CompleteUnloadingAndDepartRequest>
{
    public CompleteUnloadingAndDepartRequestValidator()
    {
        RuleFor(x => x.DeliveryNoteImage)
            .ValidateOptionalDeliveryNoteImage()
            .When(x => x.DeliveryNoteImage != null && x.DeliveryNoteImage.Length > 0);

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
