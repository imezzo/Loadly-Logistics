using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

/// <summary>
/// Projection of one <c>trip_documents</c> row (collector workflow: uploads and decisions only).
/// Joined with <c>documents</c> metadata. Driver-uploaded files are not represented here; use
/// <see cref="TripDriverDocumentDto"/> / recordings for that timeline.
/// </summary>
public class TripDocumentDto
{
    /// <summary>FK to <c>documents.id</c>.</summary>
    public long DocumentId { get; set; }

    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public bool Required { get; set; }

    /// <summary>Latest set of paths persisted on the row. Empty when no media yet.</summary>
    public List<string> Paths { get; set; } = [];

    /// <summary>Resolved URLs (<see cref="IMediaService.Read"/>), one per <see cref="Paths"/> entry.</summary>
    public List<string> FileUrls { get; set; } = [];

    /// <summary>Latest collector decision, or <c>null</c> if no collector has reviewed this row yet.</summary>
    public CollectorDocumentStatus? Status { get; set; }

    public string? Note { get; set; }

    public long? AddedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}
