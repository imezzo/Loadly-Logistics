using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Users;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.API.Requests.Company.Users;

public class CreateCompanyUserRequest
{
    public const string Permission = "create_user";

    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public CommunicationChannel MfaChannel { get; set; } = CommunicationChannel.None;
    public List<long>? RoleIds { get; set; }

    public CompanyUserManagementDto FromRequest(long companyId) => new()
    {
        CompanyId = companyId,
        FullName = FullName,
        UserName = Email,
        Email = Email,
        Phone = Phone,
        MfaChannel = MfaChannel,
        RoleIds = RoleIds
    };
}

public class CreateCompanyUserRequestValidator : AbstractValidator<CreateCompanyUserRequest>
{
    public CreateCompanyUserRequestValidator(ICompanyRepository repo)
    {
        RuleFor(x => x.FullName).IsRequiredFullName();
        RuleFor(x => x.Email).IsRequiredEmail()
            .IsUnique(async e => await repo.GetByEmailAsync(e) == null, v => $"Email '{v}' exists");

        RuleFor(x => x.Phone).IsRequiredEgyptianPhone()
            .IsUnique(async p => await repo.GetByPhoneAsync(p) == null, v => $"Phone '{v}' exists");
    }
}
