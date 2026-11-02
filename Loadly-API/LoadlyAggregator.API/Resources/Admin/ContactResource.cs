using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Contacts;

namespace LoadlyAggregator.API.Resources.Admin;

/// <summary>
/// Resource for Contact API responses (both listing and single get)
/// </summary>
public class ContactResource
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
    public CommunicationMethodRelationResource? CommunicationMethod { get; set; }

    public static ContactResource FromDto(ContactDto dto, CommunicationMethodDto? communicationMethod = null) =>
        new ContactResource
        {
            Id = dto.Id,
            ModelType = dto.ModelType,
            ModelId = dto.ModelId,
            Primary = dto.Primary,
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Position = dto.Position,
            CommunicationMethodId = dto.CommunicationMethodId,
            CommunicationMethod = communicationMethod != null ? new CommunicationMethodRelationResource
            {
                Id = communicationMethod.Id,
                Name = communicationMethod.NameEn,
                NameAr = communicationMethod.NameAr,
                NameEn = communicationMethod.NameEn
            } : null
        };
}
