using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Driver.DTOs.Documents;

namespace LoadlyAggregator.API.Requests.Driver.Documents;

public class DriverUploadDocumentRequest
{
    public const string Permission = "upload_driver_document";

    public long DocumentId { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public Domain.Common.Models.FileUpload? File { get; set; }

    public DriverUploadDocumentDto FromRequest(long driverId) => new()
    {
        DriverId = driverId,
        DocumentId = DocumentId,
        File = File!,
        ExpiryDate = ExpiryDate
    };
}

public class DriverUploadDocumentRequestValidator : AbstractValidator<DriverUploadDocumentRequest>
{
    public DriverUploadDocumentRequestValidator()
    {
        RuleFor(x => x.DocumentId).IsRequiredId("Document ID");
        RuleFor(x => x.File)
            .NotNull().WithMessage("documents.file_required")
            .Must(f => f!.Length > 0).WithMessage("documents.file_required")
            .IsValidFileType(DocumentFileTypeValidator.AllowedMimeTypes, "documents.file_type_not_allowed");
    }
}
