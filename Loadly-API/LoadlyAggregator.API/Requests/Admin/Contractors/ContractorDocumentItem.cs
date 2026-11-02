using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.API.Requests.Admin.Contractors;

public class ContractorDocumentItem
{
    public long? Id { get; set; }
    public long DocumentId { get; set; }
    public FileUpload? File { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DocumentVerificationStatus Status { get; set; } = DocumentVerificationStatus.PENDING;
}

public class ContractorDocumentItemValidator : AbstractValidator<ContractorDocumentItem>
{
    public ContractorDocumentItemValidator()
    {
        RuleFor(x => x.Id)
            .IsOptionalId("ID")
            .When(x => x.Id.HasValue);
        RuleFor(x => x.DocumentId).IsRequiredId("Document ID");
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.ExpiryDate)
            .IsFutureDate();
    }
}
