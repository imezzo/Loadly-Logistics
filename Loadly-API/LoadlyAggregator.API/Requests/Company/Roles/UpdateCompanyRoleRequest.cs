using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Company.DTOs.Roles;

namespace LoadlyAggregator.API.Requests.Company.Roles;

public class UpdateCompanyRoleRequest
{
    public const string Permission = "manage_permissions";

    [FromRoute]
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public UpdateCompanyRoleDto FromRequest(long companyId) => new()
    {
        Id = Id,
        CompanyId = companyId,
        Name = NameEn,
        NameAr = NameAr,
        Description = Description
    };
}

public class UpdateCompanyRoleRequestValidator : AbstractValidator<UpdateCompanyRoleRequest>
{
    public UpdateCompanyRoleRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Role ID");
        RuleFor(x => x.NameEn)
            .IsRequiredString(255, 2, "Role name")
            .Matches(@"^[a-zA-Z0-9\s_-]+$")
            .WithMessage("validation.role_name_chars")
            .Must(name => !string.IsNullOrWhiteSpace(name) && !name.StartsWith(' ') && !name.EndsWith(' '))
            .WithMessage("validation.role_name_trim_whitespace")
            .Must(name => !string.IsNullOrWhiteSpace(name) && !name.Contains("  "))
            .WithMessage("validation.role_name_no_double_spaces")
            .Must(name => !string.IsNullOrWhiteSpace(name) && name.Any(char.IsLetter))
            .WithMessage("validation.role_name_requires_letter");

        RuleFor(x => x.NameAr).IsRequiredArabicName();

        RuleFor(x => x.Description)
            .IsRequiredString(1000, 10, "Description");
    }
}
