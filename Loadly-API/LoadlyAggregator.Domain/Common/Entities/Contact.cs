using LoadlyAggregator.Domain.Common.BaseClasses;
namespace LoadlyAggregator.Domain.Common.Entities;

public class Contact : BaseEntity
{
    public string ModelType { get; set; } = string.Empty;
    public long ModelId { get; set; }
    public bool Primary { get; set; } = false;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public long? CommunicationMethodId { get; set; }
    public long? AddedBy { get; set; }

    public virtual CommunicationMethod? CommunicationMethod { get; set; }
}
