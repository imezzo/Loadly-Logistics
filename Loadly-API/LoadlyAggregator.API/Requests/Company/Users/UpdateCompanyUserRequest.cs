using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Users;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.API.Requests.Company.Users;

public class UpdateCompanyUserRequest
{
    public const string Permission = "edit_user";

    [FromRoute]
    public long Id { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public CommunicationChannel MfaChannel { get; set; } = CommunicationChannel.None;
    public UserStatus Status { get; set; }
    public List<long> RoleIds { get; set; } = new();

    public CompanyUserManagementDto FromRequest(long companyId) => new()
    {
        Id = Id,
        CompanyId = companyId,
        FullName = FullName,
        UserName = Email,
        Email = Email,
        Phone = Phone,
        MfaChannel = MfaChannel,
        Status = (int)Status,
        RoleIds = RoleIds
    };
}

public class UpdateCompanyUserRequestValidator : AbstractValidator<UpdateCompanyUserRequest>
{
    public UpdateCompanyUserRequestValidator(ICompanyRepository companyRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("User ID");
        RuleFor(x => x.FullName).IsRequiredFullName();
        RuleFor(x => x.Email).IsRequiredEmail()
            .MustAsync(async (request, email, ct) =>
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return true;
                }

                var existing = await companyRepository.GetByEmailAsync(email);
                return existing == null || existing.Id == request.Id;
            })
            .WithMessage("companies.email_already_exists");

        RuleFor(x => x.Phone).IsRequiredEgyptianPhone()
            .MustAsync(async (request, phone, ct) =>
            {
                if (string.IsNullOrWhiteSpace(phone))
                {
                    return true;
                }

                var existing = await companyRepository.GetByPhoneAsync(phone);
                return existing == null || existing.Id == request.Id;
            })
            .WithMessage("companies.phone_already_exists");

        RuleFor(x => x.RoleIds)
            .NotNull()
            .WithMessage("validation.role_ids_required");

        RuleFor(x => x.Status).IsInEnum();

        RuleForEach(x => x.RoleIds)
            .GreaterThan(0)
            .When(x => x.RoleIds != null && x.RoleIds.Any())
            .WithMessage("validation.must_be_greater_than_zero");
    }
}
