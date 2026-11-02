using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Settings.DocumentStatuses;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.DocumentStatuses;

public class UpdateDocumentStatusRequest
{
    public const string Permission = "edit_document_status";

    [FromRoute]
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;

    public UpdateDocumentStatusDto FromRequest() => new()
    {
        Id = Id,
        NameAr = NameAr,
        NameEn = NameEn
    };
}

public class UpdateDocumentStatusRequestValidator : AbstractValidator<UpdateDocumentStatusRequest>
{
    public UpdateDocumentStatusRequestValidator(IDocumentStatusRepository documentStatusRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Document status ID");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async (nameEn, excludeId) =>
            {
                var existing = await documentStatusRepository.GetByNameEnAsync(nameEn);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, value => $"document_statuses.name_en_already_exists|{value}");
        RuleFor(x => x.NameAr)
            .IsRequiredArabicName()
            .IsUnique(async (nameAr, excludeId) =>
            {
                var existing = await documentStatusRepository.GetByNameArAsync(nameAr);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, value => $"document_statuses.name_ar_already_exists|{value}");
    }
}
