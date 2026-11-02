using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractorHelper
{
    public static string BuildUserNameFromPhone(string phone)
    {
        var normalized = phone.Trim();
        if (normalized.StartsWith("+20", StringComparison.Ordinal))
        {
            normalized = normalized[3..];
        }
        else if (normalized.StartsWith("20", StringComparison.Ordinal)
                 && normalized.Length > 2
                 && char.IsDigit(normalized[2]))
        {
            normalized = normalized[2..];
        }
        else if (normalized.StartsWith('0') && normalized.Length > 1)
        {
            normalized = normalized[1..];
        }

        var digitsOnly = string.Concat(normalized.Where(char.IsDigit));
        return $"contractor{digitsOnly}".ToLower();
    }

    public static async Task CleanupUploadedFilesAsync(IMediaService mediaService, IEnumerable<string> uploadedFilePaths)
    {
        foreach (var filePath in uploadedFilePaths)
        {
            try
            {
                await mediaService.RemoveAsync(filePath);
            }
            catch
            {
            }
        }
    }

    public static void ValidateDocuments(
        IReadOnlyList<Document> contractorDocuments,
        IReadOnlyList<ContractorDocumentItemDto>? providedDocuments,
        bool requireFilesForRequiredDocuments = false)
    {
        var providedIds = providedDocuments?.Select(d => d.DocumentId).ToList() ?? new List<long>();
        var contractorDocumentIds = contractorDocuments.Select(d => d.Id).ToHashSet();

        foreach (var documentId in providedIds)
        {
            if (!contractorDocumentIds.Contains(documentId))
            {
                throw new ValidationException("contractors.invalid_document", documentId);
            }
        }

        var requiredDocuments = contractorDocuments.Where(d => d.Required && d.Status == DocumentStatus.ACTIVE);
        foreach (var requiredDoc in requiredDocuments)
        {
            if (!providedIds.Contains(requiredDoc.Id))
            {
                throw new ValidationException("contractors.required_document", requiredDoc.NameEn);
            }

            if (requireFilesForRequiredDocuments)
            {
                var providedDoc = providedDocuments?.FirstOrDefault(d => d.DocumentId == requiredDoc.Id);
                if (providedDoc is null || providedDoc.File is not { Length: > 0 })
                {
                    throw new ValidationException("contractors.required_document_file", requiredDoc.NameEn);
                }
            }
        }
    }

    public static void ValidateDocumentEntityType(Document document, long documentId)
    {
        if (document.EntityId != DocumentEntityType.CONTRACTOR)
        {
            throw new ValidationException("documents.invalid_entity_type_for_contractor", documentId, document.EntityId);
        }
    }

    public static void ApplyAggregateCounts(ContractorDto dto, ContractorAggregateCounts counts)
    {
        dto.TotalDrivers = counts.TotalDrivers;
        dto.TotalVehicles = counts.TotalVehicles;
        dto.TotalTrips = counts.TotalTrips;
        dto.TotalCompletedTrips = counts.TotalCompletedTrips;
        dto.TotalCompletedPaidTrips = counts.TotalCompletedPaidTrips;
        dto.TotalCompletedUnderReviewTrips = counts.TotalCompletedUnderReviewTrips;
        dto.TotalCompletedReadyToPayTrips = counts.TotalCompletedReadyToPayTrips;
    }

    public static void ApplyDocumentFileUrls(long contractorId, IEnumerable<ContractorDocumentDto> documents, IMediaService mediaService)
    {
        foreach (var doc in documents.Where(d => !string.IsNullOrWhiteSpace(d.Path) && d.Document != null))
        {
            doc.FileUrl = mediaService.Read(ContractorDocumentPathHelper.BuildFilePath(contractorId, doc.Document!.NameEn, doc.Path!));
        }
    }
}
