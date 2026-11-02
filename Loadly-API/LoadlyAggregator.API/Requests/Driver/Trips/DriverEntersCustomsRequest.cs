using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Requests.Driver.Trips;

public class DriverEntersCustomsRequest
{
    public string? Note { get; set; }
    public List<FileUpload>? CustomsDocuments { get; set; }

    public DriverEntersCustomsDto FromRequest(long tripId, long driverId) => new()
    {
        TripId = tripId,
        DriverId = driverId,
        Note = Note,
        CustomsDocuments = CustomsDocuments
    };
}

public class DriverEntersCustomsRequestValidator : AbstractValidator<DriverEntersCustomsRequest>
{
    public DriverEntersCustomsRequestValidator()
    {
        RuleFor(x => x.CustomsDocuments)
            .Must(images => images == null || images.Count <= TripImageValidationRules.MaxCustomsDocuments)
            .WithMessage(TripMessageKeys.CustomsDocumentsMax10)
            .When(x => x.CustomsDocuments != null);

        RuleForEach(x => x.CustomsDocuments)
            .Must(image => image.Length <= TripImageValidationRules.MaxImageSizeBytes)
            .WithMessage(TripMessageKeys.CustomsDocumentsImageSizeExceeded)
            .Must(image => TripImageValidationRules.AllowedImageTypes.Contains(image.ContentType.ToLower()))
            .WithMessage(TripMessageKeys.CustomsDocumentsImageType)
            .When(x => x.CustomsDocuments != null);
    }
}
