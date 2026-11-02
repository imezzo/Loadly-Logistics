using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.DriverDocuments;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.DriverDocuments;

public class ListDriverDocumentsRequest : BasePagedRequest
{
    public const string Permission = "view_driver_document";

    public long? DriverId { get; set; }
    public long? DocumentId { get; set; }
    public DocumentVerificationStatus? Status { get; set; }

    public ListDriverDocumentsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        DriverId = DriverId,
        DocumentId = DocumentId,
        Status = Status
    };
}

public class ListDriverDocumentsRequestValidator : AbstractValidator<ListDriverDocumentsRequest>
{
    public ListDriverDocumentsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.DriverId)
            .IsOptionalId("Driver ID")
            .When(x => x.DriverId.HasValue);
        RuleFor(x => x.DocumentId)
            .IsOptionalId("Document ID")
            .When(x => x.DocumentId.HasValue);
        RuleFor(x => x.Status)
            .IsValidEnum<ListDriverDocumentsRequest, Domain.Common.Enums.DocumentVerificationStatus>("Status")
            .When(x => x.Status.HasValue);
    }
}
