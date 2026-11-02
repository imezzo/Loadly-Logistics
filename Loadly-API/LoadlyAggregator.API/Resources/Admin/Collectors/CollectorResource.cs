using LoadlyAggregator.API.Resources.Admin;
using LoadlyAggregator.API.Resources.Admin.Roles;
using LoadlyAggregator.Domain.Admin.DTOs.Collectors;

namespace LoadlyAggregator.API.Resources.Admin.Collectors;

/// <summary>
/// Resource for Collector API responses (both listing and single get)
/// </summary>
public class CollectorResource
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int MfaChannel { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<RoleResource> Roles { get; set; } = new List<RoleResource>();
    public List<AddressResource> Addresses { get; set; } = new();
    public List<ContactResource> Contacts { get; set; } = new();

    public static CollectorResource FromDto(CollectorDto dto) =>
        new CollectorResource
        {
            Id = dto.Id,
            FullName = dto.FullName,
            UserName = dto.UserName,
            Email = dto.Email,
            Phone = dto.Phone,
            MfaChannel = dto.MfaChannel,
            EmailVerified = dto.EmailVerified,
            PhoneVerified = dto.PhoneVerified,
            Status = dto.Status,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            Roles = dto.Roles.Select(RoleResource.FromDto).ToList(),
            Addresses = dto.Addresses.Select(a => AddressResource.FromDto(a)).ToList(),
            Contacts = dto.Contacts.Select(c => ContactResource.FromDto(c)).ToList()
        };
}
