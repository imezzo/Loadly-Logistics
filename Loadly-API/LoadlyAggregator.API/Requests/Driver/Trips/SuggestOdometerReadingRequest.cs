using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Domain.Driver.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Requests.Driver.Trips;

public class SuggestOdometerReadingRequest
{
    public FileUpload? OdometerImage { get; set; }
    public int? ScanContext { get; set; }
    public long? PreviousScanLogId { get; set; }

    public SuggestOdometerReadingDto FromRequest(
        long tripId,
        long modelId,
        string modelType,
        string? correlationId) => new()
    {
        TripId = tripId,
        ModelId = modelId,
        ModelType = modelType,
        OdometerImage = OdometerImage!,
        ScanContext = ScanContext.HasValue
            ? (OdometerScanContext)ScanContext.Value
            : OdometerScanContext.Suggest,
        PreviousScanLogId = PreviousScanLogId,
        CorrelationId = correlationId
    };
}

public class SuggestOdometerReadingRequestValidator : AbstractValidator<SuggestOdometerReadingRequest>
{
    public SuggestOdometerReadingRequestValidator()
    {
        RuleFor(x => x.OdometerImage)
            .RequireOdometerImage();

        RuleFor(x => x.ScanContext)
            .Must(value => !value.HasValue || Enum.IsDefined(typeof(OdometerScanContext), value.Value))
            .WithMessage(TripMessageKeys.OdometerScanContextInvalid);
    }
}
