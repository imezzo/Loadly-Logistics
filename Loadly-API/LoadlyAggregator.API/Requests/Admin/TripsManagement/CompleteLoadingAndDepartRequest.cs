using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Requests.Admin.TripsManagement;

public class CompleteLoadingAndDepartRequest
{
    public const string Permission = "self_complete_trip";

    public string? Note { get; set; }
    public FileUpload? ReceiptNoteImage { get; set; }
    public List<FileUpload>? ShipmentImages { get; set; }
    public FileUpload? DeliveryNoteImage { get; set; }
    public int? Odometer { get; set; }
    public FileUpload? OdometerImage { get; set; }

    public CompleteLoadingAndDepartDto FromRequest(long tripId, long waypointId) => new()
    {
        TripId = tripId,
        WaypointId = waypointId,
        Note = Note,
        ReceiptNoteImage = ReceiptNoteImage,
        ShipmentImages = ShipmentImages,
        DeliveryNoteImage = DeliveryNoteImage,
        Odometer = Odometer,
        OdometerImage = OdometerImage
    };
}

public class CompleteLoadingAndDepartRequestValidator : AbstractValidator<CompleteLoadingAndDepartRequest>
{
    public CompleteLoadingAndDepartRequestValidator()
    {
        RuleFor(x => x.ReceiptNoteImage)
            .ValidateOptionalReceiptNoteImage()
            .When(x => x.ReceiptNoteImage != null && x.ReceiptNoteImage.Length > 0);

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
