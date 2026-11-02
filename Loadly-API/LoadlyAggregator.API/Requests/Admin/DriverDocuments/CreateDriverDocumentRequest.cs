using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.DriverDocuments;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.API.Requests.Admin.DriverDocuments;

public class CreateDriverDocumentRequest
{
    public const string Permission = "create_driver_document";

    public long DocumentId { get; set; }
    public long DriverId { get; set; }
    public FileUpload? File { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DocumentVerificationStatus Status { get; set; } = DocumentVerificationStatus.PENDING;
    public long? AddedBy { get; set; }

    public CreateDriverDocumentDto FromRequest() => new()
    {
        DocumentId = DocumentId,
        DriverId = DriverId,
        File = File,
        ExpiryDate = ExpiryDate,
        Status = Status,
        AddedBy = AddedBy
    };
}

public class CreateDriverDocumentRequestValidator : AbstractValidator<CreateDriverDocumentRequest>
{
    public CreateDriverDocumentRequestValidator()
    {
        RuleFor(x => x.DocumentId).IsRequiredId("Document ID");
        RuleFor(x => x.DriverId).IsRequiredId("Driver ID");
        RuleFor(x => x.Status)
            .IsValidEnum<CreateDriverDocumentRequest, Domain.Common.Enums.DocumentVerificationStatus>("Status");
    }
}
