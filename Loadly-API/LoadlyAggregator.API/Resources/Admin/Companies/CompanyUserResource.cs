using LoadlyAggregator.API.Resources.Admin.Roles;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;

namespace LoadlyAggregator.API.Resources.Admin.Companies;

public class CompanyUserResource
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
    public int Type { get; set; }
    public long? CompanyId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public UserCompanyDetailsResource? Company { get; set; }
    public List<RoleResource> Roles { get; set; } = new();
    public static CompanyUserResource FromDto(CompanyUserDto dto) =>
        new CompanyUserResource
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
            Type = dto.Type,
            CompanyId = dto.CompanyId,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            Company = dto.Company != null ? UserCompanyDetailsResource.FromDto(dto.Company) : null,
            Roles = dto.Roles.Select(RoleResource.FromDto).ToList()
        };
}

public class UserCompanyDetailsResource
{
    public long? ID { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public DefaultUserResource? DefaultUser { get; set; }
    public static UserCompanyDetailsResource FromDto(UserCompanyDetailsDto dto) => new UserCompanyDetailsResource
    {
        ID = dto.ID,
        NameAr = dto.NameAr,
        NameEn = dto.NameEn,
        DefaultUser = dto.DefaultUser != null ? DefaultUserResource.FromDto(dto.DefaultUser) : null
    };
}

public class DefaultUserResource
{
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int MfaChannel { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public static DefaultUserResource FromDto(DefaultUserDto dto) => new DefaultUserResource
    {
        FullName = dto.FullName,
        UserName = dto.UserName,
        Email = dto.Email,
        Phone = dto.Phone,
        MfaChannel = dto.MfaChannel,
        EmailVerified = dto.EmailVerified,
        PhoneVerified = dto.PhoneVerified
    };
}
