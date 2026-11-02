using FluentValidation;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Validators.Extensions;

/// <summary>
/// Shared constants and extension methods for trip-related image validation.
/// </summary>
public static class TripImageValidationRules
{
    public static readonly string[] AllowedImageTypes =
        { "image/jpeg", "image/jpg", "image/png", "image/webp" };

    public const long MaxImageSizeBytes = 10 * 1024 * 1024; // 10MB
    public const int MaxShipmentImages = 20;
    public const int MaxUnloadingImages = 20;
    public const int MaxTollgateImages = 20;
    public const int MaxCustomsReceipts = 20;
    public const int MaxCustomsDocuments = 20;

    /// <summary>
    /// Requires odometer image: NotNull, non-empty, valid file type, max size.
    /// </summary>
    public static IRuleBuilderOptions<T, FileUpload?> RequireOdometerImage<T>(
        this IRuleBuilder<T, FileUpload?> ruleBuilder)
    {
        return ruleBuilder
            .NotNull().WithMessage(TripMessageKeys.OdometerImageRequired)
            .Must(x => x != null && x.Length > 0).WithMessage(TripMessageKeys.OdometerImageRequired)
            .IsValidFileType(AllowedImageTypes, TripMessageKeys.OdometerImageType)
            .Must(image => image == null || image.Length <= MaxImageSizeBytes)
            .WithMessage(TripMessageKeys.OdometerImageSizeExceeded);
    }

    /// <summary>
    /// Requires receipt note image: NotNull, non-empty, valid file type, max size.
    /// </summary>
    public static IRuleBuilderOptions<T, FileUpload?> RequireReceiptNoteImage<T>(
        this IRuleBuilder<T, FileUpload?> ruleBuilder)
    {
        return ruleBuilder
            .NotNull().WithMessage(TripMessageKeys.ReceiptNoteImageRequired)
            .Must(x => x != null && x.Length > 0).WithMessage(TripMessageKeys.ReceiptNoteImageRequired)
            .IsValidFileType(AllowedImageTypes, TripMessageKeys.ReceiptNoteImageType)
            .Must(image => image == null || image.Length <= MaxImageSizeBytes)
            .WithMessage(TripMessageKeys.ReceiptNoteImageSizeExceeded);
    }

    /// <summary>
    /// Validates optional odometer image: file type and size when provided.
    /// Rules pass when value is null; validate only when non-null and non-empty.
    /// </summary>
    public static IRuleBuilderOptions<T, FileUpload?> ValidateOptionalOdometerImage<T>(
        this IRuleBuilder<T, FileUpload?> ruleBuilder)
    {
        return ruleBuilder
            .IsValidFileType(AllowedImageTypes, TripMessageKeys.OdometerImageType)
            .Must(image => image == null || image.Length == 0 || image.Length <= MaxImageSizeBytes)
            .WithMessage(TripMessageKeys.OdometerImageSizeExceeded);
    }

    /// <summary>
    /// Validates optional receipt note image: file type and size when provided.
    /// </summary>
    public static IRuleBuilderOptions<T, FileUpload?> ValidateOptionalReceiptNoteImage<T>(
        this IRuleBuilder<T, FileUpload?> ruleBuilder)
    {
        return ruleBuilder
            .IsValidFileType(AllowedImageTypes, TripMessageKeys.ReceiptNoteImageType)
            .Must(image => image == null || image.Length == 0 || image.Length <= MaxImageSizeBytes)
            .WithMessage(TripMessageKeys.ReceiptNoteImageSizeExceeded);
    }

    /// <summary>
    /// Requires delivery note image: NotNull, non-empty, valid file type, max size.
    /// </summary>
    public static IRuleBuilderOptions<T, FileUpload?> RequireDeliveryNoteImage<T>(
        this IRuleBuilder<T, FileUpload?> ruleBuilder)
    {
        return ruleBuilder
            .NotNull().WithMessage(TripMessageKeys.DeliveryNoteImageRequiredAtDropoff)
            .Must(x => x != null && x.Length > 0).WithMessage(TripMessageKeys.DeliveryNoteImageRequiredAtDropoff)
            .IsValidFileType(AllowedImageTypes, TripMessageKeys.DeliveryNoteImageType)
            .Must(image => image == null || image.Length <= MaxImageSizeBytes)
            .WithMessage(TripMessageKeys.DeliveryNoteImageSizeExceeded);
    }

    /// <summary>
    /// Validates optional delivery note image: file type and size when provided.
    /// Rules pass when value is null; validate only when non-null and non-empty.
    /// </summary>
    public static IRuleBuilderOptions<T, FileUpload?> ValidateOptionalDeliveryNoteImage<T>(
        this IRuleBuilder<T, FileUpload?> ruleBuilder)
    {
        return ruleBuilder
            .IsValidFileType(AllowedImageTypes, TripMessageKeys.DeliveryNoteImageType)
            .Must(image => image == null || image.Length == 0 || image.Length <= MaxImageSizeBytes)
            .WithMessage(TripMessageKeys.DeliveryNoteImageSizeExceeded);
    }
}
