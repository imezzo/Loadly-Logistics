using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Driver.DTOs.Documents;

namespace LoadlyAggregator.API.Requests.Driver.Documents;

public class DriverUpdateDocumentRequest
{
    public const string Permission = "update_driver_document";

    public long Id { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public Domain.Common.Models.FileUpload? File { get; set; }

    public DriverUpdateDocumentDto FromRequest(long driverId) => new()
    {
        Id = Id,
        DriverId = driverId,
        File = File,
        ExpiryDate = ExpiryDate
    };
}

public class DriverUpdateDocumentRequestValidator : AbstractValidator<DriverUpdateDocumentRequest>
{
    public DriverUpdateDocumentRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("ID");
        RuleFor(x => x.File)
            .IsValidFileType(DocumentFileTypeValidator.AllowedMimeTypes, "documents.file_type_not_allowed")
            .When(x => x.File != null && x.File.Length > 0);
        RuleFor(x => x)
            .Must(x => (x.File != null && x.File.Length > 0) || x.ExpiryDate.HasValue)
            .WithMessage("documents.update_requires_file_or_expiry_date");
    }
}
