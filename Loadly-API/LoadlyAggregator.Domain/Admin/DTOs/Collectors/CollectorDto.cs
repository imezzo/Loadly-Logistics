using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Common.DTOs.Addresses;
using LoadlyAggregator.Domain.Common.DTOs.Contacts;

namespace LoadlyAggregator.Domain.Admin.DTOs.Collectors;

public class CollectorDto
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
    public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
    public List<AddressDto> Addresses { get; set; } = new();
    public List<ContactDto> Contacts { get; set; } = new();
}
