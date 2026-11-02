using FluentValidation;
using LoadlyAggregator.Domain.Collector.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.API.Requests.Collector.Trips;
public class CollectTripDocumentsRequest
{
    public const string Permission = "collect_trip_documents";

    public List<TripDocumentDataRequest> CollectorDocuments { get; set; } = new();

    public bool Submit { get; set; }

    public CollectTripDocumentsDto FromRequest(long tripId, long collectorId) => new()
    {
        TripId = tripId,
        CollectorId = collectorId,
        Submit = Submit,
        CollectorDocuments = CollectorDocuments.Select(d => new TripDocumentDataDto
        {
            DocumentId = d.DocumentId,
            Files = d.Files,
            Status = d.Status,
            Note = d.Note
        }).ToList()
    };
}

/// <summary>One row in <see cref="CollectTripDocumentsRequest.CollectorDocuments"/>.</summary>
public class TripDocumentDataRequest
{
    public long DocumentId { get; set; }
    public List<FileUpload>? Files { get; set; }
    public CollectorDocumentStatus Status { get; set; }
    public string? Note { get; set; }
}

public class CollectTripDocumentsRequestValidator : AbstractValidator<CollectTripDocumentsRequest>
{
    public CollectTripDocumentsRequestValidator()
    {
        RuleFor(x => x.CollectorDocuments)
            .NotNull().WithMessage("trips.documents_required");

        RuleForEach(x => x.CollectorDocuments)
            .ChildRules(doc =>
            {
                doc.RuleFor(d => d.DocumentId)
                    .GreaterThan(0).WithMessage("validation.document_id_gt_zero");

                doc.RuleForEach(d => d.Files)
                    .Must(file => !string.IsNullOrWhiteSpace(file.ContentType) &&
                                  DocumentFileTypeValidator.AllowedMimeTypes.Contains(file.ContentType.Trim().ToLowerInvariant()))
                    .WithMessage("documents.file_type_not_allowed")
                    .When(d => d.Files != null);

                doc.RuleFor(d => d.Files)
                    .Must(files => files == null || files.Count <= 10)
                    .WithMessage("documents.files_must_not_exceed_count|10");

                doc.RuleFor(d => d.Note)
                    .MaximumLength(1000).WithMessage("validation.must_not_exceed_characters|1000")
                    .When(d => !string.IsNullOrWhiteSpace(d.Note));

                doc.RuleFor(d => d.Status)
                    .IsInEnum().WithMessage("validation.invalid_status");

                doc.RuleFor(d => d.Note)
                    .NotEmpty().WithMessage("trips.note_required_when_status_other")
                    .When(d => d.Status == CollectorDocumentStatus.Other);

            });
    }
}
