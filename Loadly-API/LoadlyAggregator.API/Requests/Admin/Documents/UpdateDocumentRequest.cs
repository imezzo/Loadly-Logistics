using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Documents;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Documents;

public class UpdateDocumentRequest
{
    public const string Permission = "edit_document";

    [FromRoute]
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public bool Required { get; set; } = false;
    public DocumentStatus Status { get; set; } = DocumentStatus.ACTIVE;

    public UpdateDocumentDto FromRequest() => new()
    {
        Id = Id,
        NameEn = NameEn,
        NameAr = NameAr,
        Required = Required,
        Status = Status
    };
}

public class UpdateDocumentRequestValidator : AbstractValidator<UpdateDocumentRequest>
{
    public UpdateDocumentRequestValidator(IDocumentRepository documentRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Document ID");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .MustAsync(async (request, nameEn, cancellationToken) =>
                !await documentRepository.ExistsByNameEnExcludingIdAsync(request.Id, nameEn))
            .WithMessage((request, nameEn) => $"documents.name_en_already_exists|{nameEn}");
        RuleFor(x => x.NameAr)
            .IsOptionalArabicName()
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr))
            .MustAsync(async (request, nameAr, cancellationToken) =>
                !await documentRepository.ExistsByNameArExcludingIdAsync(request.Id, nameAr!))
            .WithMessage((request, nameAr) => $"documents.name_ar_already_exists|{nameAr}")
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));
        RuleFor(x => x.Status)
            .IsValidEnum<UpdateDocumentRequest, Domain.Common.Enums.DocumentStatus>("Status");
    }
}
