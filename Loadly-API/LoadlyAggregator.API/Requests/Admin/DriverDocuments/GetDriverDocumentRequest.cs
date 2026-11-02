using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.DriverDocuments;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;

namespace LoadlyAggregator.API.Requests.Admin.DriverDocuments;

public class GetDriverDocumentRequest
{
    public const string Permission = "view_driver_document";

    public long Id { get; set; }

    public GetDriverDocumentDto FromRequest() => new() { Id = Id };
}

public class GetDriverDocumentRequestValidator : AbstractValidator<GetDriverDocumentRequest>
{
    public GetDriverDocumentRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("ID");
    }
}
