using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Documents;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Documents;

public class CreateDocumentRequest
{
    public const string Permission = "create_document";

    public DocumentEntityType EntityId { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public bool Required { get; set; } = false;
    public DocumentStatus Status { get; set; } = DocumentStatus.ACTIVE;
    public long? AddedBy { get; set; }

    public CreateDocumentDto FromRequest() => new()
    {
        EntityId = EntityId,
        NameEn = NameEn,
        NameAr = NameAr,
        Required = Required,
        Status = Status,
        AddedBy = AddedBy
    };
}

public class CreateDocumentRequestValidator : AbstractValidator<CreateDocumentRequest>
{
    public CreateDocumentRequestValidator(IDocumentRepository documentRepository)
    {
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .MustAsync(async (request, nameEn, cancellationToken) =>
                !await documentRepository.ExistsByNameEnForEntityAsync(request.EntityId, nameEn))
            .WithMessage((request, nameEn) => $"documents.name_en_already_exists|{nameEn}");
        RuleFor(x => x.NameAr)
            .IsOptionalArabicName()
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr))
            .MustAsync(async (request, nameAr, cancellationToken) =>
                !await documentRepository.ExistsByNameArForEntityAsync(request.EntityId, nameAr!))
            .WithMessage((request, nameAr) => $"documents.name_ar_already_exists|{nameAr}")
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));
        RuleFor(x => x.EntityId)
            .IsValidEnum<CreateDocumentRequest, Domain.Common.Enums.DocumentEntityType>("Entity ID");
        RuleFor(x => x.Status)
            .IsValidEnum<CreateDocumentRequest, Domain.Common.Enums.DocumentStatus>("Status");
    }
}
