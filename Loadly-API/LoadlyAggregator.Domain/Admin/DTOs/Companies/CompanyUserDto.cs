

using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Companies;
public class CompanyUserDto
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
    public UserCompanyDetailsDto? Company { get; set; }
    public List<RoleDto> Roles { get; set; } = new();
}

public class CreateCompanyUserDto
{
    public long CompanyId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public CommunicationChannel MfaChannel { get; set; } = CommunicationChannel.None;
    public List<long>? RoleIds { get; set; }
}

public class UpdateCompanyUserDto
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public CommunicationChannel MfaChannel { get; set; } = CommunicationChannel.None;

}

public class GetCompanyUserDto
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
}

public class ListCompanyUsersDto : BasePagedRequest
{
    public long CompanyId { get; set; }
    public int? Status { get; set; }
}
public class UserCompanyDetailsDto
{
    public long? ID { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public DefaultUserDto? DefaultUser { get; set; }
}
public class DefaultUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int MfaChannel { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
}

public class DeleteCompanyUserDto
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
}
