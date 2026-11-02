using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Lightweight document reference (e.g. in contract, company, vehicle, driver documents).</summary>
public class DocumentRelationResource
{
    public long Id { get; set; }
    public DocumentEntityType EntityId { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public bool Required { get; set; }
    public DocumentStatus Status { get; set; }
}
