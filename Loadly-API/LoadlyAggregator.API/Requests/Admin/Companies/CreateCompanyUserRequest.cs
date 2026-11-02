using LoadlyAggregator.Shared.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Companies;

public class CreateCompanyUserRequest
{
    public const string Permission = "create_company";

    [FromRoute]
    public long CompanyId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public CommunicationChannel MfaChannel { get; set; } = CommunicationChannel.None;
    public List<long>? RoleIds { get; set; }

    public CreateCompanyUserDto FromRequest() => new()
    {
        CompanyId = CompanyId,
        FullName = FullName,
        UserName = UserName,
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
        RuleFor(x => x.UserName)
            .IsRequiredString(50, 3, "Username")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("validation.username_pattern")
            .IsUnique(async u => await repo.GetByUserNameAsync(u) == null, _ => "companies.username_already_exists");

        RuleFor(x => x.Email).IsRequiredEmail()
            .IsUnique(async e => await repo.GetByEmailAsync(e) == null, _ => "companies.email_already_exists");

        RuleFor(x => x.Phone).IsRequiredEgyptianPhone()
            .IsUnique(async p => await repo.GetByPhoneAsync(p) == null, _ => "companies.phone_already_exists");
    }
}

