using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Roles;

public class UpdateRoleRequest
{
    public const string Permission = "manage_permissions";

    [FromRoute]
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string GuardName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public UpdateRoleDto FromRequest() => new()
    {
        Id = Id,
        Name = NameEn,
        NameAr = NameAr,
        GuardName = GuardName,
        Description = Description
    };
}

public class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleRequestValidator(IRoleRepository roleRepository)
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
                .WithMessage("validation.role_name_requires_letter")
            .MustAsync(async (request, name, cancellationToken) =>
            {
                var existing = await roleRepository.GetByNameAndGuardAsync(name, request.GuardName);
                return existing == null || existing.Id == request.Id;
            })
            .WithMessage((request, name) => $"roles.name_guard_already_exists|{name}|{request.GuardName}");
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.GuardName)
            .IsRequiredString(100, "Guard name");
        RuleFor(x => x.Description)
           .IsRequiredString(1000, 10, "Description");
    }
}
