using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Requests.Admin.TripsManagement;

public class EntersCustomsRequest
{
    public const string Permission = "self_complete_trip";

    public string? Note { get; set; }
    public List<FileUpload>? CustomsDocuments { get; set; }

    public EntersCustomsDto FromRequest(long tripId) => new()
    {
        TripId = tripId,
        Note = Note,
        CustomsDocuments = CustomsDocuments
    };
}

public class EntersCustomsRequestValidator : AbstractValidator<EntersCustomsRequest>
{
    public EntersCustomsRequestValidator()
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
