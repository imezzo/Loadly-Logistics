using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.DocumentStatuses;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.DocumentStatuses;

public class CreateDocumentStatusRequest
{
    public const string Permission = "create_document_status";

    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;

    public CreateDocumentStatusDto FromRequest() => new()
    {
        NameAr = NameAr,
        NameEn = NameEn
    };
}

public class CreateDocumentStatusRequestValidator : AbstractValidator<CreateDocumentStatusRequest>
{
    public CreateDocumentStatusRequestValidator(IDocumentStatusRepository documentStatusRepository)
    {
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async nameEn => await documentStatusRepository.GetByNameEnAsync(nameEn) == null, value => $"document_statuses.name_en_already_exists|{value}");
        RuleFor(x => x.NameAr)
            .IsRequiredArabicName()
            .IsUnique(async nameAr => await documentStatusRepository.GetByNameArAsync(nameAr) == null, value => $"document_statuses.name_ar_already_exists|{value}");
    }
}
