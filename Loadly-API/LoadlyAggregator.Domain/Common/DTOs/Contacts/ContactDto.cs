namespace LoadlyAggregator.Domain.Common.DTOs.Contacts;

public class ContactDto
{
    public long Id { get; set; }
    public string ModelType { get; set; } = string.Empty;
    public long ModelId { get; set; }
    public bool Primary { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public long? CommunicationMethodId { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
