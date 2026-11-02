using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.DriverDocuments;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.DriverDocuments;

public class UpdateDriverDocumentRequest
{
    public const string Permission = "edit_driver_document";

    [FromRoute]
    public long Id { get; set; }
    public long DocumentId { get; set; }
    public long DriverId { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DocumentVerificationStatus Status { get; set; }

    public UpdateDriverDocumentDto FromRequest() => new()
    {
        Id = Id,
        DocumentId = DocumentId,
        DriverId = DriverId,
        ExpiryDate = ExpiryDate,
        Status = Status
    };
}

public class UpdateDriverDocumentRequestValidator : AbstractValidator<UpdateDriverDocumentRequest>
{
    public UpdateDriverDocumentRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("ID");
        RuleFor(x => x.DocumentId).IsRequiredId("Document ID");
        RuleFor(x => x.DriverId).IsRequiredId("Driver ID");
        RuleFor(x => x.Status)
            .IsValidEnum<UpdateDriverDocumentRequest, Domain.Common.Enums.DocumentVerificationStatus>("Status");
    }
}
