using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.Domain.Common.BaseClasses;

public abstract class BaseUserEntity : BaseEntity, IAuthenticatable, IHasRoles
{
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public CommunicationChannel MfaChannel { get; set; } = CommunicationChannel.None;
    public bool EmailVerified { get; set; } = false;
    public bool PhoneVerified { get; set; } = false;
    public UserStatus Status { get; set; } = UserStatus.ACTIVE;

    public abstract string GuardName { get; }

    public virtual ICollection<ModelHasRole> UserRoles { get; set; } = new List<ModelHasRole>();
    public virtual ICollection<ModelHasPermission> DirectPermissions { get; set; } = new List<ModelHasPermission>();
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
