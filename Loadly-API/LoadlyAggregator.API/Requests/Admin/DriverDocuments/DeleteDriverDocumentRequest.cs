using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.DriverDocuments;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;

namespace LoadlyAggregator.API.Requests.Admin.DriverDocuments;

public class DeleteDriverDocumentRequest
{
    public const string Permission = "delete_driver_document";

    public long Id { get; set; }

    public DeleteDriverDocumentDto FromRequest() => new() { Id = Id };
}

public class DeleteDriverDocumentRequestValidator : AbstractValidator<DeleteDriverDocumentRequest>
{
    public DeleteDriverDocumentRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Entity ID");
    }
}
