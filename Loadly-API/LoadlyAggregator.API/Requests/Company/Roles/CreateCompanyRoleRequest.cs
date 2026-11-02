using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Company.DTOs.Roles;

namespace LoadlyAggregator.API.Requests.Company.Roles;

public class CreateCompanyRoleRequest
{
    public const string Permission = "manage_permissions";

    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public CreateCompanyRoleDto FromRequest(long companyId) => new()
    {
        CompanyId = companyId,
        Name = NameEn,
        NameAr = NameAr,
        Description = Description
    };
}

public class CreateCompanyRoleRequestValidator : AbstractValidator<CreateCompanyRoleRequest>
{
    public CreateCompanyRoleRequestValidator()
    {
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
