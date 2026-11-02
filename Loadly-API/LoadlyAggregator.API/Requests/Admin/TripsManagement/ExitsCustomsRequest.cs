using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.TripsManagement;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Requests.Admin.TripsManagement;

public class ExitsCustomsRequest
{
    public const string Permission = "self_complete_trip";

    public decimal? CustomsTotalAmount { get; set; }
    public string? Note { get; set; }
    public List<FileUpload>? CustomsReceipts { get; set; }

    public ExitsCustomsDto FromRequest(long tripId) => new()
    {
        TripId = tripId,
        CustomsTotalAmount = CustomsTotalAmount,
        Note = Note,
        CustomsReceipts = CustomsReceipts
    };
}

public class ExitsCustomsRequestValidator : AbstractValidator<ExitsCustomsRequest>
{
    public ExitsCustomsRequestValidator()
    {
        RuleFor(x => x.CustomsTotalAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage(TripMessageKeys.CustomsTotalAmountInvalid)
            .When(x => x.CustomsTotalAmount.HasValue);

        RuleFor(x => x.CustomsReceipts)
            .Must(images => images == null || images.Count <= TripImageValidationRules.MaxCustomsReceipts)
            .WithMessage(TripMessageKeys.CustomsReceiptsMax10)
            .When(x => x.CustomsReceipts != null);

        RuleForEach(x => x.CustomsReceipts)
            .Must(image => image.Length <= TripImageValidationRules.MaxImageSizeBytes)
            .WithMessage(TripMessageKeys.CustomsReceiptImageSizeExceeded)
            .Must(image => TripImageValidationRules.AllowedImageTypes.Contains(image.ContentType.ToLower()))
            .WithMessage(TripMessageKeys.CustomsReceiptImageType)
            .When(x => x.CustomsReceipts != null);
    }
}
